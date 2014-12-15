﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	
	public Joystick moveJoystick;           // Reference to joystick prefab
	public Joystick fireJoystick;
	public float speed = 12;             // Movement speed
	public bool useAxisInput = true;   // Use Input Axis or Joystick
	private float h = 0, v = 0;         // Horizontal and Vertical values
	private float fH = 0, fV = 0;		//horizontal and vertical values of fire joystick
	private float playerAngle = 0;			//angle of player

	//used to change player collider
	[SerializeField]
	private PolygonCollider2D[] colliders;
	private int currentColliderIndex = 0;

	//parameter for animation
	private bool moving;

	//game objects
	public GameObject Bullet, Gun, background;

	//fire rate parameters
	public float fireRate = 0.2f;
	private float nextFire = 0.0f;
	public float bulletSpeed = 6.0f;

	public int damage = 34;

	private int health = 5;
	public Texture2D heart;

	public int score = 0;

	private bool isInvincible = false;
	private float timeSpentInvincible;
	private float boundMinX, boundMaxX, boundMinY, boundMaxY;

	int fontSize = 200;

	void Start(){
		boundMinX = background.renderer.bounds.min.x;
		boundMaxX = background.renderer.bounds.max.x;
		boundMinY = background.renderer.bounds.min.y;
		boundMaxY = background.renderer.bounds.max.y;
		rigidbody2D.drag = 50;
	}

	void OnGUI(){
		Rect r = new Rect(0,0, 350, 96);
		GUILayout.BeginArea(r);
		GUILayout.BeginHorizontal();

		for(int i = 0; i < health; i++)
			GUILayout.Label(heart); //assign your heart image to this texture
		
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();

		GUI.Label( new Rect (Screen.width - 100, 0, 100, 64), "<size=40><b>" + score.ToString() + "</b></size>");
		//if health is below 0, display game over
		if(health <= 0){
			float w = 0.7f;
			float h = 0.5f;
			Rect gameOverRect = new Rect();
			gameOverRect.x = (Screen.width*(1-w))/2;
			gameOverRect.y = (Screen.height*(1-h))/2;
			gameOverRect.width = Screen.width*w;
			gameOverRect.height = Screen.height*h;
			GUIStyle gameOverTextStyle = new GUIStyle("label");
			gameOverTextStyle.alignment = TextAnchor.MiddleCenter;
			gameOverTextStyle.fontSize =  Mathf.Min(Mathf.FloorToInt(Screen.width * fontSize/1000), 
			                                        Mathf.FloorToInt(Screen.height * fontSize/1000));
			GUI.Label( gameOverRect, "<b><color=red>Game over!</color></b>", gameOverTextStyle);
		}
	}
	
	// Update is called once per frame
	void Update () {
		// Get horizontal and vertical input values from either axis or the joystick.
		if (!useAxisInput) {
			h = moveJoystick.position.x;
			v = moveJoystick.position.y;
			fH = fireJoystick.position.x; 	//fire joystick horizontal pos
			fV = fireJoystick.position.y;	//fire joystick vertical pos

			//check if fire joystick is in use or not 
			if(Mathf.Abs(fH) > 0.01 && Mathf.Abs(fV) > 0.01){
				//if firejoystick is in use, set playerAngle to match fire joystick
				playerAngle = Mathf.Atan2(fV, fH) * Mathf.Rad2Deg - 90;

				//fire bullet
				fireBullet();

			}else if( Mathf.Abs(h) > 0.01 && Mathf.Abs(v) > 0.01){
				//if fire joystick is not in use, set playerAngle to match move joystick
				playerAngle = Mathf.Atan2(v, h) * Mathf.Rad2Deg -90;
			}
		}
		else {
			h = Input.GetAxis("Horizontal");
			v = Input.GetAxis("Vertical");
		}

		//set animation parameters for player animations
		if(Mathf.Abs(h) > 0.01 || Mathf.Abs(v) > 0.01){
			gameObject.GetComponent<Animator>().SetBool("moving", true);
		}else {
			gameObject.GetComponent<Animator>().SetBool("moving", false);
		}
		
		// Apply horizontal velocity
		if (Mathf.Abs(h) > 0) {
			//check if player is at edges of background
			if(transform.position.x > boundMaxX){
				//if so, make sure the player cannot cross the boundaries
				h = Mathf.Clamp(h, -1, 0);
			}else if(transform.position.x < boundMinX){
				h = Mathf.Clamp(h, 0, 1);
			}
			rigidbody2D.velocity = new Vector2(h * speed, rigidbody2D.velocity.y);
		}
		
		// Apply vertical velocity
		if (Mathf.Abs(v) > 0) {
			//check if player is at edges of background
			if(transform.position.y > boundMaxY){
				//if so, make sure the player cannot cross the boundaries
				v = Mathf.Clamp(v, -1, 0);
			}else if(transform.position.y < boundMinY){
				v = Mathf.Clamp(v, 0, 1);
			}
			rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, v * speed);
		}
		rigidbody2D.MoveRotation(playerAngle);

		//if invincible, let renderer blink
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

	//set correct sprite collider
	public void SetColliderForSprite(int spriteNum){
		colliders[currentColliderIndex].enabled = false;
		currentColliderIndex = spriteNum;
		colliders[currentColliderIndex].enabled = true;
	}

	//fire bullets 
	private void fireBullet(){
		//check if enough time has passed to fire a bullet
		if(Time.time > nextFire){
			//set time when next bullet can be fired
			nextFire = Time.time + fireRate;
			//create new clone of bullet with proper position and rotation
			GameObject currentBullet = (GameObject)Instantiate(Bullet, 
			                                                   new Vector2(Gun.rigidbody2D.position.x, Gun.rigidbody2D.position.y), 
			                                                   Quaternion.identity);
			//set damage of bullet
			currentBullet.GetComponent<BulletScript>().setDamage(damage);
			//find direction the bullet is fired
			Vector3 angle = Gun.transform.eulerAngles;
			currentBullet.transform.eulerAngles = angle;
			Vector3 forceVector = currentBullet.transform.right;
			//add force to the bullet to actually fire it away
			currentBullet.rigidbody2D.AddForce(forceVector * bulletSpeed * 1.5f, ForceMode2D.Impulse);
		}
	}

	//set damage of bullet, used by power-ups to increase damage
	public void setBulletDamage(int damage){
		this.damage = damage;
	}

	//if trigger collision is... triggered ;)
	void OnTriggerEnter2D(Collider2D coll){ //was OnTriggerEnter2D
		//if collided with zombie and player is not invincible
		if(coll.CompareTag("zombie") && !isInvincible){
			//make invincible
			isInvincible = true;
			//control variable for invincibility 
			timeSpentInvincible = 0;
			//decrement health and check if player still has lives
			if(--health <= 0){
				//if not, game over!
				Debug.Log("Game Over");
				print (health);
				//reload level after 3 secs
				Invoke("ReloadLevel", 3f);
			}
		}
	}

	void ReloadLevel(){
		Application.LoadLevel(Application.loadedLevel);
	}

	public void addScore(int points){
		score += points;
	}

}