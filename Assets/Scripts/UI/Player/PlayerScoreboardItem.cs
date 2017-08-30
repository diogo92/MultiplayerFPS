﻿using UnityEngine;
using UnityEngine.UI;

/*
 * Scoreboard UI element
 */
public class PlayerScoreboardItem : MonoBehaviour {

	[SerializeField]
	Text usernameText;

	[SerializeField]
	Text killsText;

	[SerializeField]
	Text deathsText;

	[SerializeField]
	Image avatar;

	public void Setup(string username, int kills, int deaths, Sprite _avatar){
		usernameText.text = username;
		killsText.text = "Kills: " + kills;
		deathsText.text = "Deaths: " + deaths;
		if (_avatar == null)
			avatar.gameObject.SetActive (false);
		else
			avatar.sprite = _avatar;
	}	
}