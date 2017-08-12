using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class WeaponSwitch : NetworkBehaviour {

	private WeaponManager wm;
	private PlayerInput input;
	private int currentWeaponIndex = 0;
	void Start () {
		wm = GetComponent<WeaponManager> ();
		input = GetComponent<PlayerInput> ();
	}

	void Update(){
		if (PauseMenu.IsOn)
			return;
		if (Input.GetAxis("Mouse ScrollWheel")> 0) {
			currentWeaponIndex++;
			if (currentWeaponIndex >= wm.weapons.Length)
				currentWeaponIndex = 0;
			SwitchWeapon (currentWeaponIndex);
		} else if (Input.GetAxis("Mouse ScrollWheel") < 0) {
			currentWeaponIndex--;
			if (currentWeaponIndex < 0)
				currentWeaponIndex = wm.weapons.Length-1;
			SwitchWeapon (currentWeaponIndex);
		}
	}

	[Client]
	void SwitchWeapon(int index){
		wm.DoSwitch (index);
	}
}
