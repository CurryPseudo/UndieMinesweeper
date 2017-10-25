using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(MainData))]
public class MainDataEditor : EditWithList2DIntEditor {
    public override List2D<int> GetMainList2D()
    {
        return (target as MainData).mineDatas;
    }

    /// <summary>
    /// OnGUI is called for rendering and handling GUI events.
    /// This function can be called multiple times per frame (one call per event).
    /// </summary>
    public override void OnSceneGUI()
    {
        base.OnSceneGUI();
        MainData mainData = target as MainData;
        Handles.color = Color.white;
        for(int i = 0; i < mainData.nowX; i++){
            for(int j = 0; j < mainData.nowY; j++){
                int mineData = mainData.GetMineData(i, j); 
                Handles.color = mineData == -1 ? Color.red : Color.white;
                if(Handles.Button(mainData.AreaPosWorld(i, j), Quaternion.identity, 0.5f, 0.5f, Handles.RectangleHandleCap)){
                    mainData.ReverseMineData(i, j);
                }
            }
        }

    }
}
