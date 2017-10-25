using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public abstract class EditWithList2DIntEditor : Editor {
    public static MainData mainData;
    public abstract List2D<int> GetMainList2D();
    /// <summary>
    /// OnGUI is called for rendering and handling GUI events.
    /// This function can be called multiple times per frame (one call per event).
    /// </summary>
    public virtual void OnSceneGUI()
    {
        Handles.color = Color.white;
        List2D<int> list2D = GetMainList2D();
        if(mainData == null){
                mainData = GameObject.Find("MainData").GetComponent<MainData>();
        }
        if(list2D != null && list2D.XSize >= mainData.mineDatas.XSize && list2D.YSize >= mainData.mineDatas.YSize){
            for(int i = 0; i < mainData.nowX; i++){
                for(int j = 0; j < mainData.nowY; j++){
                    Handles.Label(mainData.AreaPosWorld(i, j), list2D[i, j].ToString());
                }
            }
        }
    }
}
