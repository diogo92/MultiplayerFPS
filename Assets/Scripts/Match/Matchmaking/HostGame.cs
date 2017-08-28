using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
public class HostGame : NetworkBehaviour {

	[SerializeField]
	private uint roomSize = 6;

	private string roomName;

	private NetworkManager networkManager;

	public string LevelToCreate;

	[SerializeField]
	private string[] sceneNames;

	[SerializeField]
	private Dropdown levelSelection;

	void Start(){
		networkManager = NetworkManager.singleton;
		if (networkManager.matchMaker == null)
			networkManager.StartMatchMaker ();
		if (PlayerPrefs.HasKey ("LastSelectedHostLevel")) {
			SetLevelToCreate (PlayerPrefs.GetInt ("LastSelectedHostLevel"));
			levelSelection.value = PlayerPrefs.GetInt ("LastSelectedHostLevel");
		}
		else
			SetLevelToCreate (0);

	}

	public void SetRoomName(string _name){
		roomName = _name;
	}

	public void CreateRoom(){
		if (roomName != "" && roomName != null) {
			Debug.Log ("Creating Room: " + roomName + " with room for " + roomSize + " players");
			LoadingScreen.ActivateLoadScreen ("Creating a match");
			//Create room
			networkManager.matchMaker.CreateMatch(roomName,roomSize,true,"","","",0,0,OnMatchCreate);
		}
	}

	void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo ){
		//LoadingScreen.instance.LoadingDone = true;
		networkManager.OnMatchCreate (success, extendedInfo, matchInfo);

	}

	public void SetLevelToCreate(int _level){
		if (_level <= sceneNames.Length) {
			networkManager.onlineScene = sceneNames [_level];
			PlayerPrefs.SetInt ("LastSelectedHostLevel", _level);
		}
	}
}
