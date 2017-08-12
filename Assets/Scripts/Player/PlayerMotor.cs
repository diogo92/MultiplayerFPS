using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : NetworkBehaviour {
	[SerializeField]
	private Camera cam;

	[SerializeField]
	private float cameraRotationLimit = 85f;

	[SerializeField]
	private Transform SpineBoneAux;
	[SerializeField]
	private Vector3 rotCorrection;
	[SerializeField]
	private WeaponIK NetworkModelIK;

	private Vector3 velocity = Vector3.zero;
	private Vector3 rotation = Vector3.zero;
	private float cameraRotationX = 0;
	private float currentCameraRotationX = 0f;
	private Vector3 thrusterForce = Vector3.zero;

	private Rigidbody rb;
	void Start(){
		rb = GetComponent<Rigidbody> ();
	}

	public void Move(Vector3 _velocity){
		velocity = _velocity;
	}

	public void Rotate(Vector3 _rotation){
		rotation = _rotation;
	}

	public void RotateCamera(float _cameraRotation){
		cameraRotationX = _cameraRotation;
	}

	public void ApplyThruster(Vector3 _thrusterForce){
		thrusterForce = _thrusterForce;
	}

	void FixedUpdate(){

		PerformMovement ();
		PerformRotation ();
	}

	void PerformMovement(){
		if (velocity != Vector3.zero) {
			rb.MovePosition (rb.position + velocity * Time.fixedDeltaTime);
		}
		if (thrusterForce != Vector3.zero) {
			rb.AddForce (thrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration);
		}
	}

	void PerformRotation(){
		rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));

		if (cam != null) {
			currentCameraRotationX -= cameraRotationX;
			currentCameraRotationX = Mathf.Clamp (currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);
			cam.transform.localEulerAngles = new Vector3 (currentCameraRotationX, 0, 0);
		}
	}

	void LateUpdate(){
		//if (!isLocalPlayer) {
			float rotval = currentCameraRotationX;
			float clampedVal = Mathf.Clamp (currentCameraRotationX, -30, 30);
			cam.transform.localEulerAngles = new Vector3 (clampedVal, 0, 0);
			SpineBoneAux.rotation = Quaternion.LookRotation (cam.transform.forward);
			SpineBoneAux.rotation *= Quaternion.Euler (rotCorrection);
			cam.transform.localEulerAngles = new Vector3 (currentCameraRotationX, 0, 0);
			Vector3 final = SpineBoneAux.rotation.eulerAngles;
			//final.z = Mathf.Clamp (final.z,-30,30);
			SpineBoneAux.rotation = Quaternion.Euler (final);
		//}
	}
}
