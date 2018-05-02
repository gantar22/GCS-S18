﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class Player_Missile_Fire : MonoBehaviour {

	// Settings
	[SerializeField]
	XboxController _ctlr;
	[SerializeField]
	XboxButton _button;
	[SerializeField]
	GameObject _missile;

	[SerializeField]
	float _offset;
	[SerializeField]
	int _explosionDamage;

	// Other variables
	private float cooldown;

	// Upgrade properties
	private static float lvl1ExploRadiusMult = 1.8f, lvl2ExploRadiusMult =  3.6f;
	private static float lvl1Cooldown = 8f, lvl2Cooldown = 3f;
	private static float angleDiffDegrees = 35;

	private static bool missilesEnabled;
	private static float exploRadiusMult;
	private static float maxCooldown;
	private static bool tracking;
	private static bool tripleShot;


	// Initialize the upgrade settings to lvl1 if not already set
	void Start () {
		if (exploRadiusMult == 0f) {
			exploRadiusMult = lvl1ExploRadiusMult;
			maxCooldown = lvl1Cooldown;
		}

		// Testing:
		//UnlockMissiles(1);
		//EnableTracking (1);
		//EnableTripleShot(1);
		//UpgradeCooldown (1);
		//UpgradeExplosionRadius (1);
	}

	// Called once per frame
	void Update () {
		if (!missilesEnabled) {
			return;
		}

		if (cooldown > 0) {
			cooldown -= Time.deltaTime;
		} else if (XCI.GetButtonDown (_button, _ctlr)) {
			fire ();
		} else if (Input.GetKey (KeyCode.X)) { // KEYBOARD TESTING --- REMOVE
			fire ();
		}
	}

	// Fire missile(s)
	private void fire() {
		// Effects

		//music_manager.Instance.shot (); // PLAY A MISSILE SHOOTING SOUND HERE
		GetComponentInChildren<ParticleSystem>().Play(); // PLays same particles as normal bullets

		CameraShakeScript CSS = Camera.main.GetComponent<CameraShakeScript> ();
		if(CSS != null){
			CSS.activate(.01f,.05f); //this feels bad
		}

		// Shoot the missile(s)
		float a = transform.eulerAngles.z * Mathf.Deg2Rad;
		Vector3 offset = new Vector3 (Mathf.Cos (a), Mathf.Sin (a), 0) * _offset;
		spawnMissile (transform.position + offset, 0f);

		if (tripleShot) {
			float angleDiffRad = angleDiffDegrees * Mathf.Deg2Rad;
			float aLeft = a - angleDiffRad;
			float aRight = a + angleDiffRad;

			Vector3 leftOffset = new Vector3 (Mathf.Cos (aLeft), Mathf.Sin (aLeft), 0) * _offset;
			Vector3 rightOffset = new Vector3 (Mathf.Cos (aRight), Mathf.Sin (aRight), 0) * _offset;

			spawnMissile (transform.position + leftOffset, -1f * angleDiffDegrees);
			spawnMissile (transform.position + rightOffset, angleDiffDegrees);
		}

		cooldown = maxCooldown;
	}

	private void spawnMissile (Vector3 pos, float angleDiffDegrees) {
		GameObject missile = Instantiate(_missile, pos, transform.rotation);
		linear_travel linTrav = missile.GetComponentInChildren<linear_travel> ();
		seeking_missile seeking = missile.GetComponentInChildren<seeking_missile> ();

		//Vector2 playerVelo = transform.root.gameObject.GetComponentInChildren<PlayerMove> ().velo;

		if (tracking) {
			linTrav.enabled = false;
			seeking.enabled = true;
		
			//seeking.setVelo (playerVelo);
			Vector2 direction = pos - transform.position;
			seeking.setVelo (direction);
		} else {
			linTrav.enabled = true;
			seeking.enabled = false;

			//linTrav.setSpeed(playerVelo.magnitude);
		}

		// Set the correct rotation
		missile.transform.Rotate(new Vector3(0, 0, angleDiffDegrees));

		// Set its explosion settings
		BulletScript BS = missile.GetComponentInChildren<BulletScript>();
		BS.setExploSettings(exploRadiusMult, _explosionDamage);
	}


	// ====== UPGRADES =====

	// Enable missile shooting
	public static void UnlockMissiles (int total) {
		if (total == 0) {
			missilesEnabled = false;
		} else {
			missilesEnabled = true;
		}
	}

	// Upgrade explosions
	public static void UpgradeExplosionRadius (int total) {
		if (total == 0) {
			exploRadiusMult = lvl1ExploRadiusMult;
		} else {
			exploRadiusMult = lvl2ExploRadiusMult;
		}
	}

	// Upgrades cooldown time
	public static void UpgradeCooldown (int total) {
		if (total == 0) {
			maxCooldown = lvl1Cooldown;
		} else {
			maxCooldown = lvl2Cooldown;
		}
	}

	// Enables tracking on missiles
	public static void EnableTracking (int total) {
		if (total == 0) {
			tracking = false;
		} else {
			tracking = true;
		}
	}

	// Enables triple shot missiles
	public static void EnableTripleShot (int total) {
		if (total == 0) {
			tripleShot = false;
		} else {
			tripleShot = true;
		}
	}
}
