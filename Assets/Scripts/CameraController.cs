using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	//target to follow with camera, background to find edges of background
	public GameObject target, background;
	//find viewable area and values of boundaries
	private float viewportX, viewportY, targetX, targetY, boundMinX, boundMaxX, boundMinY, boundMaxY;
	
	// Use this for initialization
	void Start () {
		//get first values here, to avoid loop
		viewportX = Camera.main.orthographicSize * Camera.main.aspect;
		viewportY = Camera.main.orthographicSize;
		boundMinX = background.renderer.bounds.min.x + viewportX;
		boundMaxX = background.renderer.bounds.max.x - viewportX;
		boundMinY = background.renderer.bounds.min.y + viewportY;
		boundMaxY = background.renderer.bounds.max.y - viewportY;
		transform.position = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
		//limit x positions of camera
		targetX = Mathf.Clamp(target.transform.position.x, boundMinX, boundMaxX);
		//limit y positions of camera
		targetY = Mathf.Clamp(target.transform.position.y, boundMinY, boundMaxY);

		//print(Camera.main.orthographicSize);
		//print(Camera.main.orthographicSize * Camera.main.aspect);
		transform.position = new Vector3(targetX, targetY, transform.position.z);
		//print(background.renderer.bounds.min.x);
	}
}