using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/*
 * Weapon switch input handling
 */
public class WeaponSwitch : NetworkBehaviour {

	//Reference to the weapon manager
	private WeaponManager wm;
	//Index of the currently held weapon
	private int currentWeaponIndex = 0;

	void Start () {
		wm = GetComponent<WeaponManager> ();
	}

	void Update(){
		//Disable switching if game is paused
		if (PauseMenu.IsOn)
			return;
		if (Input.GetAxis ("Mouse ScrollWheel") > 0) {
			currentWeaponIndex++;
			if (currentWeaponIndex >= wm.weapons.Length)
				currentWeaponIndex = 0;
			SwitchWeapon (currentWeaponIndex);
		} else if (Input.GetAxis ("Mouse ScrollWheel") < 0) {
			currentWeaponIndex--;
			if (currentWeaponIndex < 0)
				currentWeaponIndex = wm.weapons.Length - 1;
			SwitchWeapon (currentWeaponIndex);
		} else {
			if (Input.inputString != "") {
				char inputChar = Input.inputString.ToCharArray () [0];
				if (inputChar > '0' && inputChar <= '9') {
					int inputNum = int.Parse (inputChar.ToString ()) - 1;
					if (inputNum < wm.weapons.Length) {
						currentWeaponIndex = inputNum;
						SwitchWeapon (currentWeaponIndex);
					}
					
				}
			}
				
		}
	}

	//[Client]
	void SwitchWeapon(int index){
		//Call the switch method on the weapon manager
		wm.DoSwitch (index);
	}
}
