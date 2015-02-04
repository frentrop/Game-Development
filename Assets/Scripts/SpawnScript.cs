using UnityEngine;
using System.Collections;

public class SpawnScript : MonoBehaviour {

	//zombie prefab
	public GameObject zombie;
	public int numZombies = 1;

	private bool zombieCheck = false;

	//spawnPoints zombie
	Vector3 north,east,south,west;

	//power up spawn time
	public float minSpawnTime = 10f;
	public float maxSpawnTime = 40f;

	public GameObject powerUpPrefab;

	public GameObject popUp;

	// Use this for initialization
	void Start () {
		GameObject PopUp = (GameObject)Instantiate(popUp);
		PopUp.GetComponent<GUIPopUp>().setText("New Game!");
		north = new Vector3 (0f, renderer.bounds.max.y + 1f, 0f);
		south = new Vector3 (0f, renderer.bounds.min.y - 1f, 0f);
		east = new Vector3 (renderer.bounds.max.x + 1f, 0f, 0f);
		west = new Vector3 (renderer.bounds.min.x - 1f, 0f, 0f);
		newWave (0);
		Invoke ("powerSpawn",maxSpawnTime);
	}
	
	// Update is called once per frame
	void Update () {
		//checkZombies ();
		if (zombieCheck) {
			if (UnityEngine.GameObject.FindWithTag ("zombie") == null) {
					Debug.Log ("No zombies, new wave!");
					GameObject PopUp = (GameObject)Instantiate(popUp);
					PopUp.GetComponent<GUIPopUp>().setText("New Wave!");
					newWave (1);
			}
			zombieCheck = false;
		}
	}

	//create new wave of zombies
	public void newWave(int extraZombies){
		//initialize new wave
		numZombies += extraZombies;
		for (int i = 0; i < numZombies; i++) {
			Instantiate(zombie,north,Quaternion.identity);
			Instantiate(zombie,south,Quaternion.identity);
			Instantiate(zombie,east,Quaternion.identity);
			Instantiate(zombie,west,Quaternion.identity);
		}
	}

	//zombie dead, check if there are zombies left
	public void checkZombies(){
		zombieCheck = true;
	}

	void powerSpawn(){
		Instantiate(powerUpPrefab);
		Invoke ("powerSpawn", Random.Range (minSpawnTime, maxSpawnTime));
	}

	//TODO check score, and give zombies more health if score gets higher
}
