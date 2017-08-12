using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerManager))]
public class PlayerScore : MonoBehaviour {

	PlayerManager player;

	int lastKills = 0;
	int lastDeaths = 0;

	void Start () {
		player = GetComponent<PlayerManager> ();
		StartCoroutine (SyncScoreLoop ());
	}

	void OnDestroy(){
		if(player != null)
			SyncNow ();
	}

	IEnumerator SyncScoreLoop() {
		while (true) {
			yield return new WaitForSeconds (5f);
			SyncNow ();
		}
	}

	void SyncNow(){
		if (UserAccountManager.IsLoggedIn) {
			UserAccountManager.instance.GetData (OnDataReceived);
		}
	}

	void OnDataReceived(string data){

		if (player.kills <= lastKills && player.deaths <= lastDeaths)
			return;

		int killsSinceLast = player.kills - lastKills;
		int deathsSinceLast = player.deaths - lastDeaths;

		if (player.kills == 0 && player.deaths == 0)
			return;
		
		int kills = DataTranslator.DataToKills (data);
		int deaths = DataTranslator.DataToDeaths (data);

		int newKills = killsSinceLast + kills;
		int newDeaths = deathsSinceLast + deaths;

		string newData = DataTranslator.ValuesToData (newKills, newDeaths);

		lastKills = player.kills;
		lastDeaths = player.deaths;

		UserAccountManager.instance.SendData (newData);
	}
}
