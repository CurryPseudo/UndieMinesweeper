using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AreaView : MonoBehaviour {
    public int x;
    public int y;
    float originScaleX = -1;
    public GamePart gamePart = null;
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
    }
    public void SetSize(float size){
        Debug.Assert(size <= 1 && size >= 0);
        if(originScaleX == -1){
            originScaleX = transform.parent.localScale.x;
        }
        transform.parent.localScale = new Vector3(originScaleX * size, transform.localScale.y, transform.localScale.z);
    }
    public void DestroyAll(){
        Destroy(transform.parent.gameObject);
    }
    /// <summary>
    /// OnMouseDown is called when the user has pressed the mouse button while
    /// over the GUIElement or Collider.
    /// </summary>
    void OnMouseDown()
    {
        if(gamePart != null){
            gamePart.AreaClick(x, y);
        }
    }
}