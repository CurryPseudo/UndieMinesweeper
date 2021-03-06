﻿using System.Collections;
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
	public bool processWithDelayTime = false;
	public bool stopProcess = false;
	public float processDelayTime = 0.1f;
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
	public List<List<int>> checkResults = new List<List<int>>();
	public StepResult stepResult = StepResult.NORMAL;
	public event GamePart.ClickAction clickAction;
	void Start () {
		var md = GameObject.Find("MainData");
		var gamePart = md.GetComponent<GamePart>();
		GameObject debugWithMap = new GameObject("Map");
		//gamePart.clickAction += ProcessAfterClick;
		//gamePart.flipAction += ProcessAfterFlip;
		debugWithMap.transform.parent = transform;
		watchList = debugWithMap.AddComponent(typeof(WatchList2DInScene)) as WatchList2DInScene;
		StepInit();
		CheckHappy();
	}
	
	// Update is called once per frame
	void Update () {
		if(step) {
			step = false;
			Step();
		}
		if(process) {
			process = false;
			Process();
		}
		if(bigStep) {
			bigStep = false;
			var result = Step();
			while(result != StepResult.END && result != StepResult.GETRESULT) {
				result = Step();
			}
		}
		if(backToStart) {
			backToStart = false;
			StepInit();
		}
		if(checkHappy) {
			checkHappy = false;
			CheckHappy();
		}
		if(mineStep) {
			mineStep = false;
			var result = Step();
			while(result != StepResult.END && result != StepResult.GETRESULT && result != StepResult.PUTMINE) {
				result = Step();
			}
		}
		if(processWithDelayTime) {
			processWithDelayTime = false;
			StartCoroutine(ProcessWithDelayTime());
		}
		if(stopProcess) {
			stopProcess = false;
			StopAllCoroutines();
		}
	}
	IEnumerator ProcessWithDelayTime() {
		while(stepResult != StepResult.END) {
			Step();
			yield return new WaitForSeconds(processDelayTime);
		}
	}
	public void ProcessAfterClick(IndexOfList2D clickPos) {
		if(isHappy) {
			if(clickAction != null) {
				clickAction.Invoke(clickPos);
			}
		}
	}
	public void ProcessAfterFlip(List<FlipNode> flipNodes) {
		StepInit();
		ProcessWithCheckHappy();
	}
	public void StepInit() {
		if(happyCheck == null) {
			happyCheck = transform.parent.GetComponentInChildren<HappyCheck>();
		}
		happyCheck.resetNumbersValue();
		ufaMap = new List2DInt(happyCheck.mineDatas.XSize, happyCheck.mineDatas.YSize, -1);
		watchList.list = ufaMap;
		ufaIndexes = new int[happyCheck.unFlipAreaList.Count];
		numberCount = happyCheck.numberList.Count;
		int index = 0;
		foreach(var ufa in happyCheck.unFlipAreaList) {
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
		for(int i = 0; i < transform.childCount; i++) {
			resultGbs[i] = transform.GetChild(i).gameObject;
		}
		foreach(var resultGb in resultGbs) {
			Destroy(resultGb);
		}
	}
	
	public void ProcessWithCheckHappy() {
		isHappy = false;
		int[] resultCacMap = new int[happyCheck.unFlipAreaList.Count];
		for(int i = 0; i < resultCacMap.Length; i++) {
			resultCacMap[i] = 0;
		}
		int mineCountLeft = resultCacMap.Length;
		StepResult result = StepResult.NORMAL;
		while(result != StepResult.END) {
			result = Step();
			while(result != StepResult.GETRESULT && result != StepResult.END) {
				result = Step();
			}
			if(result != StepResult.END) {
				var oneCheckResult = checkResults[checkResults.Count - 1];
				foreach(var index in oneCheckResult) {
					if(resultCacMap[index] == 0) {
							mineCountLeft--;
						resultCacMap[index] = 1;
					}
				}
			}
			if(mineCountLeft == 0) {
				isHappy = true;
				break;
			}
		}
		happyCheck.resetNumbersValue();
	}
	public void Process() {
		while(Step() != StepResult.END);
	}
	public void CheckHappy() {
		int[] resultCacMap = new int[happyCheck.unFlipAreaList.Count];
		for(int i = 0; i < resultCacMap.Length; i++) {
			resultCacMap[i] = 0;
		}
		int mineCountLeft = resultCacMap.Length;
		foreach(var result in checkResults) {
			foreach(var index in result) {
				if(resultCacMap[index] == 0) {
					mineCountLeft--;
					resultCacMap[index] = 1;
				}
			}
		}
		isHappy = mineCountLeft == 0;
	}
	public StepResult Step() {
		stepResult = StepResult.END;
		if(sortIndex < happyCheck.unFlipAreaList.Count || stack.Count > 0) {
			stepResult = StepResult.NORMAL;
			if(sortIndex == happyCheck.unFlipAreaList.Count) {
				sortIndex = stack[stack.Count - 1];
				stack.RemoveAt(stack.Count - 1);
				HCArea ufa = happyCheck.unFlipAreaList[ufaIndexes[sortIndex]];
				foreach(HCArea number in ufa.neighbours) {
					number.value++;
					if(number.value == 1) {
						numberCount++;
						foreach(var neighbourUfa in number.neighbours) {
							ufaMap[neighbourUfa.pos] = 0;
						}
					}
				}
			}else{
				if(ufaMap[happyCheck.unFlipAreaList[ufaIndexes[sortIndex]].pos] == 0) {
					stack.Add(sortIndex);
					HCArea ufa = happyCheck.unFlipAreaList[ufaIndexes[sortIndex]];
					foreach(HCArea number in ufa.neighbours) {
						number.value--;
						if(number.value == 0) {
							numberCount--;
							foreach(var neighbourUfa in number.neighbours) {
								ufaMap[neighbourUfa.pos] = 1;
							}
						}
					}
					stepResult = StepResult.PUTMINE;
					if(numberCount == 0 && stack.Count <= Singleton.MainData.mineCount && stack.Count + happyCheck.unFlipAreaInsideList.Count >= Singleton.MainData.mineCount) {
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
	public HCArea GetUfa(int index) {
        return happyCheck.unFlipAreaList[ufaIndexes[index]];
    }
	public enum StepResult{
		NORMAL, GETRESULT, END, PUTMINE
	}
	void createResultGb(List<int> result) {
		var gb = new GameObject("StepResult");
		gb.transform.parent = transform;
		ShowMineResult sm = gb.AddComponent(typeof(ShowMineResult)) as ShowMineResult;
		sm.happyCheckStep = this;
		sm.result = result;
	}
}
