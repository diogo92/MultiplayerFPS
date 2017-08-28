using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

/*
 * Pause menu manager
 */
public class PauseMenu : MonoBehaviour {

	//Check if game is paused
	public static bool IsOn = false;

	private NetworkManager networkManager;

	void Start(){
		networkManager = NetworkManager.singleton;
	}

	/** Behaviour for the Leave Room button, to properly leave a match **/
	public void LeaveRoom(){

		MatchInfo matchInfo = networkManager.matchInfo;
		networkManager.matchMaker.DropConnection (matchInfo.networkId, matchInfo.nodeId, 0, OnDropConnection);
		networkManager.StopHost ();
	}

	void OnDropConnection(bool success, string extendedInfo){
		LoadingScreen.LoadScene ("Lobby");
		networkManager.OnDropConnection (success, extendedInfo);
	}
}
