using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	public MatchSettings matchSettings;

	[SerializeField]
	private GameObject sceneCamera;

	public delegate void OnPlayerKilledCallback(string player,string source);
	public OnPlayerKilledCallback onPlayerKilledCallback;

	void Awake(){
		instance = this;
	}

	public void SetSceneCameraState(bool state){
		if (sceneCamera == null)
			return;
		sceneCamera.SetActive (state);
	}

	#region Player registering
	private const string PLAYER_ID_PREFIX = "Player ";

	private static Dictionary <string, PlayerManager> players = new Dictionary <string,PlayerManager> ();

	public static void RegisterPlayer(string _netID,PlayerManager _player){
		string _playerID = PLAYER_ID_PREFIX + _netID;
		players.Add (_playerID, _player);
		_player.transform.name = _playerID;
	}

	public static void UnRegisterPlayer(string _playerID){
		players.Remove (_playerID);
	}

	public static PlayerManager GetPlayer(string _playerID){
		return players [_playerID];
	}

	public static PlayerManager[] GetAllPlayers(){
		return players.Values.ToArray ();
	}
	/*void OnGUI(){
		GUILayout.BeginArea (new Rect (200,200,200,500));
		GUILayout.BeginVertical ();
		foreach (string _playerID in players.Keys) {
			GUILayout.Label (_playerID + " - " + players [_playerID].transform.name);
		}
		GUILayout.EndVertical ();
		GUILayout.EndArea ();
	}*/
	#endregion
}
