﻿using System.Collections;
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



	/* Crouching*/
	public float CrouchSmooth = 2f;
	private float originalCamYPosition;
	private float originalColHeight;
	private float originalColYPos;
	private CapsuleCollider col;
	[SerializeField]
	private Animator NetworkModelAnim;
	[SyncVar]
	bool isCrouching;


	private Rigidbody rb;

	void Start(){
		if (isLocalPlayer) {
			rb = GetComponent<Rigidbody> ();
			col = GetComponent<CapsuleCollider> ();
			originalCamYPosition = cam.transform.localPosition.y;
			originalColHeight = col.height;
			originalColYPos = col.center.y;
		}
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

	public void VerticalRecoil(float _recoilAmount){
		cameraRotationX += _recoilAmount;
	}

	public void HorizontalRecoil(float _recoilAmount){
		rotation.y += _recoilAmount;
	}

	public void ApplyThruster(Vector3 _thrusterForce){
		thrusterForce = _thrusterForce;
	}

	void Update(){
		if (isLocalPlayer) {
			DoCrouch ();
		}
	}

	void FixedUpdate(){
		if (isLocalPlayer) {
			PerformMovement ();
			PerformRotation ();
		}
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
			Quaternion newRot = Quaternion.Euler (currentCameraRotationX, 0, 0);
			cam.transform.localRotation = Quaternion.Slerp(cam.transform.localRotation,newRot,Time.deltaTime*2f);
		}
	}

	void DoCrouch(){
		if (isCrouching) {
			cam.transform.localPosition = Vector3.Slerp (cam.transform.localPosition, new Vector3 (0, originalCamYPosition / 2f, 0), Time.deltaTime * CrouchSmooth);
			col.height = Mathf.Lerp (col.height, originalColHeight / 2f, Time.deltaTime * CrouchSmooth);
			col.center = Vector3.Slerp(col.center,new Vector3(0,originalColYPos/2f,0),Time.deltaTime * CrouchSmooth);
		} else {
			cam.transform.localPosition = Vector3.Slerp (cam.transform.localPosition, new Vector3 (0, originalCamYPosition, 0), Time.deltaTime * CrouchSmooth);
			col.height = Mathf.Lerp (col.height, originalColHeight, Time.deltaTime * CrouchSmooth);
			col.center = Vector3.Slerp(col.center,new Vector3(0,originalColYPos,0),Time.deltaTime * CrouchSmooth);
		}			
		NetworkModelAnim.SetBool ("Crouching",isCrouching);
	}

	void LateUpdate(){
		//if (!isLocalPlayer) {
		if (isLocalPlayer) {
			float rotval = currentCameraRotationX;
			float clampedVal = Mathf.Clamp (currentCameraRotationX, -30, 30);
			cam.transform.localEulerAngles = new Vector3 (clampedVal, 0, 0);
			SpineBoneAux.rotation = Quaternion.LookRotation (cam.transform.forward);
			SpineBoneAux.rotation *= Quaternion.Euler (rotCorrection);
			cam.transform.localEulerAngles = new Vector3 (currentCameraRotationX, 0, 0);
			Vector3 final = SpineBoneAux.rotation.eulerAngles;
			//final.z = Mathf.Clamp (final.z,-30,30);
			SpineBoneAux.rotation = Quaternion.Euler (final);
		}
		//}
	}

	[Command]
	public void CmdCrouch(bool _isCrouching){
		isCrouching = _isCrouching;
	}


}
