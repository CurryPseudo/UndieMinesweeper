using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class MainData : MonoBehaviour {
	[Header("MineAreaSize")]
	public int nextX = 10;
	public int nowX = 0;
	public int nextY = 10;
	public int nowY = 0;
	public float step = 1.06f;
	// Use this for initialization
	public bool reGenerate = false;
	public bool realReGenerate = false;
	public bool resetMineData = false;
	public bool randomSetMineData = false;
	public List2DGameObject areaGbs = null;
	public List2DInt mineDatas = new List2DInt(0, 0, 0);
	int[] aroundXs = {-1, 0, 1, -1, 1, -1, 0, 1};
	int[] aroundYs = {-1, -1, -1, 0, 0, 1, 1, 1};
	public int mineCount = 0;
	void Start () {
		if(areaGbs == null)
		areaGbs = new List2DGameObject(0, 0, gameObject);
	}
	void Update () {
		if(reGenerate){
			reGenerate = false;
			ReGenerate();
		}
		if(resetMineData){
			resetMineData = false;
			ResetMineData();
		}
		if(realReGenerate){
			realReGenerate = false;
			int tempX = nextX;
			int tempY = nextY;
			nextX = 0;
			nextY = 0;
			ReGenerate();
			nextX = tempX;
			nextY = tempY;
			ReGenerate();
		}
		if(randomSetMineData){
			randomSetMineData = false;
			RandomGenerate();
		}
	}
	void ReGenerate(){
		for(int i = 0; i < nowX; i++){
			for(int j = 0; j < nowY; j++){
				if(i >= nextX || j >= nextY){
					if(mineDatas[i, j] == -1){
						mineCount--;
					}
					DestroyImmediate(areaGbs[i, j]);
				}
			}
		}
		var newAreaGbs = new List2DGameObject(nextX, nextY, null);
		var originPrefab = Resources.Load<GameObject> ("Origin");
		var newMineData = new List2DInt(nextX, nextY, 0);
		for(int j = 0; j < nextY; j++){
			for(int i = 0; i < nextX; i++){
				if(i < nowX && j < nowY){
					newAreaGbs[i, j] = areaGbs[i, j];
					newMineData[i, j] = mineDatas[i, j];
				}
				else{
					var newInstance = Instantiate (originPrefab, transform);
					newAreaGbs[i, j] = newInstance;
					newMineData[i, j] = 0;
				}
				newAreaGbs[i, j].transform.localPosition = ((Vector3.right * i - Vector3.up * j) * step);
				newAreaGbs[i, j].GetComponentInChildren<AreaView>().x = i;
				newAreaGbs[i, j].GetComponentInChildren<AreaView>().y = j;
			}
		}
		
		areaGbs = newAreaGbs;
		mineDatas = newMineData;
		nowX = nextX;
		nowY = nextY;
	}
	public Vector3 AreaPosLocal(int x, int y){
		return (Vector3.right * x - Vector3.up * y) * step;
	}
	public Vector3 AreaPosWorld(IndexOfList2D pos){
		return AreaPosWorld(pos.x, pos.y);
	}	
	public Vector3 AreaPosWorld(int x, int y){
		return transform.TransformPoint((Vector3.right * x - Vector3.up * y) * step);
	}
	public int ListIndex(int x, int y){
		return y * nowX + x;
	}
	public void SetMineData(int x, int y, int value){
		Debug.Assert(value == -1 || value == 0);
		if(mineDatas[x, y] != -1 && value == -1){
			mineDatas.ChangeAround(x, y, 
				(int aroundX, int aroundY, int get)=>{
					if(get != -1){
						return get + 1;
					}else{
						return get;
					}
				}	
			);
			mineDatas[x, y] = -1;
			mineCount++;
		}
		if(mineDatas[x, y] == -1 && value == 0){
			int mineNum = 0;
			mineDatas.ChangeAround(x, y, 
				(int aroundX, int aroundY, int get)=>{
					if(get != -1){
						return get - 1;
					}else{
						mineNum++;
						return get;
					}
				}	
			);
			mineDatas[x, y] = mineNum;
			mineCount--;
		}
	}
	public void ReverseMineData(int x, int y){
		SetMineData(x, y, mineDatas[x, y] == -1 ? 0 : -1);
	}
	public void ResetMineData(){
		for(int i = 0; i < nowX; i++){
			for(int j = 0; j < nowY; j++){
				mineDatas[i, j] = 0;
			}
		}
		mineCount = 0;
	}
	public int GetMineData(int x, int y){
		return mineDatas[x, y];
	}
	public void RandomGenerate(){
		int tempCount = mineCount;
		ResetMineData();
		while(tempCount != 0){
			int x = (int)(Random.value * nowX);
			int y = (int)(Random.value * nowY);
			if(mineDatas[x, y] != -1){
				ReverseMineData(x, y);
				tempCount--;
			}
		}
	}
}
