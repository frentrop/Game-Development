﻿using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour
{

	private float bulletSpawn;
	public float bulletDestroyTime = 2.0f;
	public float zombieRecoil = 0.03f;
	public int damage;

	void Start(){
		bulletSpawn = Time.time;
	}

	void Update(){
		if(Time.time > bulletSpawn + bulletDestroyTime){
			Destroy (gameObject);
		}
	}

	//bullet will destroy when collided with wall
	void OnCollisionExit2D(Collision2D collisionInfo)
	{
		Debug.Log("collision exit 2D");
		//destroy if collides
		Destroy(gameObject);
	}

	void OnTriggerEnter2D(Collider2D coll){
		if(coll.CompareTag("zombie")){
			//Debug.Log("trigger collision");
			coll.transform.Translate(zombieRecoil * gameObject.rigidbody2D.velocity.x, 
			                         zombieRecoil * gameObject.rigidbody2D.velocity.y, 
			                         0);
			coll.GetComponent<ZombieController>().isHit(damage);
			Destroy(gameObject);
		}
		if(coll.CompareTag("levelGeometry")){
			Destroy(gameObject);
		}
	}

	public void setDamage(int damage){
		this.damage = damage;
	}
}