using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class Title : MonoBehaviour{
    MainData mainData;
    TextMesh textMesh;
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        textMesh = GetComponent<TextMesh>();
        mainData = GameObject.Find("MainData").GetComponent<MainData>();
    }
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        textMesh.text = "Minesweeper - " + mainData.mineCount.ToString(); 
    }
}