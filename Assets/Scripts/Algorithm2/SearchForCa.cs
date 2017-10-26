using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchForCa : MonoBehaviour {
	public bool step = false;
	public bool process = false;
	public bool processWithDelayTime = false;
	public bool stopProcess = false;
	public bool backToStart = false;
	public ConnectedAreas ca;
	public Dictionary<Area, int> areaIndexTable;
	public int numberCount = 0;
	public Area[] searchNumbers;
	public int[] pointForEachNumber;
	public List<int>[] stackForEachNumber;
	public int numberIndex = 0;
	public int neighbourIndex = 0;

	public Dictionary<int, List<List<IndexOfList2D>>> searchResults;
	// Use this for initialization
	public SearchingStepResult stepResult = SearchingStepResult.ORIGIN;
	public event System.Action<List<IndexOfList2D>> afterGetResultAction;
	public Precalculate precac;
	public float processDelayTime = 0.5f;
	void Start () {
		precac = GetComponentInParent<Precalculate>();
		precac.afterPrecacAction += RefreshCa;
		afterGetResultAction += DebugCreateShowPosition;
		RefreshCa();
	}
	public void RefreshCa(){
		var showCa =transform.parent.gameObject.GetComponent<ShowCa>();
		if(showCa != null){
			SearchInit(showCa.ca);
		}
	}
	public void DebugCreateShowPosition(List<IndexOfList2D> result){
		GameObject gb = new GameObject("StepResult");
		var showPositions = gb.AddComponent<ShowPositions>();
		showPositions.positions = result;
		gb.transform.parent = transform;
	}
	/// <summary>
	/// Update is called every frame, if the MonoBehaviour is enabled.
	/// </summary>
	void Update()
	{
		if(step){
			step = false;
			Step();
		}
		if(process){
			process= false;
			while(stepResult != SearchingStepResult.END){
				Step();
			}
		}
		if(backToStart){
			backToStart = false;
			if(ca != null){
				SearchInit(ca);
			}
		}
		if(processWithDelayTime){
			processWithDelayTime = false;
			StartCoroutine(ProcessWithDelayTime());
		}
		if(stopProcess){
			stopProcess = false;
			StopAllCoroutines();
		}
	}
	/// <summary>
	/// This function is called when the MonoBehaviour will be destroyed.
	/// </summary>
	void OnDestroy()
	{
		precac = GetComponentInParent<Precalculate>();
		if(precac != null){
			precac.afterPrecacAction -= RefreshCa;
		}
	}
	IEnumerator ProcessWithDelayTime(){
		while(stepResult != SearchingStepResult.END){
			Step();
			yield return new WaitForSeconds(processDelayTime);
		}
	}
	public void SearchInit(ConnectedAreas _ca){
		ca = _ca;
		numberCount = ca.numbers.Count;
		searchNumbers = new Area[numberCount];
		ca.numbers.CopyTo(searchNumbers);
		
		areaIndexTable = new Dictionary<Area, int>();
		for(int i = 0; i < numberCount; i++){
			areaIndexTable[searchNumbers[i]] = i;
		}

		pointForEachNumber = new int[numberCount];
		for(int i = 0; i < numberCount; i++){
			pointForEachNumber[i] = Singleton.MainData.mineDatas[searchNumbers[i].pos];
		}

		stackForEachNumber = new List<int>[numberCount];
		for(int i = 0; i < numberCount; i++){
			stackForEachNumber[i] = new List<int>();
		}

		numberIndex = 0;
		neighbourIndex = 0;
		
		searchResults = new Dictionary<int, List<List<IndexOfList2D>>>();
		stepResult = SearchingStepResult.ORIGIN;

		ShowPositions[] showPositionsArray = GetComponentsInChildren<ShowPositions>();
		foreach(var showPositions in showPositionsArray){
			Destroy(showPositions.gameObject);
		}
	}
	public void Step(){
		if(stepResult == SearchingStepResult.GETRESULT){
			numberIndex--;
			Backtracking();
		}else
		if(numberIndex < 0){
			stepResult = SearchingStepResult.END;
		}else
		if(numberIndex >= numberCount){
			var oneResult = GetMineList();
			int mineCount = oneResult.Count;
			if(!searchResults.ContainsKey(mineCount)){
				searchResults[mineCount] = new List<List<IndexOfList2D>>();
			}
			searchResults[mineCount].Add(oneResult);
			if(afterGetResultAction != null){
				afterGetResultAction(oneResult);
			}
			stepResult = SearchingStepResult.GETRESULT;
		}else
		if(pointForEachNumber[numberIndex] == 0){
			numberIndex++;
			neighbourIndex = 0;
			stepResult = SearchingStepResult.SKIPNUMBER;
		}else
		if(neighbourIndex >= searchNumbers[numberIndex].neighbours.Count){
			Backtracking();
		}else{
			var searchingNumber = searchNumbers[numberIndex];
			var searchingNeighbour = searchingNumber.neighbours[neighbourIndex];
			bool flag = true;
			foreach(var neighbourNumber in searchingNeighbour.neighbours){
				int index = areaIndexTable[neighbourNumber];
				if(pointForEachNumber[index] == 0){
					flag = false;
					break;
				}
			}
			if(flag){
				foreach(var neighbourNumber in searchingNeighbour.neighbours){
					int index = areaIndexTable[neighbourNumber];
					pointForEachNumber[index]--;
				}
				var stack = stackForEachNumber[numberIndex];
				stack.Add(neighbourIndex);
				stepResult = SearchingStepResult.PUSHSTACK;
			}else{
				stepResult = SearchingStepResult.NORMAL; 
			}
			neighbourIndex++;
		}
	}
	public void Backtracking(){
		while(numberIndex >= 0 && stackForEachNumber[numberIndex].Count == 0){
			numberIndex--;
		}
		if(numberIndex < 0){
			stepResult = SearchingStepResult.END;
		}else{
			var number = searchNumbers[numberIndex];
			var stack = stackForEachNumber[numberIndex];
			int backNeighbourIndex = stack[stack.Count - 1];
			stack.RemoveAt(stack.Count - 1);
			var backNeighbour = number.neighbours[backNeighbourIndex];
			foreach(var neighbourNumber in backNeighbour.neighbours){
				int index = areaIndexTable[neighbourNumber];
				pointForEachNumber[index]++;
			}
			neighbourIndex = backNeighbourIndex + 1;
			stepResult = SearchingStepResult.BACKTRACKING;
		}
	}
	public List<IndexOfList2D> GetMineList(){
		List<IndexOfList2D> oneResult = new List<IndexOfList2D>();
		for(int i = 0; i < numberCount; i++){
			var stack = stackForEachNumber[i];
			var number = searchNumbers[i];
			foreach(var neighbourIndex in stack){
				oneResult.Add(number.neighbours[neighbourIndex].pos);
			}
		}
		return oneResult;	
		
	}
	public List<Area> GetNumberList(){
		List<Area> result = new List<Area>();
		foreach(var area in searchNumbers){
			result.Add(area);
		}
		return result;
	}
	public Area GetIteratingNumber(){
		if(numberIndex < 0 || numberIndex >= numberCount){
			return null;
		}
		return searchNumbers[numberIndex];
	}
	public int GetNumberPoint(Area number){
		int index = areaIndexTable[number];
		return pointForEachNumber[index];
	}
	public Area GetIteratingOutside(){
		Area number = GetIteratingNumber();
		if(number != null){
			if(neighbourIndex < number.neighbours.Count){
				return number.neighbours[neighbourIndex];
			}
		}
		return null;
	}
}
public enum SearchingStepResult{
	ORIGIN, NORMAL, END, SKIPNUMBER, BACKTRACKING, PUSHSTACK, GETRESULT
}