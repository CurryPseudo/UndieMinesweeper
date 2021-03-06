using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaHover : MonoBehaviour {
    SpriteRenderer renderer;
    Color originColor;
    public Color hoverColor;
	void Start () {
        renderer = GetComponent<SpriteRenderer>();
        originColor = renderer.color;
	}
	void Update () {
		
	}
    /// <summary>
    /// Called every frame while the mouse is over the GUIElement or Collider.
    /// </summary>
    void OnMouseOver()
    {
        renderer.color = hoverColor;
    }
    /// <summary>
    /// Called when the mouse is not any longer over the GUIElement or Collider.
    /// </summary>
    void OnMouseExit()
    {
        renderer.color = originColor;
    }
}

