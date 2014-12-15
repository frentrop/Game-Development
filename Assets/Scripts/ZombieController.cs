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

	private bool isInvincible = false;
	private float timeSpentInvincible;

	//private int lives = 3;
	public int health = 100;

	public AudioClip enemyContactSound;
	public AudioClip catContactSound;

	// Use this for initialization
	void Start () {
		moveDirection = Vector3.right;
	}
	
	// Update is called once per frame
	void Update () {

		// 1
		Vector3 currentPosition = transform.position;
		// 2
		if( Input.GetButton("Fire1") ) {
			// 3
			Vector3 moveToward = Camera.main.ScreenToWorldPoint( Input.mousePosition );
			// 4
			moveDirection = moveToward - currentPosition;
			moveDirection.z = 0; 
			moveDirection.Normalize();
		}

		Vector3 target = moveDirection * moveSpeed + currentPosition;
		transform.position = Vector3.Lerp( currentPosition, target, Time.deltaTime );

		float targetAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
		transform.rotation = 
			Quaternion.Slerp( transform.rotation, 
			                 Quaternion.Euler( 0, 0, targetAngle ), 
			                 turnSpeed * Time.deltaTime );

		//enforce the bounds of the screen
		EnforceBounds();

		if(isInvincible){
			timeSpentInvincible += Time.deltaTime;
			if(timeSpentInvincible < 3f){
				float remainder = timeSpentInvincible % .3f;
				renderer.enabled = remainder > .15f;
			}else {
				renderer.enabled = true;
				isInvincible = false;
			}
		}
	}

	public void SetColliderForSprite(int spriteNum){
		colliders[currentColliderIndex].enabled = false;
		currentColliderIndex = spriteNum;
		colliders[currentColliderIndex].enabled = true;
	}

	void OnTriggerEnter2D(Collider2D other){

//		if(other.CompareTag("Bullet")){
//			health -= other.GetComponent<BulletScript>().damage;
//			Debug.Log("hit");
//		}
//
////		if(other.CompareTag("cat")){
////			audio.PlayOneShot(catContactSound);
////
////		}else if (!isInvincible && other.CompareTag("enemy")){
////			isInvincible = true;
////			audio.PlayOneShot(enemyContactSound);
////			timeSpentInvincible = 0;
////			
////		}
//		if(lives > 5){
//		}
//		if(health <= 0){
//			Debug.Log("Dead zombie!");
//			Destroy(gameObject);
//		}
	}

	private void EnforceBounds(){
		//starting values 
		Vector3 newPosition = transform.position;
		Camera mainCamera = Camera.main;
		Vector3 cameraPosition = mainCamera.transform.position;

		//values needed for x bounds
		float xDist = mainCamera.aspect * mainCamera.orthographicSize;
		float xMax = cameraPosition.x + xDist;
		float xMin = cameraPosition.x - xDist;

		//values needed for y bounds
		float yMax = mainCamera.orthographicSize;

		//xbounds handling
		if(newPosition.x < xMin || newPosition.x > xMax){
			newPosition.x = Mathf.Clamp(newPosition.x, xMin, xMax);
			moveDirection.x = -moveDirection.x;
		}
		//ybounds handling
		if(newPosition.y < -yMax || newPosition.y > yMax){
			newPosition.y = Mathf.Clamp(newPosition.y, -yMax, yMax);
			moveDirection.y = -moveDirection.y;
		}
		//transforming to new position
		transform.position = newPosition;
	}

	public void isHit(int damage){
		Debug.Log("hit!");
		health -= damage;

		if(health <= 0){
			Debug.Log("Die zombie, die!");
			GameObject.Find ("Player").GetComponent<PlayerController>().addScore(50);
			Destroy(gameObject);
		}else{
			GameObject.Find("Player").GetComponent<PlayerController>().addScore(25);
		}
	}

}
