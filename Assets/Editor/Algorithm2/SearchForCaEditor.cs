using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(SearchForCaMB))]
public class SearchForCaEditor : Editor {

	public SearchForCa searchForCa = null;
	public MainData mainData = null;
	public int areaType = 0;
	public Area now = null;
	// Use this for initialization
	public void OnSceneGUI(){
		if(searchForCa == null){
			searchForCa = (target as SearchForCaMB).searchForCa;
		}
		if(mainData == null){
			mainData = Singleton.MainData;
		}
		if(searchForCa.ca != null){
			foreach(var minePos in searchForCa.GetMineList()){
				Singleton.DrawABeaultifulButton(minePos,searchForCa.stepResult == SearchingStepResult.GETRESULT ? Color.green : Color.red, null);
			}
			foreach(var number in searchForCa.GetNumberList()){
				var point = searchForCa.GetNumberPoint(number);
				Singleton.DrawABeaultifulButton(number.pos, point == 0 ? Color.grey : Color.yellow, null);
				Singleton.DrawABeaultifulLabel(number.pos, point.ToString());
				if(point == 0){
					foreach(var neighbour in number.neighbours){
						Singleton.DrawABeautifulSmallButton(neighbour.pos, Color.grey, null);
					}
				}
			}
			var iteratingNumber = searchForCa.GetIteratingNumber();
			if(iteratingNumber != null){
				var point = searchForCa.GetNumberPoint(iteratingNumber);
				Singleton.DrawABeaultifulButton(iteratingNumber.pos, Color.blue, null);
				Singleton.DrawABeaultifulLabel(iteratingNumber.pos, point.ToString());
				var iteratingOutside = searchForCa.GetIteratingOutside(); 
				if(iteratingOutside != null){
					Singleton.DrawABeaultifulButton(iteratingOutside.pos, Color.blue, null);
				}
			}
		}
	}
}
