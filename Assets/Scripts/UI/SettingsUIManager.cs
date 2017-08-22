﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUIManager : MonoBehaviour {

	[SerializeField]
	private Text MouseSensitivityText;

	[SerializeField]
	private Slider MouseSensitivity;

	[SerializeField]
	private Dropdown PlayerIconOptions;

	List<Sprite> avatars;

	public Sprite currentPlayerIcon;

	public bool isPlayerUI = false;

	void Start(){
		avatars = new List<Sprite>();
		MouseSensitivity.onValueChanged.AddListener(SaveMouseSensitivity);
		MouseSensitivity.value = GameSettings.instance.GetMouseSensitivity ();
		if (!isPlayerUI) {
			PopulatePlayerIconDropdown ();
			if (PlayerPrefs.HasKey ("UserAvatarOption"))
				PlayerIconOptions.value = PlayerPrefs.GetInt ("UserAvatarOption");
		}
	}

	void Update () {
		MouseSensitivityText.text = GameSettings.instance.GetMouseSensitivity ().ToString ();
	}

	void SaveMouseSensitivity(float value){
		GameSettings.instance.SaveMouseSensitivity (value);
	}

	void PopulatePlayerIconDropdown(){
		PlayerIconOptions.options.Clear ();

		Object[] avatarsObj = Resources.LoadAll ("Avatars",typeof(Sprite));
		for (int i = 0; i < avatarsObj.Length; i++) {
			avatars.Add ((Sprite)avatarsObj [i]);
		}
		PlayerIconOptions.AddOptions (avatars);
		List<Dropdown.OptionData> options = PlayerIconOptions.options;
		for (int i = 0; i < options.Count; i++) {
			options [i].image = avatars [i];
		}
	}

	public void SelectPlayerIcon(int _option){
		currentPlayerIcon = avatars [_option];
		PlayerPrefs.SetInt ("UserAvatarOption", _option);
		PlayerPrefs.SetString ("UserAvatarName", currentPlayerIcon.name);
	}
}
