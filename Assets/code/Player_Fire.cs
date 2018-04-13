﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class Player_Fire : MonoBehaviour {

	[SerializeField]
	XboxController _ctlr;
	[SerializeField]
	XboxButton _button;
	[SerializeField]
	GameObject _bullet;

	[SerializeField]
	float _offset;
	[SerializeField]
	[Range(.1f,10)]
	float _fire_rate; //per second


	private bool cool_down;
	private int bullets_fired;

	[HideInInspector]
	public bool cantFire;

	
	// Update is called once per frame
	void Update () {
		if((XCI.GetButton(_button,_ctlr) || Input.GetKeyDown(KeyCode.Space)) && !cool_down){
			fire();

		}		
	}


	void fire(){
		if (cantFire) {
			// Play "click" noise here
			return;
		}

		music_manager.Instance.shot();

		CameraShakeScript CSS = Camera.main.GetComponent<CameraShakeScript> ();
		if(CSS != null){
			//CSS.activate(.2f,.05f); this feels bad
		}


		cool_down = true;
		Invoke("reload",1 / _fire_rate);
		float a = transform.eulerAngles.z * 2 * Mathf.PI / 360 ;
		GameObject bul = Instantiate(_bullet,transform.position + new Vector3(Mathf.Cos(a),Mathf.Sin(a),0) * _offset,transform.rotation);
		bul.GetComponentInChildren<ObjT>().id = bullets_fired++;
		bul.GetComponentInChildren<linear_travel>().setSpeed(transform.root.gameObject.GetComponentInChildren<PlayerMove>().velo.magnitude);

	}

	void reload(){
		cool_down = false;
	}
}
