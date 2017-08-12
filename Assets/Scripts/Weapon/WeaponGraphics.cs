using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponGraphics : MonoBehaviour {

	public ParticleSystem muzzleFlash;
	public GameObject hitEffectPrefab;

	public float amount = 0.02f;
	public float maxAmount = 0.06f;
	public float smoothAmount =6f;

	private Vector3 ikHolderInitialPosition;

	public float movementX;
	public float movementY;

	private Transform ikHolder;
	bool isSet = false;

	void LateUpdate () {
		//Weapon sway
		if (isSet) {
			movementX = Mathf.Clamp (movementX, -maxAmount, maxAmount);
			movementY = Mathf.Clamp (movementY, -maxAmount, maxAmount);

			Vector3 finalPostion = new Vector3 (movementX, movementY, 0);
			ikHolder.localPosition = Vector3.Lerp (ikHolder.localPosition, finalPostion + ikHolderInitialPosition, Time.deltaTime * smoothAmount);
		}
	}

	public void SetMovement(float _movementX,float _movementY){
		movementX =  _movementY;
		movementY =  _movementX;
	}

	public void SetPosition(Transform _ikHolder){
		ikHolder = _ikHolder.parent;
		ikHolder.localPosition = Vector3.zero;
		ikHolderInitialPosition = ikHolder.localPosition;
		isSet = true;
	}


}
