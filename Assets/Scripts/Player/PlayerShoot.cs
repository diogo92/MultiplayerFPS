using UnityEngine.Networking;
using UnityEngine;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour {
	private const string PLAYER_TAG = "Player";
	[SerializeField]
	private string weaponLayerName = "Weapon";

	PlayerMotor motor;
	private PlayerWeapon currentWeapon;
	private WeaponManager weaponManager;
	private Animator anim;
	[SerializeField]
	private Animator weaponIKAnim;

	private Transform CurrentWeaponIKHolderParent;
	[SerializeField]
	private Camera cam;

	[SerializeField]
	private LayerMask mask;

	private bool isShooting = false;
	private bool isScoping = false;

	private Vector3 originalIKparentPosition;
	private Quaternion originalIKparentRotation;

	public bool debugADS = false;
	public bool debugADSHold = false;

	bool ADSStarted = false;

	[SyncVar]
	bool isADS = false;

	/* Recoil */
	float targetVerticalRecoil;
	float currentVerticalRecoil;
	float totalVerticalAmount;


	float targetHorizontalRecoil;
	float currentHorizontalRecoil;


	void Start () {
		if (cam == null) {
			Debug.LogError ("PlayerShoot: No camera referenced!");
			this.enabled = false;
		}
		motor = GetComponent<PlayerMotor> ();
		anim = GetComponent<Animator> ();
		weaponManager = GetComponent<WeaponManager> ();
		currentWeapon = weaponManager.GetCurrentWeapon ();
	}
	
	// Update is called once per frame
	void Update () {
		HandleRecoil ();
		if (weaponIKAnim.GetCurrentAnimatorStateInfo (0).IsName ("Idle"))
			weaponIKAnim.enabled = false;
		if (!isLocalPlayer) {
			HandleADS ();
			return;
		}
		if (currentWeapon == null)
			return;
		if (PauseMenu.IsOn) {
			CmdUpdateADS(false);
			HandleADS ();
			return;
		}

		if (weaponManager.isReloading) {
			isShooting = false;
			CancelInvoke ("Shoot");
		}

		if (currentWeapon.bullets < currentWeapon.maxBullets) {
			if (Input.GetKeyDown(KeyCode.R)) {
				weaponManager.Reload ();
				return;
			}
		}
		if ((Input.GetButton ("Fire2") || debugADS) && !weaponManager.isReloading) {
			CmdUpdateADS (true);
			if (currentWeapon.hasScope && !isScoping) {
				isScoping = true;
				Invoke ("SetScoping", 0.5f);
			}
			AimDownSight ();
		} else {
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
				isShooting = false;
			}
		}
		if (isShooting) {
			CrosshairManager.instance.currentWeaponCrosshairSize = Mathf.Lerp (CrosshairManager.instance.currentWeaponCrosshairSize, currentWeapon.crosshairShootingSize, Time.deltaTime*2f);
		} else {
			CrosshairManager.instance.currentWeaponCrosshairSize = Mathf.Lerp (CrosshairManager.instance.currentWeaponCrosshairSize, currentWeapon.crosshairNormalSize,Time.deltaTime*2f);
		}

	}
		


	//Switch weapon;
	public void SwitchWeapon(PlayerWeapon _newWeapon){
		if (CurrentWeaponIKHolderParent != null && currentWeapon != null) {
			CurrentWeaponIKHolderParent.localPosition = originalIKparentPosition;
			currentWeapon.pivot.localRotation = originalIKparentRotation;
		}
		if (isScoping) {
			isScoping = false;
			CrosshairManager.instance.Scope (false);
			CancelInvoke ("SetScoping");
		}
			
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
		weaponIKAnim.enabled = true;
		weaponIKAnim.CrossFadeInFixedTime (currentWeapon.name,0.01f);
		targetVerticalRecoil = Random.Range (0.1f, currentWeapon.VerticalRecoil);
		targetHorizontalRecoil = Random.Range (-currentWeapon.HorizontalRecoil, currentWeapon.HorizontalRecoil);

		CmdOnShoot ();
		DoRaycast ();
		if (currentWeapon.bullets <= 0)
			weaponManager.Reload ();
	}

	[Command]
	void CmdPlayerShot(string _playerID, int _damage, string _sourceID){
		Debug.Log (_playerID + " has been shot");

		PlayerManager _player = GameManager.GetPlayer (_playerID);
		_player.RpcTakeDamage (_damage, _sourceID);
	}

	void DoRaycast(){
		float crosshairSize = CrosshairManager.instance.GetCrosshairSize ();
		Vector3 crosshairPosition = new Vector3 (Screen.width / 2f + Random.Range (-crosshairSize / 1.5f, crosshairSize / 1.5f), Screen.height/2f + Random.Range (-crosshairSize / 1.5f, crosshairSize / 1.5f), 0);
		RaycastHit hit;
		Ray ray = cam.ScreenPointToRay (crosshairPosition);
		if (Physics.Raycast(ray,out hit, currentWeapon.range,mask)) {
			if (hit.collider.tag == PLAYER_TAG) {
				CmdPlayerShot (hit.collider.name,currentWeapon.damage,transform.name);
			}
			CmdOnHit (hit.point, hit.normal);
		}
	}

	void HandleRecoil(){
		if (isShooting) {
			currentVerticalRecoil = Mathf.Lerp (currentVerticalRecoil, targetVerticalRecoil, Time.deltaTime * 50f);
			totalVerticalAmount += currentVerticalRecoil;

			currentHorizontalRecoil = Mathf.Lerp (currentVerticalRecoil, targetVerticalRecoil, Time.deltaTime);
		} else {
			targetHorizontalRecoil = Mathf.Lerp (targetHorizontalRecoil, 0, Time.deltaTime);
			targetVerticalRecoil = Mathf.Lerp (targetVerticalRecoil, 0, Time.deltaTime);
			currentVerticalRecoil = Mathf.Lerp (totalVerticalAmount, 0, Time.deltaTime*50f);
			totalVerticalAmount -= currentVerticalRecoil;
			currentVerticalRecoil = -currentVerticalRecoil;
			currentHorizontalRecoil = Mathf.Lerp (currentVerticalRecoil, targetVerticalRecoil, Time.deltaTime);
		}
		motor.VerticalRecoil(currentVerticalRecoil);
		//motor.HorizontalRecoil(currentHorizontalRecoil);
	}

	#endregion

	#region AimDownSight

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

	void AimDownSight(){
		if (!ADSStarted && isLocalPlayer) {
			ADSStarted = true;
			CrosshairManager.instance.SetADS (true);
		}
		if (isLocalPlayer) {
			anim.Play ("Locomotion Aim", 0, 0f);
			anim.Play ("Aim", 1, 0f);
			anim.SetFloat ("Forward", 0);
			anim.SetFloat("Sideways",0);
		}
		if (!debugADSHold) {
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
