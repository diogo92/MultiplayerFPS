using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

	[SerializeField]
	RectTransform thrusterFuelFill;

	[SerializeField]
	RectTransform healthbarFill;

	[SerializeField]
	Text ammoText;

	[SerializeField]
	GameObject pauseMenu;

	[SerializeField]
	GameObject scoreBoard;

	private PlayerManager player;
	private PlayerController controller;
	private WeaponManager weaponManager;

	public void SetPlayer(PlayerManager _player){
		player = _player;
		controller = player.GetComponent<PlayerController> ();
		weaponManager = player.GetComponent<WeaponManager> ();
	}

	void SetFuelAmount(float _amount){
		
		thrusterFuelFill.localScale = new Vector3 (1f, _amount, 1f);
	}

	void SetHealthAmount(float _amount){
		healthbarFill.localScale = new Vector3 (1f, _amount, 1f);
	}

	void SetAmmoAmount(int _amount){
		ammoText.text = _amount.ToString ();
	}

	void Start(){
		PauseMenu.IsOn = false;
	}

	void Update(){
		SetFuelAmount(controller.GetThrusterFuelAmount ());
		SetHealthAmount (player.GetHealthAmount ());
		if(weaponManager.GetCurrentWeapon() != null)
			SetAmmoAmount (weaponManager.GetCurrentWeapon ().bullets);
		if (Input.GetKeyDown (KeyCode.Escape)) {
			TogglePauseMenu ();
		}
		if (Input.GetKeyDown (KeyCode.Tab)) {
			scoreBoard.SetActive (true);
		} else if (Input.GetKeyUp (KeyCode.Tab)) {
			scoreBoard.SetActive (false);
		}
	}

	public void TogglePauseMenu(){
		pauseMenu.SetActive (!pauseMenu.activeSelf);
		PauseMenu.IsOn = pauseMenu.activeSelf;
	}
}
