using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(ShowPositions))]
public class ShowPositionsEditor : Editor {
	public ShowPositions showPositions = null;
	void OnSceneGUI(){
		if(showPositions == null){
			showPositions = target as ShowPositions;
		}
		if(showPositions.positions != null){
			foreach(var pos in showPositions.positions){
				Singleton.DrawABeaultifulButton(pos, Color.green, null);
			}
		}
		
	}
}
