using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(ShowMineResult))]
public class ShowMineResultEditor : Editor {
    
    /// <summary>
    /// OnGUI is called for rendering and handling GUI events.
    /// This function can be called multiple times per frame (one call per event).
    /// </summary>
    public void OnSceneGUI()
    {
        ShowMineResult data = target as ShowMineResult;
        Handles.color = Color.red;
        if(data.happyCheckStep != null && data.result != null){
            foreach(var index in data.result){
                var ufa = data.happyCheckStep.GetUfa(index);
                Handles.Button(MainDataSingleton.value.AreaPosWorld(ufa.pos.x, ufa.pos.y), Quaternion.identity, 0.5f, 0.5f, Handles.CylinderHandleCap);
            }
        }

    }
}
