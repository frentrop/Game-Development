using UnityEngine;
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
	public float fireRate = 0.5f;
	private float nextFire = 0.0f;
	public float bulletSpeed = 12.0f;
	//damage of the gun
	public int damage = 34;
	//health of player + texture of hearts
	private int health = 5;
	public Texture2D heart;
	//score of the player
	public int score = 0;
	//variables related to invincibility (when player is hit)
	private bool isInvincible = false;
	private float timeSpentInvincible;
	//boundaries of map
	private float boundMinX, boundMaxX, boundMinY, boundMaxY;

	//used for pause menu
	private bool pause = false;
	public float width;// = Screen.width;
	public float height;// = Screen.height;
	public Rect windowRect;// = new Rect(width/3, height/3, width/3, height/3);
	//fontsize of GUI objects
	int fontSize = 200;

	void Start(){
		//sets speed of in-game time to 1
		Time.timeScale = 1;
		//set map boundaries
		background.GetComponent<SpawnScript>().backgroundBounds(out boundMinX, 
		                                                        out boundMaxX, 
		                                                        out boundMinY, 
		                                                        out boundMaxY);
		rigidbody2D.drag = 50;
		width = Screen.width;
		height = Screen.height;
		windowRect = new Rect(width/4, height/4, width/2, height/2);
	}
	//GUI drawing
	void OnGUI(){
		//display health
		Rect r = new Rect(0,0, Screen.width, 96);
		GUILayout.BeginArea(r);
		GUILayout.BeginHorizontal();
		//display hearts depending on amount of health
		for(int i = 0; i < health; i++)
			GUILayout.Label(heart); //assign your heart image to this texture
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
		//Display score in new rect
		GUI.Label( new Rect (Screen.width - 250, 0, 250, 64), "<size=60><b>" + score.ToString() + "</b></size>");
		//if health is below 0, display game over
		if(health <= 0){
			//percentage of screen used for this rect
			float w = 0.7f;
			float h = 0.5f;
			Rect gameOverRect = new Rect();
			//This way of picking rect values is done to scale it according to screen size
			gameOverRect.x = (Screen.width*(1-w))/2;
			gameOverRect.y = (Screen.height*(1-h))/2;
			gameOverRect.width = Screen.width*w;
			gameOverRect.height = Screen.height*h;
			//give text a style to align it and give it proper fontsize
			GUIStyle gameOverTextStyle = new GUIStyle("label");
			gameOverTextStyle.alignment = TextAnchor.MiddleCenter;
			gameOverTextStyle.fontSize =  Mathf.Min(Mathf.FloorToInt(Screen.width * fontSize/1000), 
			                                        Mathf.FloorToInt(Screen.height * fontSize/1000));
			//display game over in gameOverRect with predefined style
			GUI.Label( gameOverRect, "<b><color=red>Game over!</color></b>", gameOverTextStyle);
		}
		//create style for pause button
		GUIStyle pauseStyle = new GUIStyle("button");
		pauseStyle.alignment = TextAnchor.MiddleCenter;
		pauseStyle.fontSize = 40;
		//if pause button is pressed
		if(GUI.Button(new Rect(2 * width/3, 0, 100, 100), "<b>II</b>", pauseStyle )){
			//and game was not paused
			if(!pause){
				//in-game time stops and control boolean pause = true
				Time.timeScale = 0;
				pause = true;
			}
		}
		//if game is paused
		if(pause){
			//create style for menu window
			GUIStyle menuStyle = new GUIStyle("window");
			menuStyle.fontSize = Mathf.Min(Mathf.FloorToInt(width/3 * fontSize/1000), 
			                               Mathf.FloorToInt(height/3 * fontSize/1000));
			menuStyle.stretchHeight = true;
			menuStyle.alignment = TextAnchor.UpperCenter;
			//create modal window for menu (is created in function MenuWindow) with title Menu
			windowRect = GUI.ModalWindow(0, windowRect, MenuWindow, "Menu", menuStyle);
		}

	}
	//menu window buttons
	void MenuWindow(int windowID){
		//adding style to button
		GUIStyle buttonStyle = new GUIStyle("button");
		//center both horizontally and vertically
		buttonStyle.alignment = TextAnchor.MiddleCenter;
		//fontsize depends on screen size
		buttonStyle.fontSize = Mathf.Min(Mathf.FloorToInt(width/3 * fontSize/1000), 
		                                 Mathf.FloorToInt(height/3 * fontSize/1000));
		//menu buttons
		if(GUI.Button(new Rect(width/12, height/6 - 60, width/3, 100), "Resume", buttonStyle)){
			//if Resume is clicked, control bool pause = false, in-game time speed will be normal
			pause = false;
			Time.timeScale = 1;
		}else if (GUI.Button(new Rect(width/12, height/4 - 30, width/3, 100), "Restart", buttonStyle)){
			//if restart, restart the game level
			Application.LoadLevel("main scene");
		}else if (GUI.Button(new Rect(width/12, height/3, width/3, 100), "Return to home", buttonStyle)){
			//if return to home, load the start screen
			Application.LoadLevel("home scene");
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
			//get keyboard input
			h = Input.GetAxis("Horizontal");
			v = Input.GetAxis("Vertical");
		}

		//set animation parameters for player animations
		if(Mathf.Abs(h) > 0.01 || Mathf.Abs(v) > 0.01){
			//if player is moving, start move-animation
			gameObject.GetComponent<Animator>().SetBool("moving", true);
		}else {
			//if player is not moving, stop move-animation
			gameObject.GetComponent<Animator>().SetBool("moving", false);
		}
		
		// Apply horizontal velocity
		if (Mathf.Abs(h) > 0) {
			//check if player is at edges of background
			if(transform.position.x > boundMaxX - 0.75f){
				//if so, make sure the player cannot cross the boundaries
				h = Mathf.Clamp(h, -1, -0.5f);
			}else if(transform.position.x < boundMinX + 0.75f){
				h = Mathf.Clamp(h, 0.5f, 1);
			}
			//apply velocity
			rigidbody2D.velocity = new Vector2(h * speed, rigidbody2D.velocity.y);
		}
		
		// Apply vertical velocity
		if (Mathf.Abs(v) > 0) {
			//check if player is at edges of background
			if(transform.position.y > boundMaxY - 0.75f){
				//if so, make sure the player cannot cross the boundaries
				v = Mathf.Clamp(v, -1, -0.5f);
			}else if(transform.position.y < boundMinY + 0.75f){
				v = Mathf.Clamp(v, 0.5f, 1);
			}
			//apply velocity
			rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, v * speed);
		}
		//rotate player to either firing direction (if firing weapon) or moving direction
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
			                                                   new Vector2(Gun.rigidbody2D.position.x, 
			            												    Gun.rigidbody2D.position.y), 
			                                                   Quaternion.identity);
			//set damage of bullet
			currentBullet.GetComponent<BulletScript>().setDamage(damage);
			//find direction the bullet is fired
			Vector3 angle = Gun.transform.eulerAngles;
			currentBullet.transform.eulerAngles = angle;
			Vector3 forceVector = currentBullet.transform.right;
			//add force to the bullet to actually fire it away
			currentBullet.rigidbody2D.AddForce(forceVector * bulletSpeed * 2f, ForceMode2D.Impulse);
		}
	}

	//set damage of bullet, used by power-ups to increase damage
	public void setBulletDamage(int damage){
		this.damage = damage;
	}

	//if trigger collision is... triggered ;)
	void OnTriggerEnter2D(Collider2D coll){ 
		//if collided with zombie and player is not invincible
		if(coll.CompareTag("zombie") && !isInvincible){
			//Vibrate device
			Handheld.Vibrate();
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
	//reload level
	void ReloadLevel(){
		Application.LoadLevel("home scene");
	}
	//add points to score, called from other scripts
	public void addScore(int points){
		score += points;
	}
	//change fire rate of weapon, thanks, Power Up
	public void changeFireRate(float faster){
		fireRate -= faster;
		Debug.Log ("firerate = " + fireRate);
	}
	//do extra damage, thanks, Power Up
	public void changeDamage(int extraDamage){
		damage += extraDamage;
		Debug.Log ("Damage = " + damage);
	}
	//get extra life, thanks, Power Up
	public void addHealth(int extraHealth){
		health += extraHealth;
		Debug.Log ("Health = " + health);
	}

}