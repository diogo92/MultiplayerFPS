using UnityEngine;
using UnityEngine.Networking;
/*
 * Set up the player on the local client and on all the remote players' respective clients
 */
[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour {



	//Components that will be disabled over the network
	[SerializeField]
	Behaviour[] componentsToDisable;

	[SerializeField]
	string remoteLayerName = "RemotePlayer";

	[SerializeField]
	string dontDrawLayerName = "DontDraw";

	//Full model of the player, to be drawn only on the remote players' clients
	[SerializeField]
	GameObject PlayerModelGraphics;
	//FPS model of the player, to be drawn only on the local player client
	[SerializeField]
	GameObject PlayerFPSViewGraphics;
	//Right hand of the full player model
	[SerializeField]
	Transform PlayerModelWeaponHandMount;
	//Right hand of the local player FPS model
	[SerializeField]
	Transform PlayerFPSViewWeaponHandMount;

	//Reference to the camera that only draws the local player FPS model and weapons
	[SerializeField]
	Camera weaponCamera;
	//Reference to the UI prefab to be created
	[SerializeField]
	GameObject playerUIPrefab;
	//Instance of the player UI
	[HideInInspector]
	public GameObject playerUIInstance;


	void Start () {
		
		if (isLocalPlayer) {
			SetupLocalPlayer ();
		} else {
			SetupRemotePlayer ();
		}
		//Set up the PlayerManager component
		GetComponent<PlayerManager> ().SetupPlayer ();
	}



	/** Set up the player for their local client **/
	void SetupLocalPlayer(){
		//Disable the components of the full player model that handle IK and animation
		PlayerModelGraphics.GetComponentInChildren<Animator> ().enabled = false;
		PlayerModelGraphics.GetComponentInChildren<WeaponIK> ().enabled = false;

		//Start the WeaponIK component as local player
		GetComponent<WeaponIK> ().Setup (true);

		//Set the full player model to not be drawn
		SetLayerRecursively (PlayerModelGraphics, LayerMask.NameToLayer (dontDrawLayerName));

		/* Set up the player UI */
		playerUIInstance = Instantiate (playerUIPrefab);
		playerUIInstance.name = playerUIPrefab.name;
		PlayerUI ui = playerUIInstance.GetComponent<PlayerUI> ();
		ui.SetPlayer (GetComponent<PlayerManager> ());
		ui.GetComponentInChildren<CrosshairManager> ().Setup (weaponCamera);

		//Set up the Player Score sync manager
		PlayerScore.instance.Setup(GetComponent<PlayerManager>());

		/* Set up the username of the player for the match */
		string _username = "Loading...";
		if (DatabaseHandler.IsLoggedIn)
			_username = DatabaseHandler.LoggedIn_Username;
		else
			_username = transform.name;
		CmdSetUsername (transform.name, _username);
	}

	/** Set up the player for the remote players' clients over the network **/
	void SetupRemotePlayer(){
		PlayerFPSViewGraphics.SetActive (false);
		PlayerModelGraphics.GetComponentInChildren<WeaponIK> ().enabled = true;
		PlayerModelGraphics.GetComponentInChildren<WeaponIK> ().Setup (false);

		DisableComponents ();
		AssignRemoteLayer ();
		SetLayerRecursively (PlayerFPSViewGraphics, LayerMask.NameToLayer (dontDrawLayerName));
		GetComponent<WeaponIK> ().enabled = false;
	}

	/** Command to set the player's name over the network **/
	[Command]
	void CmdSetUsername(string playerID, string username){
		PlayerManager player = GameManager.GetPlayer (playerID);
		if (player != null) {
			player.username = username;
		}
	}

	/** Set the layer of (obj) and all its children to (newLayer) **/
	void SetLayerRecursively (GameObject obj, int newLayer){
		obj.layer = newLayer;

		foreach (Transform child in obj.transform) {
			SetLayerRecursively (child.gameObject, newLayer);
		}
	}

	/** Override of the OnStartClient method to also register a new player**/
	public override void OnStartClient(){
		base.OnStartClient ();
		string _netID = GetComponent<NetworkIdentity> ().netId.ToString();
		PlayerManager _player = GetComponent<PlayerManager> ();
		GameManager.RegisterPlayer (_netID, _player);
	}

	/** Disable the components on the (componentsToDisable) array **/
	void DisableComponents(){
		for (int i = 0; i < componentsToDisable.Length; i++) {
			componentsToDisable [i].enabled = false;
		}
	}

	/** Set gameObject's layer to (remoteLayerName) **/
	void AssignRemoteLayer(){
		gameObject.layer = LayerMask.NameToLayer (remoteLayerName);
	}

	void OnDisable(){
		Destroy (playerUIInstance);
		if(isLocalPlayer)
			GameManager.instance.SetSceneCameraState (true);
		GameManager.UnRegisterPlayer (transform.name);
	}



}
