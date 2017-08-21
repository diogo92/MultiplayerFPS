using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
public class HostGame : NetworkBehaviour {

	[SerializeField]
	private uint roomSize = 6;

	private string roomName;

	private NetworkManager networkManager;

	void Start(){
		networkManager = NetworkManager.singleton;
		if (networkManager.matchMaker == null)
			networkManager.StartMatchMaker ();
	}

	public void SetRoomName(string _name){
		roomName = _name;
	}

	public void CreateRoom(){
		LoadingScreen.ActivateLoadScreen ("Creating a match");
		if (roomName != "" && roomName != null) {
			Debug.Log ("Creating Room: " + roomName + " with room for " + roomSize + " players");
			//Create room
			networkManager.matchMaker.CreateMatch(roomName,roomSize,true,"","","",0,0,OnMatchCreate);
		}
	}

	void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo ){
		//LoadingScreen.instance.LoadingDone = true;
		networkManager.OnMatchCreate (success, extendedInfo, matchInfo);

	}
}
