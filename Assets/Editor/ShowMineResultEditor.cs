using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(ShowMineResult))]
public class ShowMineResultEditor : Editor {
    

    public void OnSceneGUI()
    {
        ShowMineResult data = target as ShowMineResult;
        Handles.color = Color.green;
        if(data.happyCheckStep != null && data.result != null){
            foreach(var index in data.result){
                var ufa = data.happyCheckStep.GetUfa(index);
                Handles.Button(Singleton.MainData.AreaPosWorld(ufa.pos.x, ufa.pos.y), Quaternion.identity, 0.5f, 0.5f, Handles.CylinderHandleCap);
            }
        }

    }
}
