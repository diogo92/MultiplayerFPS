using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairManager : MonoBehaviour {
	[SerializeField]
	private RectTransform[] crosshairParts;
	[SerializeField]
	private GameObject scopeOverlay;

	private Camera weaponCamera;

	Rigidbody player;

	public static CrosshairManager instance;

	public float crosshairSpeed = 8f;
	public float currentWeaponCrosshairSize;
	public float playerSpeed;


	float crosshairSize;
	float walkSize;

	bool ADS = false;
	void Awake(){
		instance = this;
		walkSize = crosshairParts [0].position.y;
	}

	public void Setup(Camera _weaponCamera){
		weaponCamera = _weaponCamera;
	}

	void Update () {
		if (!ADS) {
			crosshairSize = walkSize * currentWeaponCrosshairSize;
			if (playerSpeed <= 0.5f)
				crosshairSize /= 2;

			crosshairParts [0].localPosition = Vector3.Slerp (crosshairParts [0].localPosition, new Vector3 (0f, crosshairSize, 0f), Time.deltaTime * crosshairSpeed);
			crosshairParts [1].localPosition = Vector3.Slerp (crosshairParts [1].localPosition, new Vector3 (crosshairSize, 0f, 0f), Time.deltaTime * crosshairSpeed);
			crosshairParts [2].localPosition = Vector3.Slerp (crosshairParts [2].localPosition, new Vector3 (-crosshairSize, 0f, 0f), Time.deltaTime * crosshairSpeed);
			crosshairParts [3].localPosition = Vector3.Slerp (crosshairParts [3].localPosition, new Vector3 (0f, -crosshairSize, 0f), Time.deltaTime * crosshairSpeed);
		}
	}

	public float GetCrosshairSize(){
		return crosshairSize;
	}
	public void SetADS(bool _ADS){
		ADS = _ADS;
		if (ADS) {
			for (int i = 0; i < crosshairParts.Length; i++)
				crosshairParts [i].gameObject.SetActive (false);
		} else {
			for (int i = 0; i < crosshairParts.Length; i++)
				crosshairParts [i].gameObject.SetActive (true);
		}
	}
	public void Scope(bool _scopeActive){
		if (_scopeActive) {
			weaponCamera.enabled = false;
			scopeOverlay.SetActive (true);
		} else if (!_scopeActive) {
			weaponCamera.enabled = true;
			scopeOverlay.SetActive (false);

		}

	}
}
