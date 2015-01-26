using UnityEngine;
using System.Collections;

public class powerUpScript : MonoBehaviour {
	private bool justOnce = true;
	private GameObject player;
	public GUIText guiText;

	// Use this for initialization
	void Start () {
		player = GameObject.Find ("Player");
		guiText.pixelOffset = new Vector2 (300, 200);//new Vector2(transform.position.x,transform.position.y);

	}

	void OnGUI(){
		guiText.text = "Testing";
		guiText.pixelOffset = new Vector2 (300, 200);
	}

	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerExit2D(Collider2D coll){
		if (coll.CompareTag ("Player")) {
			//moet maar 1 x gebeuren
			if(justOnce){
				justOnce = false;
				//kies random of er damage bijkomt of fireRate omhoog gaat
				float i = Random.Range(0,3f);
				if(i <= 1f){
					//TODO add more damage to weapon player
					player.GetComponent<PlayerController>().changeDamage(10);
				}else if(i <= 2f){
					//firerate verlagen met 0,01f
					player.GetComponent<PlayerController>().changeFireRate(0.05f);
				}else {
					//extra health
					player.GetComponent<PlayerController>().addHealth(1);
				}
			gameObject.GetComponent<Animator>().SetBool("removePowerUp",true);
			} 
		}
	}

	public void PowerUpRemove(){
		Destroy (gameObject);
	}
}
