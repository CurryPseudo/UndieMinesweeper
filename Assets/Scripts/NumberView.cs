using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberView : MonoBehaviour {
	TextMesh textMesh;
	AreaView areaView;
	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake()
	{
		textMesh = GetComponentInChildren<TextMesh>();
		areaView = GetComponentInChildren<AreaView>();
	}
	// Use this for initialization
	void Start () {
		}
	
	// Update is called once per frame
	void Update () {
		if(areaView.gamePart != null){
			int value = areaView.gamePart.mineDatas[areaView.x, areaView.y];
			textMesh.text = value == 0 ? "" : value.ToString();
		}
	}
}
