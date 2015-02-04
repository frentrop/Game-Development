using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZombieController : MonoBehaviour {

	public float moveSpeed;
	public float turnSpeed;

	private Vector3 moveDirection;

	[SerializeField]
	private PolygonCollider2D[] colliders;
	private int currentColliderIndex = 0;

	private float timeSpentInvincible;

	public int health = 100;

	public AudioClip enemyContactSound;
	public AudioClip catContactSound;

	//player, angle of zombie to face player, vector pos of player
	public GameObject player;
	private float zombieAngle;
	private Vector3 targetPos;

	bool startup;
	private GameObject background;
	private float boundMaxX, boundMinX, boundMaxY, boundMinY;

	// Use this for initialization
	void Start () {
		//find gameobject for player
		player = GameObject.Find ("Player");
		//find gameobject for background
		background = GameObject.Find ("background");
		moveDirection = Vector3.right;
		turnSpeed = 2f;
		moveSpeed = 0.75f;
		startup = true;
		boundMaxX = background.renderer.bounds.max.x;
		boundMinX = background.renderer.bounds.min.x;
		boundMaxY = background.renderer.bounds.max.y;
		boundMinY = background.renderer.bounds.min.y;
	}
	
	// Update is called once per frame
	void Update () {

		//get current zombie position
		Vector3 currentPos = transform.position;
		//find position of player (target)
		targetPos = player.transform.position;
		//find direction to target
		moveDirection = targetPos - currentPos;

		//check if zombie is in startup
		if(startup){
			//if still in startup, move towards center first
			moveDirection = new Vector3(0,0,0) - currentPos;
			//and check if zombie is in playable field
			if( (Mathf.Clamp(currentPos.x, boundMinX, boundMaxX) == currentPos.x) && (Mathf.Clamp(currentPos.y, boundMinY, boundMaxY) == currentPos.y) ){
				//if zombie is in playable field, it is no longer in startup mode and should go find BRAAAIINS! 
				startup = false;
			}
		}

		//z axis is zero ofc
		moveDirection.z = 0;
		//normalize vector (return vector with magnitude of 1)
		moveDirection.Normalize ();

		//find out where to move to (with predefined speed)
		Vector3 movePos = moveDirection * moveSpeed + currentPos;
		//actual movement
		transform.position = Vector3.Lerp (currentPos, movePos, Time.deltaTime);

		//find angle in order to face target
		float zombieAngle = Mathf.Atan2 (moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
		//rotate zombie to face target
		transform.rotation = Quaternion.Slerp (transform.rotation, 
		                                      Quaternion.Euler (0, 0, zombieAngle),
		                                      turnSpeed * Time.deltaTime);


	}

	public void SetColliderForSprite(int spriteNum){
		colliders[currentColliderIndex].enabled = false;
		currentColliderIndex = spriteNum;
		colliders[currentColliderIndex].enabled = true;
	}

	void OnTriggerEnter2D(Collider2D other){

	}

	public void isHit(int damage){
		//Debug.Log("hit!");
		health -= damage;

		if(health <= 0){
			//Debug.Log("Die zombie, die!");
			GameObject.Find ("Player").GetComponent<PlayerController>().addScore(50);
			Destroy(gameObject);
			GameObject.Find ("background").GetComponent<SpawnScript>().checkZombies();
		}else{
			GameObject.Find("Player").GetComponent<PlayerController>().addScore(25);
		}
	}

	public void healt(int extraHealth){
		health += extraHealth;
	}

}
