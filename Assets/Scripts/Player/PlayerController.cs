﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {

	[SerializeField]
	private float speed = 5f;
	[SerializeField]
	private float lookSensitivity = 3f;
	[SerializeField]
	private float thrusterForce = 1000f;

	[SerializeField]
	private float thrusterFuelBurnSpeed = 1f;
	[SerializeField]
	private float thrusterFuelRegenSpeed = 0.3f;
	private float thrusterFuelAmount = 1f;

	public float GetThrusterFuelAmount(){return thrusterFuelAmount;}


	[SerializeField]
	private LayerMask environmentMask;

	[Header("Spring settings:")]
	[SerializeField]
	private float jointSpring = 20f;
	[SerializeField]
	private float jointMaxForce = 40f;

	//Component caching
	private PlayerMotor motor;
	private ConfigurableJoint joint;
	private WeaponManager weaponManager;

	[SerializeField]
	private Animator FPSViewAnim;
	[SerializeField]
	private Animator NetworkModelAnim;
	void Start(){
		weaponManager = GetComponent<WeaponManager> ();
		FPSViewAnim = GetComponent<Animator> ();
		motor = GetComponent<PlayerMotor> ();
		joint = GetComponent<ConfigurableJoint> ();
		SetJointSettings (jointSpring);
	}

	void Update(){
		if (PauseMenu.IsOn) {
			if (Cursor.lockState != CursorLockMode.None)
				Cursor.lockState = CursorLockMode.None;
			motor.Move (Vector3.zero);
			motor.Rotate (Vector3.zero);
			motor.RotateCamera (0f);
			return;
		}

		if (Cursor.lockState != CursorLockMode.Locked)
			Cursor.lockState = CursorLockMode.Locked;
		//Check for ground and set spring target position
		RaycastHit _hit;
		if (Physics.Raycast (transform.position, Vector3.down, out _hit, 100f,environmentMask)) {
			joint.targetPosition = new Vector3 (0f, -_hit.point.y, 0f);
		} else {
			joint.targetPosition = new Vector3 (0f, 0f, 0f);
		}

		//Calculate velocity
		float _xMov = Input.GetAxis("Horizontal");
		float _zMov = Input.GetAxis("Vertical");

		FPSViewAnim.SetFloat ("Forward", _zMov);
		FPSViewAnim.SetFloat ("Sideways", _xMov);

		NetworkModelAnim.SetFloat ("Forward", _zMov);
		NetworkModelAnim.SetFloat ("Sideways", _xMov);

		Vector3 _movHorizontal = transform.right * _xMov;
		Vector3 _movVertical = transform.forward * _zMov;

		Vector3 _velocity = (_movHorizontal + _movVertical) * speed;

		//animator.SetFloat ("ForwardVelocity", _zMov);
		CrosshairManager.instance.playerSpeed = _velocity.magnitude;
		motor.Move (_velocity);

		//Turning around
		float _yRot = Input.GetAxisRaw ("Mouse X");

		Vector3 _rotation = new Vector3 (0f, _yRot, 0f) * lookSensitivity;

		motor.Rotate (_rotation);

		//Camera rotation
		float _xRot = Input.GetAxisRaw ("Mouse Y");

		float _cameraRotationX = _xRot * lookSensitivity;

		motor.RotateCamera (_cameraRotationX);

		//Weapon Sway
		if(weaponManager.GetCurrentGraphics() != null)
			weaponManager.GetCurrentGraphics().SetMovement(_xRot* lookSensitivity, _yRot* lookSensitivity);

		//Thruster
		Vector3 _thrusterForce = Vector3.zero;

		if (Input.GetButton ("Jump") && thrusterFuelAmount > 0f) {
			thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.deltaTime;
			if (thrusterFuelAmount >= 0.01f) {
				_thrusterForce = Vector3.up * thrusterForce;
				SetJointSettings (0f);
			}
		} else {
			thrusterFuelAmount += thrusterFuelRegenSpeed * Time.deltaTime;
			SetJointSettings (jointSpring);
		}
		thrusterFuelAmount = Mathf.Clamp01 (thrusterFuelAmount);
		motor.ApplyThruster (_thrusterForce);
	}

	private void SetJointSettings(float _jointSpring){
		joint.yDrive = new JointDrive {
			positionSpring = _jointSpring,
			maximumForce = jointMaxForce
		};
	}


}