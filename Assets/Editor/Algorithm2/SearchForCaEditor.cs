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
	public void OnSceneGUI() {
		if(searchForCa == null) {
			searchForCa = (target as SearchForCaMB).searchForCa;
		}
		if(mainData == null) {
			mainData = Singleton.MainData;
		}
		if(searchForCa.ca != null) {
			foreach(var minePos in searchForCa.GetMineList()) {
				SingletonForEditor.DrawABeaultifulButton(minePos,searchForCa.stepResult == SearchingStepResult.GETRESULT ? Color.green : Color.red, null);
			}
			foreach(var number in searchForCa.GetNumberList()) {
				var point = searchForCa.GetNumberPoint(number);
				SingletonForEditor.DrawABeaultifulButton(number.pos, point == 0 ? Color.grey : Color.yellow, null);
				SingletonForEditor.DrawABeaultifulLabel(number.pos, point.ToString());
				if(point == 0) {
					foreach(var neighbour in number.neighbours) {
						SingletonForEditor.DrawABeautifulSmallButton(neighbour.pos, Color.grey, null);
					}
				}
			}
			var iteratingNumber = searchForCa.GetIteratingNumber();
			if(iteratingNumber != null) {
				var point = searchForCa.GetNumberPoint(iteratingNumber);
				SingletonForEditor.DrawABeaultifulButton(iteratingNumber.pos, Color.blue, null);
				SingletonForEditor.DrawABeaultifulLabel(iteratingNumber.pos, point.ToString());
				var iteratingOutside = searchForCa.GetIteratingOutside(); 
				if(iteratingOutside != null) {
					SingletonForEditor.DrawABeaultifulButton(iteratingOutside.pos, Color.blue, null);
				}
			}
		}
	}
}
