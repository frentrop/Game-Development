using UnityEngine;
using System.Collections;

public class GUIPopUp : MonoBehaviour {
	//GUIText object
	public GUIText popUp;
	//declare colors for pop up
	public Color colorStart;
	public Color colorEnd;
	//time values for pop up 
	public float beforeFade = 2.0f;
	public float duration = 4.0f;
	private float startTime = Time.time;

	// Use this for initialization
	void Start () {
		//color definitions
		colorStart = Color.white;
		colorEnd = Color.clear;
		//check if there are more pop ups, if so, change position so they don't overlap
		int length = GameObject.FindGameObjectsWithTag("powerUp").Length;
		if(length > 1){
			transform.position = new Vector3(0.5f, transform.position.y + 0.1f * (length - 1), 0);
		}
	}
	
	// Update is called once per frame
	void Update () {
		//time since pop up was instantiated
		float time = Time.time - startTime;
		//if pop up was instantiated longer than beforeFade amount of time, start fading out
		if(time >= beforeFade){
			//give value between the 2 values inserted in PingPong
			float lerp = Mathf.PingPong(time - beforeFade, duration) / duration;
			//change color between colorStart and colorEnd, according to lerp
			popUp.material.color = Color.Lerp(colorStart, colorEnd, lerp);
		}
		if(time >= beforeFade + duration){
			Destroy(gameObject);
		}
	}

	public void setText(string displayText){
		popUp.text = displayText;
	}
	
}
