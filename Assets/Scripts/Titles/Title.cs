using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class Title : MonoBehaviour{
    TextMesh textMesh;
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
    }
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if(textMesh == null){
            textMesh = GetComponent<TextMesh>();
        }
        
        textMesh.text = "Minesweeper - " + (Singleton.MainData.mineCount - Singleton.FlagPart.FlagCount).ToString(); 
    }
}