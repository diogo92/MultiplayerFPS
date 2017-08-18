using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
 * World Space UI that shows the player's HP and name
 */
public class PlayerNameplate : MonoBehaviour {

	[SerializeField]
	private Text usernameText;

	[SerializeField]
	private RectTransform healthbarFill;

	[SerializeField]
	private PlayerManager player;

	public Camera cam; 

	//Scaling of the object
	public float objectScale = 1.0f; 
	//Check if we want to change size depending on distance from the player
	public bool resizeDistance = false;
	//Original scale vector
	private Vector3 initialScale; 

	void Start ()
	{
		cam = Camera.main;

		initialScale = transform.localScale; 

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
		//Always face the player
		transform.LookAt (transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);

		if (resizeDistance) {
			Plane plane = new Plane(cam.transform.forward, cam.transform.position); 
			float dist = plane.GetDistanceToPoint(transform.position); 
			transform.localScale = initialScale * dist * objectScale; 
		}
	}
}
