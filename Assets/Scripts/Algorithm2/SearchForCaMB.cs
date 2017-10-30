using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchForCaMB : MonoBehaviour {
	public bool step = false;
	public bool process = false;
	public bool backToStart = false;
	public bool processWithDelayTime = false;
	public bool stopProcess = false;
	public bool visualizedData = false;
	public SearchForCa searchForCa = null;
	public float processDelayTime = 0.1f;
	void Start () {
	}

	/// <summary>
	/// Update is called every frame, if the MonoBehaviour is enabled.
	/// </summary>
	void Update()
	{
		if(searchForCa != null){
			if(step){
				step = false;
				searchForCa.Step();
			}
			if(process){
				process = false;
				searchForCa.Process();
			}
			if(backToStart){
				backToStart = false;
				searchForCa = new SearchForCa(searchForCa.ca);
			}
			if(processWithDelayTime){
				processWithDelayTime = false;
				StartCoroutine(ProcessWithDelayTime());
			}
			if(stopProcess){
				stopProcess = false;
				StopAllCoroutines();
			}
			if(visualizedData){
				visualizedData = false;
				VisualizedData();
			}
		}
		
	}
	/// <summary>
	/// This function is called when the MonoBehaviour will be destroyed.
	/// </summary>

	public void VisualizedData(){
		Singleton.DestroyAllChilds(transform);
		Dictionary<int, GameObject> gbs = new Dictionary<int, GameObject>();
		foreach(var key in searchForCa.searchResults.Keys){
			GameObject gb = new GameObject(key.ToString());
			gb.transform.parent = transform;
			gbs[key] = gb;
		}
		foreach(var keyAndValue in searchForCa.searchResults){
			int resultIndex = 0;
			foreach(var result in keyAndValue.Value){
				GameObject valueGb = new GameObject("Result " + resultIndex.ToString());
				valueGb.transform.parent = gbs[keyAndValue.Key].transform;
				ShowPositions sp = valueGb.AddComponent<ShowPositions>();
				sp.positionsList.Add(new ShowPositions.PositionProperty(result, Color.green));
				resultIndex++;
			}
			
		}
	}

	public void ProcessWithDelayTimeAction(){
		StartCoroutine(ProcessWithDelayTime());
	}
	IEnumerator ProcessWithDelayTime(){
		while(searchForCa.stepResult != SearchingStepResult.END){
			searchForCa.Step();
			yield return new WaitForSeconds(processDelayTime);
		}
		searchForCa.CheckDead();
	}
}
