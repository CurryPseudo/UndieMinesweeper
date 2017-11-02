using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HappyClick : MonoBehaviour {

	public HappyCheck happyCheck;
	public HappyCheckStep happyCheckStep;
	
	[System.NonSerialized]
	public List<IndexOfList2D> outsideUfas = null;
	public List<IndexOfList2D> insideUfas = null;
	public WatchList2DInScene watchList;
	// Use this for initialization
	void Awake () {
		happyCheck = GameObject.Find("Algorithm").GetComponentInChildren<HappyCheck>();
		happyCheckStep = GameObject.Find("Algorithm").GetComponentInChildren<HappyCheckStep>();
		GameObject debugWithMap = new GameObject("Map");
		debugWithMap.transform.parent = transform;
		watchList = debugWithMap.AddComponent(typeof(WatchList2DInScene)) as WatchList2DInScene;
		happyCheckStep.clickAction += changeMineDataForHappy;
	}
	
	public void changeMineDataForHappy(IndexOfList2D clickPos) {
		List2DInt map = new List2DInt(Singleton.MainData.XSize, Singleton.MainData.YSize, 0);
		outsideUfas = new List<IndexOfList2D>();
		insideUfas = new List<IndexOfList2D>();
		foreach(var ufa in happyCheck.unFlipAreaList) {
			outsideUfas.Add(ufa.pos);
		}
		foreach(var pos in happyCheck.unFlipAreaInsideList) {
			if(pos.x != clickPos.x || pos.y != clickPos.y) {
				insideUfas.Add(pos);
			}
		} 
		List<int> trueResult = null;
		var results = happyCheckStep.checkResults;
		int needPutMineCount = 0;
		if(map[clickPos] != -1) {
			if(results.Count > 0) {
				for(int i = 0; i < results.Count; i++) {
					needPutMineCount = Singleton.MainData.mineCount - results[i].Count;
					if(needPutMineCount <= insideUfas.Count) {
						trueResult = results[i];
						break;
					}
				}
			}else{
				trueResult = new List<int>();
				needPutMineCount = Singleton.MainData.mineCount;
			}
		}else{
			for(int i = 0; i < results.Count; i++) {
				var result = results[i];
				bool flag = true;
				foreach(int index in result) {
					var ufa = happyCheckStep.GetUfa(index);
					if(ufa.pos.x == clickPos.x && ufa.pos.y == clickPos.y) {
						flag = false;
						break;
					}
				}
				if(flag) {
					trueResult = result;
				}
			}
		}
		if(trueResult == null) {
			return;
		}
		foreach(int index in trueResult) {
			var ufa = happyCheckStep.GetUfa(index);
			map[ufa.pos] = -1;
		}
		for(int i = 0; i < needPutMineCount; i++) {
			IndexOfList2D pos = insideUfas[(int)(Random.Range(0, insideUfas.Count))];
			insideUfas.Remove(pos);
			map[pos] = -1;
		}
		watchList.list = map;
		var mainData = Singleton.MainData;
		foreach(var ufa in happyCheck.unFlipAreaList) {
			mainData.SetMineData(ufa.pos.x, ufa.pos.y, map[ufa.pos]);
		}
		foreach(var pos in happyCheck.unFlipAreaInsideList) {
			mainData.SetMineData(pos.x, pos.y, map[pos]);
		}
	}
	/// <summary>
	/// Start is called on the frame when a script is enabled just before
	/// any of the Update methods is called the first time.
	/// </summary>
	void Start()
	{
		
	}
	// Update is called once per frame
	void Update () {
		
	}
}
