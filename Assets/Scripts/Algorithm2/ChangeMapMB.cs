using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class ChangeMapMB : MonoBehaviour{
    public Precalculate precalculate;
    public ChangeMap changeMap;
    
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        precalculate = GetComponent<Precalculate>();
        Singleton.GamePart.clickAction += AfterClick;
    }
    public void AfterClick(IndexOfList2D clickPos){
        if(precalculate.problemsAndResults != null && precalculate.tableBase != null){
            foreach(var search in precalculate.problemsAndResults.Values){
                if(!search.isDead){
                    return;
                }
            }
            changeMap = new ChangeMap(clickPos, precalculate.problemsAndResults, precalculate.tableBase);
            changeMap.Calculate();
            List<SetMap> setMaps = changeMap.result;
            visualizedData(setMaps);
            changeMap.ChangeMineData();
        }
    }
    public void visualizedData(List<SetMap> setMaps){
        List<IndexOfList2D> emptyToMine = new List<IndexOfList2D>();
        List<IndexOfList2D> mineToEmpty = new List<IndexOfList2D>();
        foreach(var setMap in setMaps){
            List2DInt mineData = Singleton.MainData.mineDatas;
            if(mineData[setMap.pos] == -1 && setMap.value != -1){
                mineToEmpty.Add(setMap.pos);
            }else if(mineData[setMap.pos] != -1 && setMap.value == -1){
                emptyToMine.Add(setMap.pos);
            }
        }
        GameObject gb = GameObject.Find("ChangeMapResult");
        if(gb == null){
            gb = new GameObject("ChangeMapResult");
        }
        ShowPositions sp = gb.GetComponent<ShowPositions>();
        if(sp == null){
            sp = gb.AddComponent<ShowPositions>();
        }
        sp.positionsList.Clear();
        sp.positionsList.Add(new ShowPositions.PositionProperty(emptyToMine, Color.green));
        sp.positionsList.Add(new ShowPositions.PositionProperty(mineToEmpty, Color.red));
        return;
    }
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        
    }
}