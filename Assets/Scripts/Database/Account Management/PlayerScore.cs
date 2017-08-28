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
		if (DatabaseHandler.IsLoggedIn) {
			StartCoroutine(DatabaseHandler.instance.GetKillCount (OnKillCountReceived));
			StartCoroutine(DatabaseHandler.instance.GetDeathCount (OnDeathCountReceived));
		}
	}

	void OnKillCountReceived(string data){

		if (player.kills <= lastKills)
			return;

		int killsSinceLast = player.kills - lastKills;

		if (player.kills == 0)
			return;
		
		int kills = int.Parse (data);

		int newKills = killsSinceLast + kills;

		lastKills = player.kills;

		StartCoroutine(DatabaseHandler.instance.SetKillCount(newKills));
	}

	void OnDeathCountReceived(string data){

		if (player.deaths <= lastDeaths)
			return;

		int deathsSinceLast = player.deaths - lastDeaths;

		if (player.deaths == 0)
			return;

		int deaths = int.Parse (data);

		int newDeaths = deathsSinceLast + deaths;

		lastDeaths = player.deaths;

		StartCoroutine(DatabaseHandler.instance.SetDeathCount(newDeaths));
	}
}
