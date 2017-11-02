using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewRect : MonoBehaviour {

	public Vector3 Size{
		get{
			return RightUp - LeftDown;
		}
	}
	public Vector3 LeftDown{
		get{
			return Singleton.MainData.LeftDown;
		}
	}
	public Vector3 RightUp{
		get{
			return new Vector3(Singleton.MainData.RightUp.x, titleTransform.position.y, 0);
		}
	}
	public Transform titleTransform;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
				
	}
}
