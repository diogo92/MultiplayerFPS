using UnityEngine;
using UnityEngine.Networking;


[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour {

	[SerializeField]
	Behaviour[] componentsToDisable;

	[SerializeField]
	string remoteLayerName = "RemotePlayer";

	[SerializeField]
	string dontDrawLayerName = "DontDraw";

	[SerializeField]
	GameObject PlayerModelGraphics;
	[SerializeField]
	GameObject PlayerFPSViewGraphics;
	[SerializeField]
	Transform PlayerModelWeaponHandMount;
	[SerializeField]
	Transform PlayerFPSViewWeaponHandMount;

	[SerializeField]
	Camera weaponCamera;
	[SerializeField]
	GameObject playerUIPrefab;

	[HideInInspector]
	public GameObject playerUIInstance;



	void Start () {
		if (!isLocalPlayer) {
			PlayerFPSViewGraphics.SetActive (false);
			PlayerModelGraphics.GetComponentInChildren<WeaponIK> ().enabled = true;
			PlayerModelGraphics.GetComponentInChildren<WeaponIK> ().Setup (false);
			DisableComponents ();
			AssignRemoteLayer ();
			SetLayerRecursively (PlayerFPSViewGraphics, LayerMask.NameToLayer (dontDrawLayerName));
			GetComponent<WeaponIK> ().enabled = false;

		} else {
			PlayerModelGraphics.GetComponentInChildren<Animator> ().enabled = false;
			PlayerModelGraphics.GetComponentInChildren<WeaponIK> ().enabled = false;
			GetComponentInChildren<WeaponIK> ().Setup (true);
			SetLayerRecursively (PlayerModelGraphics, LayerMask.NameToLayer (dontDrawLayerName));
			playerUIInstance = Instantiate (playerUIPrefab);
			playerUIInstance.name = playerUIPrefab.name;
			PlayerUI ui = playerUIInstance.GetComponent<PlayerUI> ();
			ui.SetPlayer (GetComponent<PlayerManager> ());
			ui.GetComponentInChildren<CrosshairManager> ().Setup (weaponCamera);
			GetComponent<PlayerManager> ().SetupPlayer ();
			string _username = "Loading...";
			if (UserAccountManager.IsLoggedIn)
				_username = UserAccountManager.LoggedIn_Username;
			else
				_username = transform.name;
			CmdSetUsername (transform.name, _username);
		}
	}

	[Command]
	void CmdSetUsername(string playerID, string username){
		PlayerManager player = GameManager.GetPlayer (playerID);
		if (player != null) {
			player.username = username;
		}
	}


	void SetLayerRecursively (GameObject obj, int newLayer){
		obj.layer = newLayer;

		foreach (Transform child in obj.transform) {
			SetLayerRecursively (child.gameObject, newLayer);
		}
	}

	public override void OnStartClient(){
		base.OnStartClient ();
		string _netID = GetComponent<NetworkIdentity> ().netId.ToString();
		PlayerManager _player = GetComponent<PlayerManager> ();
		GameManager.RegisterPlayer (_netID, _player);
	}

	void DisableComponents(){
		for (int i = 0; i < componentsToDisable.Length; i++) {
			componentsToDisable [i].enabled = false;
		}
	}

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
