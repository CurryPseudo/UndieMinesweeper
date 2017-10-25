using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HappyCheck : MonoBehaviour {
	public List2DInt mineDatas;
	public List2DInt flipBoard;
	// Use this for initialization
	[System.NonSerialized]
	public List2DInt map = null;
	public SmartList<HCArea> numberList = null;
	public SmartList<HCArea> unFlipAreaList = null;
	void Awake () {
		var md = GameObject.Find("MainData");
		mineDatas = md.GetComponent<MainData>().mineDatas;
		flipBoard = md.GetComponent<GamePart>().flipBoard;
		map = new List2DInt(mineDatas.XSize, mineDatas.YSize, -1);
		numberList = new SmartList<HCArea>();
		unFlipAreaList = new SmartList<HCArea>();
		var gamePart = md.GetComponent<GamePart>();
		gamePart.flipAction += ChangeMapByFlip;
		
		GameObject debugWithMap = new GameObject("Map");
		debugWithMap.transform.parent = transform;
		debugWithMap.AddComponent(typeof(WatchList2DInScene));
		debugWithMap.GetComponent<WatchList2DInScene>().list = map;
		
		
	}
	public void AddUnflipArea(HCArea ufa){
		map[ufa.pos] = unFlipAreaList.Add(ufa);
	}
	public void AddNumber(HCArea number){
		map[number.pos] = -2 - numberList.Add(number);
	}
	public HCArea GetHCArea(int x, int y){
		if(map[x, y] > -1){
			return unFlipAreaList[map[x, y]];
		}else if(map[x, y] < -1){
			return numberList[-2 -map[x, y]];
		}else{
			return null;
		}
	}
	public void RemoveUnflipArea(HCArea ufa){
		unFlipAreaList.RemoveAt(map[ufa.pos]);
		map[ufa.pos] = -1;
	}
	public void RemoveNumber(HCArea number){
		numberList.RemoveAt(-2 - map[number.pos]);
		map[number.pos] = -1;
	}
	public void ChangeMapByFlip(List<FlipNode> flipNodes){
		foreach(var node in flipNodes){
			if(map[node.x, node.y] != -1){
				var ufa = GetHCArea(node.x, node.y);
				foreach(var aroundNumber in ufa.neighbours){
					aroundNumber.neighbours.Remove(ufa);
					if(aroundNumber.neighbours.Count == 0){
						RemoveNumber(aroundNumber);
					}
				}
				RemoveUnflipArea(ufa);
			}
			List<HCArea> aroundUfa = new List<HCArea>();
			HCArea number = new HCArea(mineDatas[node.x, node.y], new List2DIndex(node.x, node.y), aroundUfa);
			flipBoard.ChangeAround(node.x, node.y,
				(int x, int y, int get)=>{
					if(get == 0){
						HCArea ufa = GetHCArea(x, y);
						if(ufa == null){
							ufa = new HCArea(-1, new List2DIndex(x, y), new List<HCArea>());
							AddUnflipArea(ufa);
						}
						ufa.neighbours.Add(number);
						aroundUfa.Add(ufa);
					}
					return get;
				}
			);
			if(aroundUfa.Count > 0){
				AddNumber(number);
			}
		}

	}
	/*public void Check(){
		if(unFlipAreaList.Count == 0) return;
		List2DInt ufaMap = new List2DInt(mineDatas.XSize, mineDatas.YSize, -1);
		int[] ufaIndexes = new int[unFlipAreaList.Count];
		int numberCount = numberList.Count;
		int index = 0;
		foreach(var ufa in unFlipAreaList){
			ufaIndexes[index] = map[ufa.pos];
			ufaMap[ufa.pos] = 0;
		}
		List<int> stack = new List<int>();
		int sortIndex = 0;
		while(sortIndex < unFlipAreaList.Count || stack.Count > 0){
			if(numberCount == 0){
				print(stack);
			}
			if(sortIndex == unFlipAreaList.Count){
				sortIndex = stack[stack.Count - 1];
				stack.RemoveAt(stack.Count - 1);
				HCArea ufa = unFlipAreaList[ufaIndexes[sortIndex]];
				foreach(HCArea number in ufa.neighbours){
					number.value++;
					if(number.value == 1){
						numberCount++;
						foreach(var neighbourUfa in number.neighbours){
							ufaMap[neighbourUfa.pos] = 0;
						}
					}
				}
			}else
			if(ufaMap[unFlipAreaList[ufaIndexes[sortIndex]].pos] == 0){
				stack.Add(sortIndex);
				HCArea ufa = unFlipAreaList[ufaIndexes[sortIndex]];
				foreach(HCArea number in ufa.neighbours){
					number.value--;
					if(number.value == 0){
						numberCount--;
						foreach(var neighbourUfa in number.neighbours){
							ufaMap[neighbourUfa.pos] = 1;
						}
					}
				}
			}
			sortIndex++;
		}
	}*/
	// Update is called once per frame
	void Update () {
		
	}

}
public class HCArea{
	public int value;
	public List2DIndex pos;
	public List<HCArea> neighbours;
	public HCArea(int _value, List2DIndex _pos, List<HCArea> _neighbours){
		pos = _pos;
		neighbours = _neighbours;
		value = _value;
	}
}
