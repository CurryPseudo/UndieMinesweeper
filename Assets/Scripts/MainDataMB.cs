using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
[ExecuteInEditMode]
public class MainDataMB : MonoBehaviour{
    public float step = 1.06f;
    public int nextX = 10;
    public int nextY = 10;
	public bool reGenerate = false;
	public bool resetMineData = false;
	public bool randomSetMineData = false;
    public int randomGenerateCount = 10;
    public MainData mainData = null;
    public event System.Action AfterReGenerateAction;
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
    }
    public void ReGenerate(){
        if(mainData != null){
            mainData.Destroy();
        }
        MainData last = mainData;
        mainData = new MainData(nextX, nextY, step, transform, this);
        mainData.CopyMainData(last);
        if(AfterReGenerateAction != null){
            AfterReGenerateAction();
        }
    }
	void Update () {
		if(reGenerate){
			reGenerate = false;
            ReGenerate();
		}
		if(resetMineData){
			resetMineData = false;
			mainData.ResetMineData();
		}
		
		if(randomSetMineData){
			randomSetMineData = false;
			mainData.RandomGenerate(randomGenerateCount);
		}
	}
}