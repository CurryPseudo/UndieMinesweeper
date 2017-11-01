using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TittleButtonChangeView : MonoBehaviour {

	public Precalculate precalculate;
	public Sprite Insidious;
	public Sprite Smile;
	public Sprite Happy;
	public Sprite Embarrassed;
	public SpriteRenderer spriteRenderer;
	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
		precalculate = GameObject.Find("Algorithm2").GetComponent<Precalculate>(); 
		precalculate.afterPrecacAction += RefreshView;
	}
	
	public void RefreshView(){
		if(Singleton.GamePart.GameValid == false){
			if(Singleton.GamePart.flipMine){
				spriteRenderer.sprite = Embarrassed;
			}else{
				if(Singleton.GamePart.UnFlipedAreaCount == Singleton.MainData.mineCount){
					spriteRenderer.sprite = Happy;
				}
			}
	}else{
			bool isDead = true;
			foreach(var search in precalculate.problemsAndResults.Values){
				if(!search.isDead){
					isDead = false;
					break;
				}
			}
			if(isDead){
				spriteRenderer.sprite = Insidious; 
			}else{
				spriteRenderer.sprite = Smile;
			}
		}
	}
	// Update is called once per frame
	void Update () {
	}
}
