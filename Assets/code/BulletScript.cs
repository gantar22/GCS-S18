﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(ObjT))]
public class BulletScript : MonoBehaviour {
	public int _damage;

	public enum bulletType {basic, sin, missile,firework}

	public bulletType _type;

	private float exploRadiusMult = 0f;
	private int exploDamage = 0;

	private ObjT objT;

	// Variables to make sure bullet only hits one thing
	private List<GameObject> objsToHit;
	//private bool hitInvulnerable;

	// Ricochet counter
	[HideInInspector]
	public int ricochetsLeft = 0;
	private GameObject lastHit;


	void Awake () {
		objT = GetComponent<ObjT> ();
		objsToHit = new List<GameObject> ();
		//hitInvulnerable = false;
	}

	// Called once per frame, after all physics & collision checks
	void Update () {
		// If the bullet has collided with anything, then damage one.
		// If it only hits something invulnerable, then hit it. Otherwise damage something else
		bool damagedSomething = false;
		Invulnerable invulnToHit = null;

		while (objsToHit.Count > 0) {
			GameObject nextObj = objsToHit [0];
			if (nextObj == null) {
				objsToHit.RemoveAt (0);
				continue;
			}

			Invulnerable I = nextObj.GetComponent<Invulnerable> ();
			if (I != null && I.enabled) {
				invulnToHit = I;
				objsToHit.Remove (nextObj);
			} else if (dealDamage (nextObj, _damage, transform.position, objT.typ, transform.right)) {
				//(GetComponent<seeking_missile>() && GetComponent<ObjT>() && GetComponent<ObjT>().typ == ObjT.obj.enemy_bullet)
				if (ricochetsLeft > 0) {
					ricochet (nextObj);
					lastHit = nextObj;
				} else {
					die ();
				}

				objsToHit.Clear ();
				damagedSomething = true;
			} else {
				objsToHit.Remove (nextObj);
			}
		}

		if (!damagedSomething && invulnToHit != null) {
			invulnToHit.gotHit ();
			die ();
		}
	}

	void OnTriggerEnter2D(Collider2D col){
		if(sword_script.sword_destroys_projectiles && col.gameObject.tag == "spike"){
			die();
			//destroyAttachedEnemyHP();
			//Destroy(transform.root.gameObject);
		}
		if (col.gameObject.GetComponent<turtle_shield>()){
			if(turtle_script.refl){
				transform.right = (transform.position - col.gameObject.transform.position).normalized;
				sin_travel sin;
				if(sin = GetComponentInChildren<sin_travel>())
				{
					sin.redirect();
				}
			} else {
				die();
				//destroyAttachedEnemyHP();
				//Destroy(transform.root.gameObject);
			}

		}

		if (col.gameObject != lastHit) {
			objsToHit.Add (col.gameObject);
		}

		/* if (hitInvulnerable) {
			return;
		}

		Invulnerable I = col.gameObject.GetComponent<Invulnerable> ();
		if (I != null && I.enabled) {
			hitInvulnerable = true;
			I.gotHit ();
			die ();
		} else {
			objsToHit.Add (col.gameObject);
		} */
	}

	// Anything that does damage should call this to damage whatever it hit.
	// Returns true if it actually deals damage (so that the bullet should die, etc.)
	public static bool dealDamage (GameObject obj, int damage, Vector3 pos, ObjT.obj type,Vector3 bullet) {
		if (obj == null) {
			return false;
		}

		EnemyHP s = obj.GetComponent<EnemyHP>(); 
		if (s != null){
			s.gotHit(bullet,damage);
			return true;
		}

		if(obj.layer == 9){
			return true;
		}

		PlayerHP PHP = obj.GetComponent<PlayerHP> ();
		if (PHP != null && (type != ObjT.obj.player_bullet) && (type != ObjT.obj.player_explosion)) {

			if(!PlayerHP.dashing) PHP.gotHit (damage);
			return true;
		}

		upgrade_button ub;
		if((ub = obj.GetComponentInParent<upgrade_button>())){
			ub.put_description();
			return true; // I beleive this will destroy the bullet
		}
		
		return false;
	}

	void die(){
		switch(_type){
		case bulletType.basic:
			ParticleSystem ps = transform.GetComponentInChildren<ParticleSystem>();
			if(ps){
			ps.transform.parent = null;
			ps.Play();}
			music_manager.play_by_name("player_hit");
			break;

		case bulletType.missile:
			if (exploRadiusMult > 0f && exploRadiusMult > 0f) {
				BoxCollider2D boxCol = GetComponentInChildren<BoxCollider2D> ();
				Vector3 exploPos;
				if (boxCol) {
					Vector3 offset = boxCol.size.x * transform.lossyScale.x * 0.5f * (transform.rotation * Vector3.right);
					exploPos = transform.position + offset;
				} else {
					exploPos = transform.position;
				}
				
				AOE_Damage aoe_damage = Instantiate (PrefabManager.Instance.aoe_damage, exploPos, transform.rotation);

				aoe_damage.scaleExplosion (exploRadiusMult);
				aoe_damage._damage = exploDamage;
			}
			destroyAttachedEnemyHP();
			break;
		default:
			break;
		}
		Destroy(transform.root.gameObject);
	}

	// Ricochet off the object that was just hit
	private void ricochet(GameObject justHit) {
		/* Vector3 normal = transform.position - justHit.transform.position;
		Vector3 newDirection = Vector3.Reflect (transform.right, normal);

		//Vector3 newDirection = transform.right.normalized * -1f;
		newDirection += Vector3.right * Random.Range (-0.1f, 0.1f) + Vector3.up * Random.Range (-0.1f, 0.1f);
		transform.right = newDirection; */

		Vector3 leftBounce = ((Vector2)transform.right).Rotate (180f + 75f);
		Vector3 rightBounce = ((Vector2)transform.right).Rotate (180f - 75f);

		Vector3 normal = transform.position - justHit.transform.position;
		float leftDot = Vector3.Dot (leftBounce, normal);
		float rightDot = Vector3.Dot (rightBounce, normal);

		if (leftDot > rightDot) {
			transform.right = leftBounce;
		} else {
			transform.right = rightBounce;
		}

		ricochetsLeft--;
		music_manager.play_by_name("player_hit");
	}

	// Set the explosion settings for this bullet (ie. missile)
	public void setExploSettings (float radiusMult, int damage) {
		exploDamage = damage;
		exploRadiusMult = radiusMult;
	}

	// Public function to destroy this bullet
	public void destroyBullet() {
		die ();
	}

	// Checks for any attached EnemyHP components (if this is a missile) and kills them if they exist
	private void destroyAttachedEnemyHP() {
		if (_type != bulletType.missile) {
			return;
		}
		EnemyHP[] EHPs = transform.root.gameObject.GetComponentsInChildren<EnemyHP>();
		foreach (EnemyHP EHP in EHPs) {
			EHP.die();
		}
	}
}
