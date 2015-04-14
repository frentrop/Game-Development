using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour{
	private float bulletSpawn;
	public float bulletDestroyTime = 2.5f;
	public float zombieRecoil = 0.03f;
	public int damage;

	void Start(){
		//time when bullet is spawned
		bulletSpawn = Time.time;
		//TODO add bullet fired sound effect
	}

	void Update(){
		//destroy gameobject if it hasn't hit anything in time
		if(Time.time > bulletSpawn + bulletDestroyTime){
			Destroy (gameObject);
		}
	}
	//collision with other objects
	void OnTriggerEnter2D(Collider2D coll){
		//check what it hit
		if(coll.CompareTag("zombie")){
			//add recoil to hit zombie
			//TODO fix recoil translation!
			//bullet is given a force, maybe that creates the problem of wrong translation
			coll.transform.Translate(zombieRecoil * gameObject.rigidbody2D.velocity.x, 
			                         zombieRecoil * gameObject.rigidbody2D.velocity.y, 
			                         0);
			coll.GetComponent<ZombieController>().isHit(damage);
			//TODO add bullet hit zombie sound effect
			Destroy(gameObject);
		}else if(coll.CompareTag("levelGeometry")){
			Destroy(gameObject);
			//TODO add level geometry hit sound effect
		}
	}
	//set the damage done by bullets
	public void setDamage(int damage){
		this.damage = damage;
	}
}