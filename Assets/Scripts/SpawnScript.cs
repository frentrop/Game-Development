using UnityEngine;
using System.Collections;

public class SpawnScript : MonoBehaviour {
	//zombie prefab
	public GameObject zombie, player;
	public int numZombies = 1;
	public int extraHealth = 0;
	public float extraSpeed = 0;
	//control boolean for checking zombies
	private bool zombieCheck = false;
	//spawnPoints zombie
	Vector3 north,east,south,west;
	float minX, minY, maxX, maxY;
	//power up spawn time
	public float minSpawnTime = 10f;
	public float maxSpawnTime = 40f;
	//power up prefab
	public GameObject powerUpPrefab;
	//pop up prefab
	public GameObject popUp;

	// Use this for initialization
	void Start () {
		//create pop up for new game
		GameObject PopUp = (GameObject)Instantiate(popUp);
		PopUp.GetComponent<GUIPopUp>().setText("New Game!");
		player = GameObject.Find("Player");
		minX = renderer.bounds.min.x;
		maxX = renderer.bounds.max.x;
		minY = renderer.bounds.min.y;
		maxY = renderer.bounds.max.y;
		//set zombie spawning points
		north = new Vector3 (0f, maxY + 1f, 0f);
		south = new Vector3 (0f, minY - 1f, 0f);
		east = new Vector3 (maxX + 1f, 0f, 0f);
		west = new Vector3 (minX - 1f, 0f, 0f);
		//start waves and power up spawns
		newWave (0);
		Invoke ("powerSpawn",maxSpawnTime);
	}
	//used to provide other scripts wih position of the player
	public void playerPos(out Vector3 playerposition){
		playerposition = player.transform.position;
	}
	//used to provide other scripts with boundary values of map
	public void backgroundBounds(out float minX, out float maxX, out float minY, out float maxY){
		minX = this.minX;
		maxX = this.maxX;
		minY = this.minY;
		maxY = this.maxY;
	}	
	// Update is called once per frame
	void Update () {
		//does the level need to be checked for zombies?
		if (zombieCheck) {
			//check for zombie tag
			if (UnityEngine.GameObject.FindWithTag ("zombie") == null) {
					//no zombies, so give pop up and spawn new wave
					GameObject PopUp = (GameObject)Instantiate(popUp);
					PopUp.GetComponent<GUIPopUp>().setText("New Wave!");
					newWave (1);
			}
			//level has been checked
			zombieCheck = false;
		}
	}

	//create new wave of zombies
	public void newWave(int extraZombies){
		//It's aliiiiveeee!!!
		numZombies += extraZombies;
		//spawn zombies in all spawn points
		for (int i = 0; i < numZombies; i++) {
			GameObject north = Instantiate(zombie,
			            				   north + new Vector3(Random.Range (-2f, 2f),Random.Range (-1f, 5f),0),
			            				   Quaternion.identity);
			north.GetComponent<ZombieController>().improveZombie(extraHealth, extraSpeed);
			Instantiate(zombie,
			            south + new Vector3(Random.Range (-2f, 2f),Random.Range (-5f, 1f),0),
			            Quaternion.identity);
			Instantiate(zombie,
			            east + new Vector3(Random.Range (-1f, 5f),Random.Range (-2.3f, 2.3f),0),
			            Quaternion.identity);
			Instantiate(zombie,
			            west + new Vector3(Random.Range (-5f, 1f),Random.Range(-2.3f,2.3f),0),
			            Quaternion.identity);
		}
	}

	//zombie dead, check if there are zombies left, can be called by other scripts
	public void checkZombies(){
		zombieCheck = true;
	}
	//spawns power ups
	void powerSpawn(){
		//spawn the power up
		Instantiate(powerUpPrefab);
		//call this function after a random time in an interval
		Invoke ("powerSpawn", Random.Range (minSpawnTime, maxSpawnTime));
	}

	//TODO check score, and give zombies more health or speed if score gets higher

}
