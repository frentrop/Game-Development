using UnityEngine;
using System.Collections;

public class powerUpScript : MonoBehaviour {
	//control bool so action is done only once
	private bool justOnce = true;
	private GameObject player;
	//max time of power up on map
	private float maxTime = 20f;
	private float startTime;

	//declare game object for pop up 
	public GameObject popUp;
	//declare background and boundaries
	private GameObject background;
	private float boundMaxX, boundMinX, boundMaxY, boundMinY;

	// Use this for initialization
	void Start () {
		//get the time when the object was initiated
		startTime = Time.time;
		//find player, to better use in script later
		player = GameObject.Find ("Player");
		//find background to specify the boundaries in which the power up must remain
		background = GameObject.Find ("background");
		background.GetComponent<SpawnScript>().backgroundBounds(out boundMinX, 
		                                                        out boundMaxX, 
		                                                        out boundMinY, 
		                                                        out boundMaxY);
		//give power up random position on map
		transform.position = new Vector3(Random.Range(boundMinX + 2.5f,boundMaxX - 2.4f),
		                                 Random.Range (boundMinY + 2.5f,boundMaxY - 2.65f));
	}

	// Update is called once per frame
	void Update () {
		//destroy power up if it is not picked up in time
		if(Time.time - startTime > maxTime){
			Destroy(gameObject);
		}
	}

	void OnTriggerExit2D(Collider2D coll){
		if (coll.CompareTag ("Player")) {
			//moet maar 1 x gebeuren
			if(justOnce){
				justOnce = false;
				//kies random of er damage bijkomt of fireRate omhoog gaat
				float i = Random.Range(0,3f);
				if(i <= 1f){
					//add more damage to weapon player
					player.GetComponent<PlayerController>().changeDamage(10);
					//pop up with what power up it was
					GameObject PopUp = (GameObject)Instantiate(popUp);
					PopUp.GetComponent<GUIPopUp>().setText("Weapon will do extra damage!");
				}else if(i <= 2f){
					//lower the firerate
					player.GetComponent<PlayerController>().changeFireRate(0.05f);
					GameObject PopUp = (GameObject)Instantiate(popUp);
					PopUp.GetComponent<GUIPopUp>().setText("Weapon will fire quicker!");
				}else {
					//extra health
					player.GetComponent<PlayerController>().addHealth(1);
					GameObject PopUp = (GameObject)Instantiate(popUp);
					PopUp.GetComponent<GUIPopUp>().setText("Extra life!");
				}
			//if picked up by player, initialize animation for removal
			gameObject.GetComponent<Animator>().SetBool("removePowerUp",true);
			} 
		}
	}

	//remove the gameobject
	public void PowerUpRemove(){
		Destroy (gameObject);
	}
}
