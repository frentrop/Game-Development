using UnityEngine;
using System.Collections;

public class homeScreen : MonoBehaviour {
	//rectangle used for button
	Rect rect;
	float width, height;
	int fontSize = 200;
	// Use this for initialization
	void Start () {
		width = Screen.width;
		height = Screen.height;
		//size of button
		rect = new Rect(width/2 - width/6, height/2 - 75, width/3, 150);
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
	}
}
