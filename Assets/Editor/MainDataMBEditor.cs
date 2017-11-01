using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(MainDataMB))]
public class MainDataMBEditor : Editor {

    /// <summary>
    /// OnGUI is called for rendering and handling GUI events.
    /// This function can be called multiple times per frame (one call per event).
    /// </summary>
    public void OnSceneGUI()
    {
        MainData mainData = (target as MainDataMB).mainData;
        Handles.color = Color.white;
        for(int i = 0; i < mainData.XSize; i++){
            for(int j = 0; j < mainData.YSize; j++){
                int mineData = mainData.GetMineData(i, j); 
                Handles.color = mineData == -1 ? Color.red : Color.white;
                if(Handles.Button(mainData.AreaPosWorld(i, j), Quaternion.identity, 0.5f, 0.5f, Handles.RectangleHandleCap)){
                    mainData.ReverseMineData(i, j);
                }
            }
        }
        foreach(var pos in mainData.mineDatas.Positions()){
            Singleton.DrawABeaultifulLabel(pos, mainData.mineDatas[pos].ToString());
        }

    }
}
