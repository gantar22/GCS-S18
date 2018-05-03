﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class y_meter : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (!GM.Instance.player) {
			return;
		}

		GetComponent<Image>().fillAmount = 1 - GM.Instance.player.GetComponentInChildren<YButtonManager>().getYBarValue();
	}
}
