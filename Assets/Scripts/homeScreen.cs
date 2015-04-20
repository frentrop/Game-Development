using UnityEngine;
using System.Collections;

public class homeScreen : MonoBehaviour {
	//rectangle used for button
	Rect rect;
	float width, height;
	int fontSize = 200;
	public Texture2D sound, soundMuted;
	bool mute; 
	// Use this for initialization
	void Start () {
		width = Screen.width;
		height = Screen.height;
		//size of button
		rect = new Rect(width/2 - width/6, height/2 - 75, width/3, 150);
		//Check if there are playerprefs for audio and load them.
		if(!PlayerPrefs.HasKey("Mute") || PlayerPrefs.GetInt("Mute") == 0){
			mute = false;
			//create playerpref for Mute
			PlayerPrefs.SetInt("Mute",0);
			//save prefs on device
			PlayerPrefs.Save();
			AudioListener.volume = 100;
		}else if(PlayerPrefs.GetInt("Mute") == 1){
			//if muted
			mute = true;
			AudioListener.volume = 0;
		}
	}
	//draw GUI components 
	void OnGUI(){
		//adding style to button
		GUIStyle buttonStyle = new GUIStyle("button");
		//center both horizontally and vertically
		buttonStyle.alignment = TextAnchor.MiddleCenter;
		//fontsize depends on screen size
		buttonStyle.fontSize = Mathf.Min(Mathf.FloorToInt(width/3 * fontSize/1000), 
		                      Mathf.FloorToInt(height/3 * fontSize/1000));
		//play button
		if(GUI.Button(rect,"Play!", buttonStyle)){
			//if pressed, load main scene
			Application.LoadLevel("main scene");
		}
		//mute button
		if(!mute){
			if(GUI.Button(new Rect(10, 10, 75, 75), sound, buttonStyle)){
				AudioListener.volume = 0;
				mute = true;
				PlayerPrefs.SetInt("Mute",1);
				PlayerPrefs.Save ();
			}
		}else if(mute){
			if(GUI.Button (new Rect(10,10,75,75), soundMuted, buttonStyle)){
				AudioListener.volume = 100;
				mute = false;
				PlayerPrefs.SetInt("Mute",0);
				PlayerPrefs.Save ();
			}
		}
		//TODO display top score
		if(PlayerPrefs.HasKey("HighScore")){
			//Display score in new rect
			GUI.Label( new Rect (Screen.width*3/5, 0, 500, 64), 
			          "<size=60><b>Highscore: " + PlayerPrefs.GetInt("HighScore").ToString() + "</b></size>");
		}
	}
}
