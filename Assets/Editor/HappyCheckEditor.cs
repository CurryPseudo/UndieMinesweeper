using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(HappyCheck))]
public class HappyCheckEditor : Editor {
    public static MainData mainData;
    public HCArea now = null;
    /// <summary>
    /// OnGUI is called for rendering and handling GUI events.
    /// This function can be called multiple times per frame (one call per event).
    /// </summary>
    public void OnSceneGUI()
    {
        HappyCheck happyCheck = target as HappyCheck;
        Handles.color = Color.white;
        if(mainData == null){
            mainData = GameObject.Find("MainData").GetComponent<MainData>();
        }
        if(happyCheck.map == null) return;
        if(now == null){
            SmartList<HCArea>[] hack = {happyCheck.unFlipAreaList, happyCheck.numberList};
            for(int i = 0; i < 2; i++){
                foreach(HCArea ufa in hack[i]){
                    Handles.color = ufa.value >= 0 ? Color.red : Color.yellow;
                    if(Handles.Button(mainData.AreaPosWorld(ufa.pos.x, ufa.pos.y), Quaternion.identity, 0.5f, 0.5f, Handles.CylinderHandleCap)){
                        now = ufa;
                    }   
                }   
            }
        }else{
            foreach(HCArea neighbor in now.neighbours){
                Handles.color = neighbor.value >= 0 ? Color.red : Color.yellow;
                Handles.Label(mainData.AreaPosWorld(neighbor.pos.x, neighbor.pos.y), neighbor.value.ToString());
                if(Handles.Button(mainData.AreaPosWorld(neighbor.pos.x, neighbor.pos.y), Quaternion.identity, 0.5f, 0.5f, Handles.CylinderHandleCap)){
                    now = neighbor;
                }   
            }
            Handles.color = now.value >= 0 ? Color.red : Color.yellow;
            if(Handles.Button(mainData.AreaPosWorld(now.pos.x, now.pos.y), Quaternion.identity, 0.5f, 0.5f, Handles.CylinderHandleCap)){
                now = null;
            }   
            
        }
        
        
    }
}
