using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Weapon graphical effects component
 */
public class WeaponGraphics : MonoBehaviour {

	//Reference to the muzzle flash
	public ParticleSystem muzzleFlash;
	//Reference to the bullet hit effect
	public GameObject hitEffectPrefab;

	/* Weapon sway variables */
	public float maxAmount = 0.06f;
	public float smoothAmount =6f;

	//Starting position of the hands' IK target parent
	private Vector3 ikHolderInitialPosition;

	/* Amount to move on each axis */
	public float movementX;
	public float movementY;

	//Reference to the hands' IK target parent
	private Transform ikHolder;

	//Check if everything is set properly on the SetPosition method
	bool isSet = false;

	/** Apply the sway on the LateUpdate method, so it takes proper effect after the animator update **/
	void LateUpdate () {
		//Weapon sway
		if (isSet) {
			//Clamp values to the maxAmount
			movementX = Mathf.Clamp (movementX, -maxAmount, maxAmount);
			movementY = Mathf.Clamp (movementY, -maxAmount, maxAmount);

			//Lerp to the target position
			Vector3 finalPostion = new Vector3 (movementX, movementY, 0);
			ikHolder.localPosition = Vector3.Lerp (ikHolder.localPosition, finalPostion + ikHolderInitialPosition, Time.deltaTime * smoothAmount);
		}
	}

	/** Update the axis sway movement amount **/
	public void SetMovement(float _movementX,float _movementY){
		movementX =  -_movementY;
		movementY =  -_movementX;
	}

	/** Initialize the hands' IK target parent after switching a weapon **/
	public void SetPosition(Transform _handIKTransform){
		ikHolder = _handIKTransform.parent;
		ikHolder.localPosition = Vector3.zero;
		ikHolderInitialPosition = ikHolder.localPosition;
		isSet = true;
	}


}
