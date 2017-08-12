using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
public class PlayerStats : MonoBehaviour {

	public Text killCount;
	public Text deathCount;

	void Start () {
		if(UserAccountManager.IsLoggedIn)
			UserAccountManager.instance.GetData (OnReceivedData);
	}


	void OnReceivedData (string data) {
		if (killCount == null || deathCount == null)
			return;
		killCount.text = DataTranslator.DataToKills(data) + " KILLS";
		deathCount.text = DataTranslator.DataToDeaths(data) + " DEATHS";
	}
}
