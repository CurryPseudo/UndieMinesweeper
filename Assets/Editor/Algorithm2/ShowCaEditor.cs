using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(ShowCa))]
public class ShowCaEditor : Editor {
	public ShowCa showCas = null;
	public MainData mainData = null;
	public int areaType = 0;
	public Area now = null;
	void OnSceneGUI(){
		if(showCas == null){
			showCas = target as ShowCa;
		}
		if(mainData == null){
			mainData = Singleton.MainData;
		}
		if(showCas.ca != null){
			if(now == null){
				Handles.color = Color.yellow;
				foreach(Area area in showCas.ca.numbers){
					if(Handles.Button(mainData.AreaPosWorld(area.pos), Quaternion.identity, 0.5f, 0.5f, Handles.CylinderHandleCap)){
						now = area;
						areaType = 0;
					}
				}
				Handles.color = Color.red;
				foreach(Area area in showCas.ca.outsides){
					if(Handles.Button(mainData.AreaPosWorld(area.pos), Quaternion.identity, 0.5f, 0.5f, Handles.CylinderHandleCap)){
						now = area;
						areaType = 1;
					}
				}
			}else{
				Color[] colors = {Color.yellow, Color.red};
				Handles.color = colors[1 - areaType];
				foreach(var neighbour in now.neighbours){
					if(Handles.Button(mainData.AreaPosWorld(neighbour.pos), Quaternion.identity, 0.5f, 0.5f, Handles.CylinderHandleCap)){
						now = neighbour;
						areaType = 1 - areaType;
					}
				}
				Handles.color = colors[areaType];
				if(Handles.Button(mainData.AreaPosWorld(now.pos), Quaternion.identity, 0.5f, 0.5f, Handles.CylinderHandleCap)){
					now = null;
				}
			}
		}
	}
}
