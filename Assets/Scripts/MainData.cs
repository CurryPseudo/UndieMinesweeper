using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class MainData{
	public MainDataMB mb;
	public float step = 1.06f;
	public int XSize = 0;
	public int YSize = 0;
	public List2DGameObject areaGbs = null;
	public List2DInt mineDatas = new List2DInt(0, 0, 0);
	public int mineCount = 0;
	public Transform transform;
	public GameObject originPrefab;
	public event System.Action BeforeDestroyAction;
	public event System.Action AfterDestroyAction; 
	public Vector3 centerOffSet = new Vector3(0.5f, -0.5f, 0);
	public float outsideWidth = 0.6f; 
	public Vector3 Size{
		get{
			return RightUp - LeftDown;
		}
	}
	public Vector3 LeftDown{
		get{
			//return transform.TransformPoint(AreaPosLocal(0, YSize - 1) + new Vector3(-1.1f, -0.1f, 0));
			return transform.TransformPoint(AreaPosLocal(0, YSize) - centerOffSet + new Vector3(-1, -1, 0) * outsideWidth);
		}
	}
	public Vector3 RightUp{
		get{
			//return transform.TransformPoint(AreaPosLocal(XSize - 1, 0) + new Vector3(1.1f, 0.1f, 0));
			return transform.TransformPoint(AreaPosLocal(XSize, 0) - centerOffSet + new Vector3(1, 1, 0) * outsideWidth);
		}
	}
	public MainData(int x, int y, float _step, Transform _transform, MainDataMB _mb) {
		mb = _mb;
		areaGbs = new List2DGameObject(0, 0, null);
		transform = _transform;
		XSize = x;
		YSize = y;
		mineDatas = new List2DInt(x, y, 0);
		areaGbs = new List2DGameObject(x, y, null);
		step = _step;
		originPrefab = Resources.Load<GameObject> ("Origin");
		foreach(var pos in areaGbs.Positions()) {
			CreateNewGb(pos);
		}
	}
	public int CountingMineCount() {
		int count = 0;
		foreach(var pos in mineDatas.Positions()) {
			if(mineDatas[pos] == -1) {
				count++;
			}
		}
		return count;
	}
	public void CopyMainData(MainData other) {
		foreach(var pos in mineDatas.Positions()) {
			if(other.mineDatas.Inside(pos)) {
				mineDatas[pos] = other.mineDatas[pos];
			}
		}
		mineCount = CountingMineCount();
	}
	void CreateNewGb(IndexOfList2D pos) {
		areaGbs[pos] = GameObject.Instantiate (originPrefab, transform);
		areaGbs[pos].transform.localPosition = AreaPosLocal(pos);
		areaGbs[pos].GetComponentInChildren<AreaView>().x = pos.x;
		areaGbs[pos].GetComponentInChildren<AreaView>().y = pos.y;
			
	}
	public Vector3 AreaPosLocal(IndexOfList2D pos) {
		return AreaPosLocal(pos.x, pos.y);
	}
	public Vector3 AreaPosLocal(int x, int y) {
		var midUpPositionLocal = Vector3.zero;
		var size = new Vector3((XSize - 1) * step + 1, (YSize - 1) * step + 1, 0);	
		var originPositionLocal = midUpPositionLocal + Vector3.left * size.x / 2;
		return originPositionLocal + (Vector3.right * x - Vector3.up * y) * step;
	}
	public Vector3 AreaPosWorld(IndexOfList2D pos) {
		return AreaPosWorld(pos.x, pos.y);
	}	
	public Vector3 AreaPosWorld(int x, int y) {
		return transform.TransformPoint(AreaPosLocal(x, y));
	}
	public int ListIndex(int x, int y) {
		return y * XSize + x;
	}
	public void SetMineData(int x, int y, int value) {
		Debug.Assert(value == -1 || value == 0);
		if(mineDatas[x, y] != -1 && value == -1) {
			mineDatas.ChangeAround(x, y, 
				(int aroundX, int aroundY, int get)=>{
					if(get != -1) {
						return get + 1;
					}else{
						return get;
					}
				}	
			);
			mineDatas[x, y] = -1;
			mineCount++;
		}
		if(mineDatas[x, y] == -1 && value == 0) {
			int mineNum = 0;
			mineDatas.ChangeAround(x, y, 
				(int aroundX, int aroundY, int get)=>{
					if(get != -1) {
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
	public void ReverseMineData(int x, int y) {
		SetMineData(x, y, mineDatas[x, y] == -1 ? 0 : -1);
	}
	public void ResetMineData() {
		for(int i = 0; i < XSize; i++) {
			for(int j = 0; j < YSize; j++) {
				mineDatas[i, j] = 0;
			}
		}
		mineCount = 0;
	}
	public int GetMineData(int x, int y) {
		return mineDatas[x, y];
	}
	public void RandomGenerate(int randomGenerateCount) {
		int tempCount = randomGenerateCount;
		ResetMineData();
		List<IndexOfList2D> positions = new List<IndexOfList2D>();
		foreach(var pos in mineDatas.Positions()) {
			positions.Add(pos);
		}
		while(tempCount != 0) {
			int index = (int)(Random.value * positions.Count);
			IndexOfList2D pos = positions[index];
			positions.Remove(pos);
			ReverseMineData(pos.x, pos.y);
			tempCount--;
		}
	}
	public void Destroy() {
		if(BeforeDestroyAction != null) {
			BeforeDestroyAction();
		}
		foreach(var pos in areaGbs.Positions()) {
			if(areaGbs[pos] != null) {
				GameObject.DestroyImmediate(areaGbs[pos]);
			}
		}
		if(AfterDestroyAction != null) {
			AfterDestroyAction();
		}
	}
	
}
