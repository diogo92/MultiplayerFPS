using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
public class PlayerStats : MonoBehaviour {

	public Text killCount;
	public Text deathCount;

	void Start () {
		/*if(UserAccountManager.IsLoggedIn)
			UserAccountManager.instance.GetData (OnReceivedData);*/
		if (DatabaseHandler.IsLoggedIn) {
			StartCoroutine(DatabaseHandler.instance.GetKillCount (OnReceivedDataKills));
			StartCoroutine(DatabaseHandler.instance.GetDeathCount (OnReceivedDataDeaths));
		}
	}


	void OnReceivedDataKills (string data) {
		if (killCount == null)
			return;
		killCount.text = int.Parse(data) + " KILLS";
	}

	void OnReceivedDataDeaths (string data) {
		if (deathCount == null)
			return;
		deathCount.text = int.Parse(data) + " DEATHS";
	}

}
