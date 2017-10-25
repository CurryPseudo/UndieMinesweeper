using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(HappyCheckStep))]
public class HappyCheckStepEditor : Editor {
    public static MainData mainData;
    public HappyCheckStep happyCheckStep;
    public HCArea now = null;
    /// <summary>
    /// OnGUI is called for rendering and handling GUI events.
    /// This function can be called multiple times per frame (one call per event).
    /// </summary>
    public void OnSceneGUI()
    {
        happyCheckStep = target as HappyCheckStep;
        Handles.color = Color.white;
        if(mainData == null){
            mainData = GameObject.Find("MainData").GetComponent<MainData>();
        }
        if(happyCheckStep.ufaMap == null) return;
        if(happyCheckStep.sortIndex < happyCheckStep.ufaIndexes.Length){
            HCArea ufa = getUfa(happyCheckStep.sortIndex);
            Handles.color = Color.yellow;
            Handles.Button(mainData.AreaPosWorld(ufa.pos.x, ufa.pos.y), Quaternion.identity, 0.5f, 0.5f, Handles.CylinderHandleCap);
        }
        foreach(var index in happyCheckStep.stack){
            HCArea ufa = getUfa(index);
            Handles.color = happyCheckStep.numberCount == 0 ? Color.green : Color.red;
            Handles.Button(mainData.AreaPosWorld(ufa.pos.x, ufa.pos.y), Quaternion.identity, 0.5f, 0.5f, Handles.CylinderHandleCap);
        }
        for(int i = 0; i < happyCheckStep.ufaIndexes.Length; i++){
            if(happyCheckStep.ufaMap[getUfa(i).pos] == 1){
                HCArea ufa = getUfa(i);
                Handles.color = Color.grey;
                Handles.Button(mainData.AreaPosWorld(ufa.pos.x, ufa.pos.y), Quaternion.identity, 0.5f, 0.5f, Handles.ArrowHandleCap);
            }
        }
        
    }
    HCArea getUfa(int index){
        return happyCheckStep.happyCheck.unFlipAreaList[happyCheckStep.ufaIndexes[index]];
    }
}
