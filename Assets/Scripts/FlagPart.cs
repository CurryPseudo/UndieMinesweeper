using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagPart : MonoBehaviour {
	public List2DInt flagMap;
	public List2DGameObject flagGbs;
	public int FlagCount{
		get{
			int count = 0;
			foreach(var pos in flagMap.Positions()) {
				if(flagMap[pos] == 1) {
					count++;
				}
			}
			return count;
		}
	}
	public void ReStart() {
		Destroy();
		flagMap = Singleton.CreateNewList2DInt();
		flagGbs = new List2DGameObject(Singleton.MainData.XSize, Singleton.MainData.YSize, null);
	}
	// Use this for initialization
	void Start () {
		flagMap = Singleton.CreateNewList2DInt();
		flagGbs = new List2DGameObject(Singleton.MainData.XSize, Singleton.MainData.YSize, null);
	}
	public void Destroy() {
		foreach(var pos in flagGbs.Positions()) {
			if(flagGbs[pos] != null) {
				DestroyImmediate(flagGbs[pos]);
			}
		}
	}
	public void RefreshView() {
		for(int i = 0; i < flagMap.XSize; i++) {
			for(int j = 0; j < flagGbs.YSize; j++) {
				var pos = new IndexOfList2D(i, j);
				if(flagMap[pos] == 0 && flagGbs[pos] != null) {
					Destroy(flagGbs[pos]);
					flagGbs[pos] = null;
				}else
				if(flagMap[pos] == 1 && flagGbs[pos] == null) {
					CreateFlagGb(pos);
				}
				if(flagGbs[pos] != null) {
					flagGbs[pos].transform.position = Singleton.MainData.AreaPosWorld(pos);
				}
			}
		}
	}
	public void CreateFlagGb(IndexOfList2D pos) {
		GameObject flagFather = GameObject.Find("Flags");
		if(flagFather == null) {
			flagFather = new GameObject("Flags");
		}
		var flagPrefab = Resources.Load<GameObject>("flag");					
		flagGbs[pos] = Instantiate(flagPrefab);
		flagGbs[pos].transform.parent = flagFather.transform;
		
	}
	public void FlipAround(IndexOfList2D pos){
		bool hasFlagAround = false;
		flagMap.ChangeAround(pos.x, pos.y,
			(int aroundX, int aroundY, int get)=>{
				if(get == 1) {
					hasFlagAround = true;
				}
				return get;
			}
		);
		if(hasFlagAround) {
			flagMap.ChangeAround(pos.x, pos.y,
				(int aroundX, int aroundY, int get)=>{
					if(get != 1) {
						Singleton.GamePart.AreaClick(aroundX, aroundY);
					}
					return get;
				}
			);
		}

	}
	public void MidclickPos(IndexOfList2D pos){
		if(Singleton.GamePart.flipBoard[pos] == 1){
			FlipAround(pos);
		}
	}
	public void RclickPos(IndexOfList2D pos) {
		if(Singleton.GamePart.flipBoard[pos] != 1) {
			flagMap[pos] = 1 - flagMap[pos];
			RefreshView();
		}
	}
	// Update is called once per frame
	void Update () {
		
	}
}
