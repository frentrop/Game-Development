using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZombieController : MonoBehaviour {
	//zombie movement
	public float moveSpeed;
	public float turnSpeed;
	private Vector3 moveDirection;
	//colliders
//	[SerializeField]
//	private PolygonCollider2D[] colliders;
//	private int currentColliderIndex = 0;
	//invincibility
	private float timeSpentInvincible;
	public int health = 100;
	//TODO audio
	public AudioClip enemyContactSound;
	public AudioClip catContactSound;
	//angle of zombie to face player, vector pos of player
	private float zombieAngle;
	private Vector3 targetPos;
	//start and environment variables
	bool startup;
	private GameObject background;
	private float boundMaxX, boundMinX, boundMaxY, boundMinY;

	// Use this for initialization
	void Start () {
		//find gameobject for background
		background = GameObject.FindWithTag ("background");
		//find proper alignment of vector
		moveDirection = Vector3.right;
		//zombie speeds
		turnSpeed = 2f;
		moveSpeed = 0.75f;
		startup = true;
		//boundaries
		background.GetComponent<SpawnScript>().backgroundBounds(out boundMinX, 
		                                                        out boundMaxX, 
		                                                        out boundMinY, 
		                                                        out boundMaxY);
	}
	
	// Update is called once per frame
	void Update () {
		//get current zombie position
		Vector3 currentPos = transform.position;
		//find position of player (targetPos), 
		//done by getting it from SpawnScript (Better performance when zombies are spawned than GameObject.Find())
		background.GetComponent<SpawnScript>().playerPos(out targetPos);
		//find direction to target
		moveDirection = targetPos - currentPos;

		//check if zombie is in startup
		if(startup){
			//if still in startup, move towards center first
			moveDirection = new Vector3(0,0,0) - currentPos;
			//and check if zombie is in playable field
			if( (Mathf.Clamp(currentPos.x, boundMinX, boundMaxX) == currentPos.x) && 
			    (Mathf.Clamp(currentPos.y, boundMinY, boundMaxY) == currentPos.y) ){
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
	//set proper collider
//	public void SetColliderForSprite(int spriteNum){
//		colliders[currentColliderIndex].enabled = false;
//		currentColliderIndex = spriteNum;
//		colliders[currentColliderIndex].enabled = true;
//	}
	//zombie is hit, can be called from other scripts
	public void isHit(int damage){
		//do damage
		health -= damage;
		//still alive?
		if(health <= 0){
			//He's dead, Jim, add score, destroy zombie and let level check for other zombies
			GameObject.Find ("Player").GetComponent<PlayerController>().addScore(50);
			Destroy(gameObject);
			background.GetComponent<SpawnScript>().checkZombies();
		}else{
			//still alive. Well... Sort of...
			GameObject.Find("Player").GetComponent<PlayerController>().addScore(25);
		}
	}
	//improve zombies according to SpawnScript
	public void improveZombie(int extraHealth, float extraSpeed){
		health += extraHealth;
		moveSpeed += extraSpeed;
	}
}
