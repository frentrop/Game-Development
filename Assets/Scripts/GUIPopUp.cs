using UnityEngine;
using System.Collections;

public class GUIPopUp : MonoBehaviour {

	public GUIText popUp;
	public Color colorStart;
	public Color colorEnd;
	public float beforeFade = 2.5f;
	public float duration = 4.0f;
	private float startTime = Time.time;

	// Use this for initialization
	void Start () {
		colorStart = Color.white;
		colorEnd = Color.clear;
		//Fade();
	}
	
	// Update is called once per frame
	void Update () {
		float time = Time.time;
		if(time >= startTime + beforeFade){
		float lerp = Mathf.PingPong(time + beforeFade, duration) / duration;
		popUp.material.color = Color.Lerp(colorStart, colorEnd, lerp);
		}
		if(time >= startTime + beforeFade + duration){
			Destroy(gameObject);
		}
	}

	public void setText(string displayText){
		popUp.text = displayText;
	}

//	void Fade(){
//		while( popUp.material.color.a > 0f){
//			popUp.material.color.a -= 0.1f * Time.deltaTime * 2f;
//		}
//	}
}
