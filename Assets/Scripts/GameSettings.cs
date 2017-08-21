using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour {

	public static GameSettings instance;
	int MouseSensitivity = 3;

	void Awake(){
		if (instance == null)
			instance = this;
		DontDestroyOnLoad (this);
		if(PlayerPrefs.HasKey("MouseSensitivity"))
			MouseSensitivity=PlayerPrefs.GetInt("MouseSensitivity");
	}

	public int GetMouseSensitivity(){
		return MouseSensitivity;
	}

	public void SaveMouseSensitivity(float _MouseSensitivity){
		MouseSensitivity = (int)_MouseSensitivity;
		PlayerPrefs.SetInt ("MouseSensitivity", MouseSensitivity);
	}
}
