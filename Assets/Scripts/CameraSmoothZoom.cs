﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSmoothZoom : MonoBehaviour {

	// Settings
	public float zoomTime;

	// Other variables
	private float targetSize;
	private float currentZoomVelo;

	// References
	private Camera cam;
	[SerializeField]
	private Camera UICam;


	// Initialization
	void Awake () {
		cam = GetComponent<Camera> ();

		targetSize = cam.orthographicSize;
		currentZoomVelo = 0f;

		// Testing
		//addToCameraSize(5f);
	}
	
	// Called once per frame
	void Update () {
		float newSize = Mathf.SmoothDamp (cam.orthographicSize, targetSize, ref currentZoomVelo, zoomTime);
		cam.orthographicSize = newSize;
		UICam.orthographicSize = newSize;
	}
		
	// Update the target camera size
	public void newCameraSize (float newTarget) {
		targetSize = newTarget;
		currentZoomVelo = 0f;
	}

	// Add to the target camera size
	public void addToCameraSize (float addition) {
		newCameraSize (targetSize + addition);
	}
}
