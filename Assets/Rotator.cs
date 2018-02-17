﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class Rotator : MonoBehaviour {
	[SerializeField]
	float turnRadius;
	[SerializeField]
	XboxController ctlr;

	private float targetTheta;
	private float deltaTheta;
	private Vector2 joy;
	private float prevTargetTheta = 0;
	
	// Update is called once per frame
	void Update () {
		
		joy = new Vector2(XCI.GetAxisRaw(XboxAxis.LeftStickX,ctlr),XCI.GetAxisRaw(XboxAxis.LeftStickY,ctlr));
		if (joy.x == 0 && joy.y == 0) return;
		if(joy.y == 0) targetTheta = 180 * joy.x * -1;
		targetTheta = joy.x == 0 ? 90 * joy.y : (360 / (2 * Mathf.PI)) * Mathf.Atan(joy.y / joy.x);

		if(joy.x < 0) targetTheta += 180;
		if(targetTheta < 0) targetTheta += 360;

		if(joy.x > 0) {
			if((transform.eulerAngles.z > 270 || transform.eulerAngles.z == 0)&& joy.y > 0){
				targetTheta += 360; 
			} else if(transform.eulerAngles.z < 180 && joy.y < 0){
				targetTheta -= 360;
			} 
		}

		deltaTheta = mod((targetTheta - transform.eulerAngles.z),360);
		if(Mathf.Abs(deltaTheta) > 360) print(deltaTheta);

		transform.Rotate(Vector3.forward * Mathf.Clamp(deltaTheta,-1 * turnRadius,turnRadius));
		prevTargetTheta = targetTheta;
	}

	float mod(float m,float n){
		return m % n < 0 ? m % n + n : m % n;
	}
}
