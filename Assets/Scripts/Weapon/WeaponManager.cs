using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
public class WeaponManager : NetworkBehaviour {

	//Weapon layer for first person weapon camera
	[SerializeField]
	private string weaponLayerName = "Weapon";

	//Array with all weapons
	public PlayerWeapon[] weapons;

	//Current index of the weapon the player is holding
	//SyncVar so we can set properly on the player model over the network for other players
	[SyncVar]
	public int currentWeaponIndex;

	//Reference to objects that we want to set to be drawn only by the weapon camera (setting the layer to the weapon layer)
	[SerializeField]
	GameObject[] WeaponCamObjects;

	//References of the WeaponIK script for first person view (local player) and player model (remote players over the network), respectively
	[SerializeField]
	WeaponIK FPSViewWeaponIK;
	[SerializeField]
	WeaponIK NetworkWeaponIK;


	//Currently held weapon
	private PlayerWeapon currentWeapon;
	//WeaponGraphics script of the currently held weapon
	private WeaponGraphics currentGraphics;
	//PlayerShoot script for switching weapon
	private PlayerShoot playerShoot;

	//Reference to the animator to be used, which is the first person view model for the local player, and the network player model for remote players
	[SerializeField]
	private Animator animator;

	//Check if player is currently reloading. Public so the WeaponIK script can access it
	[SyncVar]
	public bool isReloading = false;

	void Start () {
		//Get all the weapons' transforms in order to set the appropriate parent, which will be the right hand bone of either the first person view or the network player model
		Transform[] weaponParents = new Transform[weapons.Length];
		for (int i = 0; i < weapons.Length; i++) {
			weaponParents [i] = weapons [i].graphics.transform;
		}
		playerShoot = GetComponent<PlayerShoot> ();
		/*
		 *	If it's the local player, the current weapon defaults to the first in the array of weapons, and the animator is the prefab parent's, because it will be controlled by the firt player view model
		 *	Then set the correct layer for the objects we want to be drawn only by the weapon camera, and the WeaponManager reference for the WeaponIK script as this.
		 *	Finally send the DoSwitch command so we properly "switch" to the default weapon
		 */
		if (isLocalPlayer) {
			currentWeaponIndex = 0;
			animator = GetComponent<Animator> ();
			for (int i = 0; i < weapons.Length; i++) {
				weapons [i].bullets = weapons [i].maxBullets;
			}
			for (int i = 0; i < WeaponCamObjects.Length; i++) {
				Util.SetLayerRecursively (WeaponCamObjects [i], LayerMask.NameToLayer (weaponLayerName));
			}
			DoSwitch (0);
			FPSViewWeaponIK.weaponManager = this;
			FPSViewWeaponIK.SetWeaponTransformParent (weaponParents);
		} 
		/*
		 *	If it's not the local player, set the animator to be the network player model's, and set the WeaponManager reference for the WeaponIK script as this
		 *	Then correct the positioning of the weapons' respective pivots to match the network player model
		 *	Finally call the SwitchWeapon method so the network player model's currently held weapon matches the respective player's
		 */
		else {
			animator = NetworkWeaponIK.gameObject.GetComponent<Animator> ();
			NetworkWeaponIK.weaponManager = this;
			for (int i = 0; i < weapons.Length; i++) {
				weapons [i].pivot.localPosition += weapons [i].PivotCorrectionOffset;
				weapons [i].pivot.localRotation = Quaternion.Euler(weapons [i].pivot.localRotation.eulerAngles +  weapons [i].RotationCorrectionOffset);
			}
			SwitchWeapon (currentWeaponIndex);
			NetworkWeaponIK.SetWeaponTransformParent (weaponParents);
		}

	}


	/** Return the currently held weapon **/
	public PlayerWeapon GetCurrentWeapon(){
		return currentWeapon;
	}

	/** Return the currently active WeaponGraphics behaviour**/
	public WeaponGraphics GetCurrentGraphics(){
		return currentGraphics;
	}

	/** Reload method called by the PlayerShoot script **/
	public void Reload(){
		if (isReloading)
			return;
		
		CmdOnReload ();

	}

	/** Reloading Coroutine **/
	private IEnumerator Reload_Coroutine(){
		isReloading = true;
		//Start the reloading animation
		animator.SetBool ("Reloading", true);

		//When the animation ends, the "Reloading" bool is reset to false by the ReloadEnd animator behaviour
		while (animator.GetBool ("Reloading"))
			yield return null;

		//Reset the amount of bullets
		currentWeapon.bullets = currentWeapon.maxBullets;

	}
		
	/** Reload server Command **/
	[Command]
	void CmdOnReload(){
		RpcOnReload ();
	}

	/** Reload client rpc call **/
	[ClientRpc]
	void RpcOnReload(){
		StartCoroutine (Reload_Coroutine ());
		animator.SetTrigger ("Reload");
	}

	/** Weapon switch server command **/
	[Command]
	void CmdOnSwitch(int index){
		RpcSwitchWeapon (index);
	}

	/** Weapon switch client rpc call **/
	[ClientRpc]
	void RpcSwitchWeapon(int index){
		SwitchWeapon (index);
	}

	/** Calls the Weapon Switch server command**/
	public void DoSwitch(int index){
		CmdOnSwitch (index);
	}

	/** Weapon Switching method **/
	void SwitchWeapon(int index){
		//Change the currently held weapon index to new one
		currentWeaponIndex = index;

		//Check if it's not the first time the method is being called to avoid null reference errors
		//Deactivate the previously held weapon so it's not drawn
		if (currentWeapon != null && currentGraphics != null) {
			Util.SetLayerRecursively (currentGraphics.gameObject, LayerMask.NameToLayer ("Default"));
			currentGraphics.gameObject.SetActive (false);
		}

		//Set the new weapon's behaviours
		currentWeapon = weapons [index];
		currentGraphics = currentWeapon.graphics.GetComponent<WeaponGraphics> ();
		currentGraphics.gameObject.SetActive (true);

		/*
		 * Call the SwitchWeapon method on the appropriate WeaponIK behaviour, depending on if
		 * it's the local player or not
		 */
		if (isLocalPlayer) {
			Util.SetLayerRecursively (currentGraphics.gameObject, LayerMask.NameToLayer (weaponLayerName));
			FPSViewWeaponIK.SwitchWeapon (currentWeapon);
			//Set up the correct transform for the weapon sway
			currentGraphics.SetPosition (currentWeapon.LocalIKRightHandHold.parent);
		} else {
			NetworkWeaponIK.SwitchWeapon (currentWeapon);
		}
		//Set up the new weapon on the PlayerShoot component
		playerShoot.SwitchWeapon (currentWeapon);
	}



}
