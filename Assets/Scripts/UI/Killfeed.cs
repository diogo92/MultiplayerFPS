using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Kill feed to show player when a player has killed another
 */
public class Killfeed : MonoBehaviour {

	/* Reference to the UI prefab of the kill feed item */
	[SerializeField]
	GameObject killfeedItemPrefab;

	void Start () {
		//Queue a callback to the OnKill method
		GameManager.instance.onPlayerKilledCallback += OnKill;
	}

	/* When a player is killed, instantiate the object prefab */
	public void OnKill (string player,string source) {
		GameObject go = (GameObject)Instantiate (killfeedItemPrefab, this.transform);
		go.GetComponent<KillfeedItem> ().Setup (player, source);
		//Set to show up on top
		go.transform.SetAsFirstSibling ();
		Destroy (go, 4f);
	}
}
