using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameplate : MonoBehaviour {

	[SerializeField]
	private Text usernameText;

	[SerializeField]
	private RectTransform healthbarFill;

	[SerializeField]
	private PlayerManager player;

	public Camera cam; 
	public float objectScale = 1.0f; 
	public bool resizeDistance = false;
	private Vector3 initialScale; 

	// set the initial scale, and setup reference camera
	void Start ()
	{
		cam = Camera.main;
		// record initial scale, use this as a basis
		initialScale = transform.localScale; 

		// if no specific camera, grab the default camera
		if (cam == null)
			cam = Camera.main; 
	}

	void Update () {
		cam = Camera.main;
		usernameText.text = player.username;	
		healthbarFill.localScale = new Vector3(player.GetHealthAmount(),1f,1f);
		CameraFacingBillboard ();
	}

	void CameraFacingBillboard(){
		transform.LookAt (transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
		if (resizeDistance) {
			Plane plane = new Plane(cam.transform.forward, cam.transform.position); 
			float dist = plane.GetDistanceToPoint(transform.position); 
			transform.localScale = initialScale * dist * objectScale; 
		}
	}
}
