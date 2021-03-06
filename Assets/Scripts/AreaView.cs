using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AreaView : MonoBehaviour {
    public int x;
    public int y;
    float originScaleX = -1;
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
    public void SetSize(float size) {
        Debug.Assert(size <= 1 && size >= 0);
        if(originScaleX == -1) {
            originScaleX = transform.parent.localScale.x;
        }
        transform.parent.localScale = new Vector3(originScaleX * size, transform.localScale.y, transform.localScale.z);
    }
    public void DestroyAll() {
        Destroy(transform.parent.gameObject);
    }
    /// <summary>
    /// Called every frame while the mouse is over the GUIElement or Collider.
    /// </summary>
    void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(0)) {
            Singleton.GamePart.AreaClick(x, y);
        }
        if(Input.GetMouseButtonDown(1)) {
            if(Singleton.FlagPart != null) {
                Singleton.FlagPart.RclickPos(new IndexOfList2D(x, y));
            }
        }
        if(Input.GetMouseButtonDown(2)) {
            if(Singleton.FlagPart != null){
                Singleton.FlagPart.MidclickPos(new IndexOfList2D(x, y));
            }
        }
    }
   
}