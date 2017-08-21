using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUIManager : MonoBehaviour {

	public Text MouseSensitivityText;
	public Slider MouseSensitivity;

	void Start(){
		MouseSensitivity.onValueChanged.AddListener(SaveMouseSensitivity);
		MouseSensitivity.value = GameSettings.instance.GetMouseSensitivity ();
	}

	void Update () {
		MouseSensitivityText.text = GameSettings.instance.GetMouseSensitivity ().ToString ();
	}

	void SaveMouseSensitivity(float value){
		GameSettings.instance.SaveMouseSensitivity (value);
	}
}
