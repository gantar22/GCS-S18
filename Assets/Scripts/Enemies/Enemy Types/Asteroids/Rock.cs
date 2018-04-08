﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour {

	// References
	[HideInInspector]
	public Asteroid asteroid;
	private EnemyHP EHP;


	// Initialize
	void Awake () {
		asteroid = GetComponentInParent<Asteroid> ();
		EHP = GetComponent<EnemyHP> ();
	}

	// Check for collisions with enemies
	void OnTriggerEnter2D(Collider2D col){
		EnemyHP s = col.gameObject.GetComponent<EnemyHP>(); 
		if (s != null && col.gameObject.GetComponent<Turret>() == null){
			s.die ();
			EHP.gotHit (1);
		}
	}
}
