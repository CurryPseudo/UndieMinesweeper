using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class MainCameraSize : MonoBehaviour {
	Camera camera;
	public void ChangeView() {
		if(camera == null) {
			camera = GetComponent<Camera>();
		}
		float cameraAspect = camera.aspect; 
		Vector3 size = Singleton.ViewRect.Size;
		float viewRectAspect = size.x / size.y;
		if(viewRectAspect > cameraAspect) {
			//Change view by width. 
			camera.orthographicSize = size.x / cameraAspect / 2;	
		}else{
			//Change view by height.
			camera.orthographicSize = size.y / 2;
		}
		Vector3 center = (Singleton.ViewRect.LeftDown + Singleton.ViewRect.RightUp) / 2;
		center.z = transform.position.z;
		transform.position = center;
	}
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		ChangeView();		
	}
}
