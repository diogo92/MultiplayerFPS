using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Crosshair handler
 */
public class CrosshairManager : MonoBehaviour {
	//Reference to each crosshair graphic part
	[SerializeField]
	private RectTransform[] crosshairParts;

	//UI skin overlay for the scope
	[SerializeField]
	private GameObject scopeOverlay;

	private Camera weaponCamera;

	Rigidbody player;

	public static CrosshairManager instance;

	//Speed at which the crosshair increases size
	public float crosshairSpeed = 8f;
	//Current amount of weapon spread calculated on the PlayerShoot component
	public float currentWeaponCrosshairSize;
	//Player current velocity
	public float playerSpeed;

	//Current size of the crosshair
	float crosshairSize;
	//Amount to increase the crosshair depen
	float startingSize;

	//Check if player is aiming
	bool ADS = false;

	void Awake(){
		instance = this;
		startingSize = crosshairParts [0].position.y;
	}

	public void Setup(Camera _weaponCamera){
		weaponCamera = _weaponCamera;
	}

	void Update () {
		HandleCrosshair ();	
	}

	void HandleCrosshair(){
		//If the player is aiming the crosshair is not drawn at all and so, does not need to update
		if (!ADS) {
			//Increase to target amount
			crosshairSize = startingSize * currentWeaponCrosshairSize;
			//If player is walking slowly or is still, decrease the crosshair size
			if (playerSpeed <= 0.5f)
				crosshairSize /= 2;

			/* Move all crosshair parts to their respective positions */
			crosshairParts [0].localPosition = Vector3.Slerp (crosshairParts [0].localPosition, new Vector3 (0f, crosshairSize, 0f), Time.deltaTime * crosshairSpeed);
			crosshairParts [1].localPosition = Vector3.Slerp (crosshairParts [1].localPosition, new Vector3 (crosshairSize, 0f, 0f), Time.deltaTime * crosshairSpeed);
			crosshairParts [2].localPosition = Vector3.Slerp (crosshairParts [2].localPosition, new Vector3 (-crosshairSize, 0f, 0f), Time.deltaTime * crosshairSpeed);
			crosshairParts [3].localPosition = Vector3.Slerp (crosshairParts [3].localPosition, new Vector3 (0f, -crosshairSize, 0f), Time.deltaTime * crosshairSpeed);
		}
	}


	public float GetCrosshairSize(){
		return crosshairSize;
	}

	/** Set if player is aiming or not **/
	public void SetADS(bool _ADS){
		ADS = _ADS;
		if (ADS) {
			//Disable the crosshair parts if player is aiming
			for (int i = 0; i < crosshairParts.Length; i++)
				crosshairParts [i].gameObject.SetActive (false);
		} else {
			//Enable crosshair parts otherwise
			for (int i = 0; i < crosshairParts.Length; i++)
				crosshairParts [i].gameObject.SetActive (true);
		}
	}

	/** Set if the currently held weapon is scoped or not **/
	public void Scope(bool _scopeActive){
		if (_scopeActive) {
			//Disable the weapon camera so the weapon isn't drawn
			weaponCamera.enabled = false;
			//Enable the UI overlay with the scope skin
			scopeOverlay.SetActive (true);
		} else if (!_scopeActive) {
			//Reenable the weapon camera
			weaponCamera.enabled = true;
			//Disable the UI overlay with the scope skin
			scopeOverlay.SetActive (false);

		}

	}
}
