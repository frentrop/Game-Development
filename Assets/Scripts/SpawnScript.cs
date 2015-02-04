using UnityEngine;
using System.Collections;

public class SpawnScript : MonoBehaviour {
	//zombie prefab
	public GameObject zombie;
	public int numZombies = 1;
	//control boolean for checking zombies
	private bool zombieCheck = false;
	//spawnPoints zombie
	Vector3 north,east,south,west;
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
		//set zombie spawning points
		north = new Vector3 (0f, renderer.bounds.max.y + 1f, 0f);
		south = new Vector3 (0f, renderer.bounds.min.y - 1f, 0f);
		east = new Vector3 (renderer.bounds.max.x + 1f, 0f, 0f);
		west = new Vector3 (renderer.bounds.min.x - 1f, 0f, 0f);
		//start waves and power up spawns
		newWave (0);
		Invoke ("powerSpawn",maxSpawnTime);
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
			Instantiate(zombie,north,Quaternion.identity);
			Instantiate(zombie,south,Quaternion.identity);
			Instantiate(zombie,east,Quaternion.identity);
			Instantiate(zombie,west,Quaternion.identity);
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

	//TODO check score, and give zombies more health if score gets higher
}
