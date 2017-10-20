using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(MainData))]
public class MainDataEditor : Editor {
    /// <summary>
    /// OnGUI is called for rendering and handling GUI events.
    /// This function can be called multiple times per frame (one call per event).
    /// </summary>
    void OnSceneGUI()
    {
        MainData mainData = target as MainData;
        Handles.color = Color.white;
        for(int i = 0; i < mainData.nowX; i++){
            for(int j = 0; j < mainData.nowY; j++){
                int mineData = mainData.GetMineData(i, j); 
                Handles.color = mineData == -1 ? Color.red : Color.white;
                if(Handles.Button(mainData.AreaPos(i, j), Quaternion.identity, 0.5f, 0.5f, Handles.RectangleHandleCap)){
                    mainData.ReverseMineData(i, j);
                }
                
                Handles.Label(mainData.AreaPos(i, j), mineData.ToString());
            }
        }
    }
}