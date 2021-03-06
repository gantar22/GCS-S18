﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class bomb_det : MonoBehaviour {

	public float cooldown = 60;
	bool canfire;
	public float timer;

	void Start(){
		canfire = true;
	}
	
	// Update is called once per frame
	void Update () {
		if(((XCI.GetButton(XboxButton.B,XboxController.First) && XCI.GetButton(XboxButton.B,XboxController.Second))
			#if UNITY_EDITOR
			|| Input.GetKeyDown(KeyCode.B)
			#endif
		)
		 && canfire && !PauseManager.paused){
			/*GameObject[] allObjects = Object.FindObjectsOfType<GameObject>() ;
			foreach(GameObject go in allObjects){
				if (go.activeInHierarchy && go.GetComponent<EnemyHP>())
				go.GetComponent<EnemyHP>().hitByBomb();
			}*/

			canfire = false;
			foreach(GameObject go in GM.Instance.enemies){
				EnemyHP EHP = go.GetComponent<EnemyHP> ();
				if (EHP) {
					EHP.hitByBomb ();
				}
			}
			foreach(BulletScript bs in FindObjectsOfType<BulletScript>())
			{
				EnemyHP[] EHPs = bs.gameObject.GetComponentsInChildren<EnemyHP>();
				foreach (EnemyHP EHP in EHPs) {
					EHP.die();
				}
				Destroy(bs.gameObject);
			}
			CameraShakeScript CSS = Camera.main.GetComponent<CameraShakeScript> ();
			if(CSS != null){
				CSS.activate(.5f,.5f); 
			}
			//Invoke("act",cooldown);
			timer = cooldown;
		}
		//timer -= Time.deltaTime;
		//no recharging over time
		if(timer <= 0) act();
	}

	public bool addCharge(int c){
		bool r = timer > 0;
		timer -= 10 * c;
		timer = Mathf.Max (timer, 0);
		return r;
	}

	public void refreshCharge() {
		timer = 0;
	}

	void act(){
		canfire = true;
	}
}
