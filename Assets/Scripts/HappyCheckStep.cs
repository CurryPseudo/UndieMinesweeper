using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HappyCheckStep : MonoBehaviour {
	public HappyCheck happyCheck;
	public bool step = false;
	public bool bigStep = false;
	public bool mineStep = false;
	public bool process = false;
	public bool backToStart = false;
	public bool checkHappy = false;
	[System.NonSerialized]
	public List2DInt ufaMap = null;
	public int[] ufaIndexes;
	public int numberCount;
	public List<int> stack;
	// Use this for initialization
	public int sortIndex;
	WatchList2DInScene watchList;
	public bool isHappy = false;
	public int[] happyMap;
	public List<List<int>> checkResults;
	public StepResult stepResult = StepResult.NORMAL;
	void Start () {
		happyCheck = transform.parent.GetComponentInChildren<HappyCheck>();
		var md = GameObject.Find("MainData");
		var gamePart = md.GetComponent<GamePart>();
		gamePart.flipAction += stepInit;
		GameObject debugWithMap = new GameObject("Map");
		debugWithMap.transform.parent = transform;
		watchList = debugWithMap.AddComponent(typeof(WatchList2DInScene)) as WatchList2DInScene;
		stepInit(null);
	}
	
	// Update is called once per frame
	void Update () {
		if(step){
			step = false;
			Step();
		}
		if(process){
			process = false;
			Process();
		}
		if(bigStep){
			bigStep = false;
			var result = Step();
			while(result != StepResult.END && result != StepResult.GETRESULT){
				result = Step();
			}
		}
		if(backToStart){
			backToStart = false;
			Process();
			stepInit(null);
		}
		if(checkHappy){
			checkHappy = false;
			CheckHappy();
		}
		if(mineStep){
			mineStep = false;
			var result = Step();
			while(result != StepResult.END && result != StepResult.GETRESULT && result != StepResult.PUTMINE){
				result = Step();
			}
		}
		
	}
	public void stepInit(List<FlipNode> nodes){
		ufaMap = new List2DInt(happyCheck.mineDatas.XSize, happyCheck.mineDatas.YSize, -1);
		watchList.list = ufaMap;
		ufaIndexes = new int[happyCheck.unFlipAreaList.Count];
		numberCount = happyCheck.numberList.Count;
		int index = 0;
		foreach(var ufa in happyCheck.unFlipAreaList){
			ufaIndexes[index] = happyCheck.map[ufa.pos];
			ufaMap[ufa.pos] = 0;
			index++;
		}
		stack = new List<int>();
		happyMap = new int[happyCheck.unFlipAreaList.Count];
		checkResults = new List<List<int>>();
		isHappy = false;
		GameObject[] resultGbs = new GameObject[transform.childCount];
		sortIndex = 0;
		for(int i = 0; i < transform.childCount; i++){
			resultGbs[i] = transform.GetChild(i).gameObject;
		}
		foreach(var resultGb in resultGbs){
			Destroy(resultGb);
		}
	}
	public void Process(){
		while(Step() != StepResult.END);
	}
	public void CheckHappy(){
		int[] resultCacMap = new int[happyCheck.unFlipAreaList.Count];
		for(int i = 0; i < resultCacMap.Length; i++){
			resultCacMap[i] = 0;
		}
		int mineCountLeft = resultCacMap.Length;
		foreach(var result in checkResults){
			foreach(var index in result){
				if(resultCacMap[index] == 0){
					mineCountLeft--;
					resultCacMap[index] = 1;
				}
			}
		}
		isHappy = mineCountLeft == 0;
	}
	public StepResult Step(){
		stepResult = StepResult.END;
		if(sortIndex < happyCheck.unFlipAreaList.Count || stack.Count > 0){
			stepResult = StepResult.NORMAL;
			if(sortIndex == happyCheck.unFlipAreaList.Count){
				sortIndex = stack[stack.Count - 1];
				stack.RemoveAt(stack.Count - 1);
				HCArea ufa = happyCheck.unFlipAreaList[ufaIndexes[sortIndex]];
				foreach(HCArea number in ufa.neighbours){
					number.value++;
					if(number.value == 1){
						numberCount++;
						foreach(var neighbourUfa in number.neighbours){
							ufaMap[neighbourUfa.pos] = 0;
						}
					}
				}
			}else{
				if(ufaMap[happyCheck.unFlipAreaList[ufaIndexes[sortIndex]].pos] == 0){
					stack.Add(sortIndex);
					HCArea ufa = happyCheck.unFlipAreaList[ufaIndexes[sortIndex]];
					foreach(HCArea number in ufa.neighbours){
						number.value--;
						if(number.value == 0){
							numberCount--;
							foreach(var neighbourUfa in number.neighbours){
								ufaMap[neighbourUfa.pos] = 1;
							}
						}
					}
					stepResult = StepResult.PUTMINE;
					if(numberCount == 0 && stack.Count <= MainDataSingleton.value.mineCount){
						List<int> oneResult = new List<int>(stack);
						checkResults.Add(oneResult);
						stepResult = StepResult.GETRESULT;
						createResultGb(oneResult);
					
					}
				}
			}
			sortIndex++;
		}
		return stepResult;
	}
	public HCArea GetUfa(int index){
        return happyCheck.unFlipAreaList[ufaIndexes[index]];
    }
	public enum StepResult{
		NORMAL, GETRESULT, END, PUTMINE
	}
	void createResultGb(List<int> result){
		var gb = new GameObject("stepResult");
		gb.transform.parent = transform;
		ShowMineResult sm = gb.AddComponent(typeof(ShowMineResult)) as ShowMineResult;
		sm.happyCheckStep = this;
		sm.result = result;
	}
}
