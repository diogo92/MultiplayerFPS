using UnityEngine;
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

	public void Setup(string username, int kills, int deaths){
		usernameText.text = username;
		killsText.text = "Kills: " + kills;
		deathsText.text = "Deaths: " + deaths;
	}
}
