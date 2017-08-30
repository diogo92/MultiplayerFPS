using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

	[SerializeField]
	Image thrusterFuelFill;

	[SerializeField]
	Image healthbarFill;

	[SerializeField]
	Text ammoText;

	[SerializeField]
	GameObject pauseMenu;

	[SerializeField]
	GameObject scoreBoard;

	[SerializeField]
	Text nameText;

	[SerializeField]
	Image playerImage;

	private PlayerManager player;
	private PlayerController controller;
	private WeaponManager weaponManager;

	public void SetPlayer(PlayerManager _player){
		player = _player;
		controller = player.GetComponent<PlayerController> ();
		weaponManager = player.GetComponent<WeaponManager> ();
		SetPlayerName (player.name);
		if (PlayerPrefs.HasKey ("UserAvatarName"))
			playerImage.sprite = Util.GetCurrentPlayerAvatar( (PlayerPrefs.GetString("UserAvatarName")));
			
	}

	void SetPlayerName(string _name){
		nameText.text = _name;
	}

	void SetFuelAmount(float _amount){
		
		thrusterFuelFill.fillAmount =_amount;
	}

	void SetHealthAmount(float _amount){
		healthbarFill.fillAmount = _amount;
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
