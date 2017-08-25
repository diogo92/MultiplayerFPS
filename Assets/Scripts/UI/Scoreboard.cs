using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Scoreboard UI panel
 */
public class Scoreboard : MonoBehaviour {

	[SerializeField]
	GameObject playerScoreboardItem;

	[SerializeField]
	Transform playerScoreboardList;

	void OnEnable(){
		PlayerManager[] players = GameManager.GetAllPlayers ();
		foreach (PlayerManager player in players) {
			GameObject go = (GameObject) Instantiate (playerScoreboardItem, playerScoreboardList);
			PlayerScoreboardItem item = go.GetComponent<PlayerScoreboardItem> ();
			if (item != null) {
				item.Setup (player.username, player.kills, player.deaths/*, player.GetPlayerAvatar()*/);
			}
		}
	}

	void OnDisable(){
		foreach (Transform child in playerScoreboardList) {
			Destroy (child.gameObject);
		}
	}
}
