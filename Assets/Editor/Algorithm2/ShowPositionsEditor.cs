using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(ShowPositions))]
public class ShowPositionsEditor : Editor {
	public ShowPositions showPositions = null;
	void OnSceneGUI() {
		if(showPositions == null) {
			showPositions = target as ShowPositions;
		}
		foreach(var positions in showPositions.positionsList) {
			foreach(var pos in positions.positions) {
				SingletonForEditor.DrawABeaultifulButton(pos, positions.color, null);
			}

		}
	}
}
