using UnityEngine.Networking;
using UnityEngine;
using System.Collections;

/*
 * Player shooting handler
 * Handles Fire Input, Raycast shooting and Aiming
 */
[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour {
	//Tag to identify a player game object
	private const string PLAYER_TAG = "Player";

	[SerializeField]
	private string weaponLayerName = "Weapon";

	/* Component caching */
	private PlayerMotor motor;
	private PlayerWeapon currentWeapon;
	private WeaponManager weaponManager;
	private Animator anim;
	private PlayerSound playerSound;

	//Reference to the animator of the player model (FPS view model if is local player, or full player model if not)
	[SerializeField]
	private Animator weaponIKAnim;

	//Parent of the IK targets for the hands of the currect active weapon
	private Transform CurrentWeaponIKHolderParent;

	//Player main camera
	[SerializeField]
	private Camera cam;

	//Mask for raycast shooting
	[SerializeField]
	private LayerMask mask;

	//Check if it is the first shot with a weapon after a certain time
	private bool isFirstShot = true;

	//Check if player is shooting
	private bool isShooting = false;

	//Check if player is aiming with a scoped weapon
	private bool isScoping = false;

	/* Original transform components of the parent of the IK targets for the hands of the currect active weapon */
	private Vector3 originalIKparentPosition;
	private Quaternion originalIKparentRotation;

	/* Debug values for adjusting the positioning of the weapon when aiming */
	public bool debugADS = false;
	public bool debugADSHold = false;

	//Check if player has started aiming, to enable or disable crosshair
	bool ADSStarted = false;

	//Syncvar to send the aiming state value over the network so other players see the full player model aiming as well
	[SyncVar]
	bool isADS = false;

	/* Recoil values */
	//Speed of the recoil
	public float RecoilSmoothAmount;

	//Amount of recoil to add
	float targetVerticalRecoil;
	float targetHorizontalRecoil;

	//Current amount of recoil
	float currentVerticalRecoil;
	float currentHorizontalRecoil;

	//Total acumulated recoil
	float totalVerticalAmount;
	float totalHorizontalAmount;

	bool isRecoiling= false;

	void Start () {
		if (cam == null) {
			Debug.LogError ("PlayerShoot: No camera referenced!");
			this.enabled = false;
		}
		motor = GetComponent<PlayerMotor> ();
		anim = GetComponent<Animator> ();
		weaponManager = GetComponent<WeaponManager> ();
		playerSound = GetComponent<PlayerSound> ();
		currentWeapon = weaponManager.GetCurrentWeapon ();
	}

	void Update () {
		HandleRecoil ();

		//Disable the animator component if player is not shooting
		if (weaponIKAnim.GetCurrentAnimatorStateInfo (0).IsName ("Idle"))
			weaponIKAnim.enabled = false;

		//Always handle the aiming when on remote client, so the model updates properly
		if (!isLocalPlayer) {
			HandleADS ();
			return;
		}

		if (currentWeapon == null)
			return;

		//Stop aiming when paused
		if (PauseMenu.IsOn) {
			CmdUpdateADS(false);
			HandleADS ();
			return;
		}
			
		//Stop shooting when reloading
		if (weaponManager.isReloading) {
			isShooting = false;
			CancelInvoke ("Shoot");
		}

		HandleInput ();

		//Update the crosshair
		if (isShooting) {
			CrosshairManager.instance.currentWeaponCrosshairSize = Mathf.Lerp (CrosshairManager.instance.currentWeaponCrosshairSize, currentWeapon.crosshairShootingSize, Time.deltaTime*2f);
		} else {
			CrosshairManager.instance.currentWeaponCrosshairSize = Mathf.Lerp (CrosshairManager.instance.currentWeaponCrosshairSize, currentWeapon.crosshairNormalSize,Time.deltaTime*2f);
		}

	}
		

	void HandleInput(){
		//Allow manual reload when weapon has less bullets than the max
		if (currentWeapon.bullets < currentWeapon.maxBullets) {
			if (Input.GetKeyDown(KeyCode.R)) {
				weaponManager.Reload ();
				return;
			}
		}

		/* Aiming Down Sight*/
		if ((Input.GetButton ("Fire2") || debugADS) && !weaponManager.isReloading) {
			//Change the isADS variable over the network
			CmdUpdateADS (true);
			//If weapon is scoped, activate the UI skin 
			if (currentWeapon.hasScope && !isScoping) {
				isScoping = true;
				Invoke ("SetScoping", 0.5f);
			}
			AimDownSight ();
		} else {
			/* Stop aiming */
			if (Input.GetButtonUp ("Fire2")) {
				CmdUpdateADS (false);
				isScoping = false;
				CrosshairManager.instance.Scope (false);
				CancelInvoke ("SetScoping");
			}
			UndoADS ();
		}


		if (currentWeapon.fireRate <= 0) {
			if (Input.GetButtonDown("Fire1")) {
				Shoot ();
			}
		} else {
			if (Input.GetButtonDown("Fire1")) {
				InvokeRepeating ("Shoot", 0, 1f / currentWeapon.fireRate);
			} else if(Input.GetButtonUp("Fire1")){
				CancelInvoke ("Shoot");
				StartCoroutine (ResetFireAccuracy ());
				isShooting = false;
			}
		}
	}

	//After a certain time, set isFirstShot to true, so the weapon doesen't start spreading right after starting to shoot
	IEnumerator ResetFireAccuracy(){
		yield return new WaitForSeconds (1.5f);
		isFirstShot = true;
	}

	//Switch weapon;
	public void SwitchWeapon(PlayerWeapon _newWeapon){
		//Reset the transform of the parent of the IK targets for the hands of the currect active weapon
		if (CurrentWeaponIKHolderParent != null && currentWeapon != null) {
			CurrentWeaponIKHolderParent.localPosition = originalIKparentPosition;
			currentWeapon.pivot.localRotation = originalIKparentRotation;
		}
		//Disable the UI skin of the scope
		if (isScoping) {
			isScoping = false;
			CrosshairManager.instance.Scope (false);
			CancelInvoke ("SetScoping");
		}
			
		/* Set new weapon values */
		currentWeapon = weaponManager.GetCurrentWeapon ();
		CurrentWeaponIKHolderParent = currentWeapon.LocalIKRightHandHold.parent;
		originalIKparentPosition = CurrentWeaponIKHolderParent.localPosition;
		originalIKparentRotation = currentWeapon.pivot.localRotation;
		if(isLocalPlayer)
			CrosshairManager.instance.currentWeaponCrosshairSize = currentWeapon.crosshairNormalSize;
	}



	#region shooting
	//Called on server when a player shoots
	[Command]
	void CmdOnShoot(){
		RpcDoShootEffect ();
	}
	//Called on all clients to do shoot effect
	[ClientRpc]
	void RpcDoShootEffect(){
		weaponManager.GetCurrentGraphics ().muzzleFlash.Play ();
		playerSound.PlayShootSound ();
	}


	//Called on server when something is hit
	[Command]
	void CmdOnHit(Vector3 _pos, Vector3 _normal){
		RpcDoHitEffect (_pos, _normal);
	}

	//Called on all clients to do hit effect
	[ClientRpc]
	void RpcDoHitEffect(Vector3 _pos, Vector3 _normal){
		GameObject hitEffect= (GameObject)Instantiate (weaponManager.GetCurrentGraphics ().hitEffectPrefab, _pos, Quaternion.LookRotation (_normal));
		Destroy (hitEffect, 2f);
	}

	[Client]
	void Shoot(){
		isShooting = true;
		if (!isLocalPlayer || weaponManager.isReloading) {
			isShooting = false;
			return;
		}

		if (currentWeapon.bullets <= 0) {
			isShooting = false;
			weaponManager.Reload ();
			return;
		}
		currentWeapon.bullets--;
		//Enable the animator to do the weapon shooting animations
		weaponIKAnim.enabled = true;
		weaponIKAnim.CrossFadeInFixedTime (currentWeapon.name,0.01f);
		/* Add recoil */
		if (!isRecoiling) {
			targetVerticalRecoil = Random.Range (0.1f, currentWeapon.VerticalRecoil);
			targetHorizontalRecoil = Random.Range (0, currentWeapon.HorizontalRecoil);
			StartCoroutine (Recoil ());
		}
		CmdOnShoot ();
		DoRaycast ();
		if (currentWeapon.bullets <= 0)
			weaponManager.Reload ();
	}

	/* Command called on the server upon hitting a player */
	[Command]
	void CmdPlayerShot(string _playerID, int _damage, string _sourceID){
		Debug.Log (_playerID + " has been shot");

		PlayerManager _player = GameManager.GetPlayer (_playerID);
		_player.RpcTakeDamage (_damage, _sourceID);
	}

	/* Shooting raycast */
	void DoRaycast(){
		/* Crosshair setting */
		float crosshairSize = CrosshairManager.instance.GetCrosshairSize ();
		//Actual raycast target position
		Vector3 pointInCrosshair;
		//For a first shot after a certain time, increase accuracy  
		if (isFirstShot) {
			pointInCrosshair = new Vector3 (Screen.width / 2f, Screen.height / 2f, 0);
			isFirstShot = false;
		}
		//If not, do a random spread
		else
			pointInCrosshair = new Vector3 (Screen.width / 2f + Random.Range (-crosshairSize / 1.5f, crosshairSize / 1.5f), Screen.height/2f + Random.Range (-crosshairSize / 1.5f, crosshairSize / 1.5f), 0);
		
		RaycastHit hit;
		Ray ray = cam.ScreenPointToRay (pointInCrosshair);
		if (Physics.Raycast(ray,out hit, currentWeapon.range,mask)) {
			if (hit.collider.tag == PLAYER_TAG) {
				CmdPlayerShot (hit.collider.name,currentWeapon.damage,transform.name);
			}
			CmdOnHit (hit.point, hit.normal);
		}
	}

	/* Recoil Coroutine, so the recoil only increments after the previous amount has been reached and it starts being undone, for a better effect */
	IEnumerator Recoil(){
		isRecoiling = true;
		while (currentVerticalRecoil < targetVerticalRecoil) {
			currentVerticalRecoil = Mathf.Lerp (currentVerticalRecoil, targetVerticalRecoil, Time.deltaTime*RecoilSmoothAmount);
			totalVerticalAmount += currentVerticalRecoil;

			currentHorizontalRecoil = Mathf.Lerp (currentHorizontalRecoil, targetHorizontalRecoil, Time.deltaTime*RecoilSmoothAmount);
			totalHorizontalAmount += currentHorizontalRecoil;
			yield return null;
		}
		isRecoiling = false;
	}

	/* Recoil Handling */
	void HandleRecoil(){
		//If has reached the target amount, start undoing the recoil
		if (!isRecoiling) {
			targetHorizontalRecoil = Mathf.Lerp (targetHorizontalRecoil, 0, Time.deltaTime*RecoilSmoothAmount);
			targetVerticalRecoil = Mathf.Lerp (targetVerticalRecoil, 0, Time.deltaTime*RecoilSmoothAmount);

			currentVerticalRecoil = Mathf.Lerp (totalVerticalAmount, 0, Time.deltaTime*RecoilSmoothAmount);
			currentHorizontalRecoil = Mathf.Lerp (totalHorizontalAmount, 0, Time.deltaTime*RecoilSmoothAmount);

			totalVerticalAmount -= currentVerticalRecoil;
			totalHorizontalAmount -= currentHorizontalRecoil;

			currentVerticalRecoil = -currentVerticalRecoil;
			currentHorizontalRecoil = -currentHorizontalRecoil;
		}

		//Camera and player rotation has to be changed on the PlayerMotor component
		motor.VerticalRecoil(currentVerticalRecoil);
		motor.HorizontalRecoil(currentHorizontalRecoil);
	}

	#endregion

	#region AimDownSight

	/* Update the variable over the network */
	[Command]
	void CmdUpdateADS(bool _isADS){
		isADS = _isADS;
	}
		
	[Command]
	void CmdHandleADS(){
		RpcHandleADS ();
	}

	[ClientRpc]
	void RpcHandleADS(){
			HandleADS ();
	}

	void HandleADS(){
		if(!isLocalPlayer){
			if (isADS || debugADS)
				AimDownSight ();
			else {
				if (!debugADS)
					UndoADS ();
			}
		}
	}

	/* Aiming Down Sight */ 
	void AimDownSight(){
		//Disable the crosshair
		if (!ADSStarted && isLocalPlayer) {
			ADSStarted = true;
			CrosshairManager.instance.SetADS (true);
		}
		//Set the animator to a freezing state so the weapon stops moving
		if (isLocalPlayer) {
			anim.Play ("Locomotion Aim", 0, 0f);
			anim.Play ("Aim", 1, 0f);
			anim.SetFloat ("Forward", 0);
			anim.SetFloat("Sideways",0);
		}
		//Start the actual aiming, if debug not wanted for keeping the weapon in position
		if (!debugADSHold) {
			/* Move the parent of the IK targets for the hands of the currect active weapon to the desired position, that depends if it is the local player client or a remote client*/
			if (isLocalPlayer) {
				CurrentWeaponIKHolderParent.localPosition = Vector3.Slerp (CurrentWeaponIKHolderParent.localPosition, currentWeapon.ADSPosition, Time.deltaTime * currentWeapon.ADSSpeed);
				currentWeapon.pivot.localRotation = Quaternion.Slerp (currentWeapon.pivot.localRotation, Quaternion.Euler (currentWeapon.ADSRotation), Time.deltaTime * currentWeapon.ADSSpeed);
				Camera.main.fieldOfView = Mathf.Lerp (Camera.main.fieldOfView, currentWeapon.ADSFOV, Time.deltaTime * currentWeapon.ADSSpeed);
			} else {
				CurrentWeaponIKHolderParent.localPosition = Vector3.Slerp (CurrentWeaponIKHolderParent.localPosition, currentWeapon.ADSRemotePosition, Time.deltaTime * currentWeapon.ADSSpeed);
				currentWeapon.pivot.localRotation = Quaternion.Slerp (currentWeapon.pivot.localRotation, Quaternion.Euler (currentWeapon.ADSRemoteRotation), Time.deltaTime * currentWeapon.ADSSpeed);
			}
		}
	}

	/* Undo the aiming, it's the opposite of what is done in AimDownSight() */
	void UndoADS(){
		if (debugADS)
			return;
		if (ADSStarted) {
			ADSStarted = false;
			CrosshairManager.instance.SetADS (false);
		}
		CurrentWeaponIKHolderParent.localPosition = Vector3.Slerp (CurrentWeaponIKHolderParent.localPosition, originalIKparentPosition, Time.deltaTime * currentWeapon.ADSSpeed);
		currentWeapon.pivot.localRotation = Quaternion.Slerp (currentWeapon.pivot.localRotation, originalIKparentRotation, Time.deltaTime * currentWeapon.ADSSpeed);
		if(isLocalPlayer)
			Camera.main.fieldOfView = Mathf.Lerp (Camera.main.fieldOfView, 70, Time.deltaTime * currentWeapon.ADSSpeed);
	}

	void SetScoping(){
		CrosshairManager.instance.Scope (true);
	}

	#endregion
}
