using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

[RequireComponent(typeof(PlayerSetup))]
public class PlayerManager : NetworkBehaviour {

	[SyncVar]
	private bool _isDead = false;
	public bool isDead{
		get{ return _isDead; }
		protected set { _isDead = value;}
	}

	[SerializeField]
	private int maxHealth = 100;

	[SyncVar]
	private int currentHealth;

	public float GetHealthAmount(){
		return (float)currentHealth / maxHealth;
	}

	[SyncVar]
	public string username = "Loading...";

	[SyncVar]
	public bool isLoggedIn = false;
	[SyncVar]
	bool broadcastDone = false;


	public int kills;
	public int deaths;


	[SerializeField]
	private Behaviour[] disableOnDeath;
	private bool[] wasEnabled;

	[SerializeField]
	private GameObject[] disableGameObjectsOnDeath;

	[SerializeField]
	private GameObject deathEffect;

	[SerializeField]
	private GameObject spawnEffect;

	private bool firstSetup = true;

	private Sprite playerAvatar;
	private Sprite localPlayerAvatar;

	void OnDestroy(){
		if(PlayerScore.instance != null)
			PlayerScore.instance.SyncNow ();
	}

	public void SetupPlayer(){
		if (isLocalPlayer) {
			GameManager.instance.SetSceneCameraState (false);
			GetComponent<PlayerSetup> ().playerUIInstance.SetActive (true);
			if (PlayerPrefs.HasKey ("UserAvatarName"))
				SetLocalPlayerAvatar (PlayerPrefs.GetString ("UserAvatarName"));
			CmdBroadCastNewPlayerSetup ();
		} else {
			StartCoroutine(WaitForBroadcast());
		}
			
	}

	IEnumerator WaitForBroadcast(){
		while (!broadcastDone) {
			yield return null;
		}
		if (isLoggedIn) {
			StartCoroutine(DatabaseHandler.instance.GetAvatar (SetPlayerAvatar,username));
		}
	}

	void SetPlayerAvatar(string data){
		Texture2D tex = new Texture2D (1, 1, TextureFormat.DXT1, false);
		tex.LoadImage (Convert.FromBase64String(data));
		playerAvatar = Sprite.Create (tex, new Rect (0f, 0f, tex.width, tex.height), new Vector2 (0.5f, 0.5f), 100.0f);

	}

	void SetLocalPlayerAvatar(string _imageName){
		UnityEngine.Object[] avatarsObj = Resources.LoadAll ("Avatars",typeof(Sprite));
		for (int i = 0; i < avatarsObj.Length; i++) {
			if (((Sprite)avatarsObj [i]).name == _imageName) {
				Sprite playerImage = ((Sprite)avatarsObj [i]);
				localPlayerAvatar = playerImage;
			}
		}
	}

	public Sprite GetPlayerAvatar(){
		if (isLocalPlayer)
			return localPlayerAvatar;
		else {
			if(isLoggedIn)
				return playerAvatar;
		}
		return null;
	}

	[Command]
	private void CmdBroadCastNewPlayerSetup(){
		broadcastDone = true;
		isLoggedIn = DatabaseHandler.IsLoggedIn;
		RpcSetupPlayerOnAllClients ();
	}

	[ClientRpc]
	private void RpcSetupPlayerOnAllClients(){
		if (firstSetup) {
			wasEnabled = new bool[disableOnDeath.Length];
			for (int i = 0; i < wasEnabled.Length; i++) {
				wasEnabled [i] = disableOnDeath [i].enabled;
			}
			firstSetup = false;
		}
		SetDefaults ();
	}

//	void Update(){
//		if (!isLocalPlayer)
//			return;
//
//		if (Input.GetKeyDown (KeyCode.K))
//			RpcTakeDamage (9999);
//	}

	public void SetDefaults(){
		isDead = false;
		currentHealth = maxHealth;

		//Enable components
		for (int i = 0; i < disableOnDeath.Length; i++) {
			disableOnDeath [i].enabled = wasEnabled [i];
		}

		//Enable GameObjects
		for (int i = 0; i < disableGameObjectsOnDeath.Length; i++) {
			disableGameObjectsOnDeath [i].SetActive(true);
		}
		Collider _col = GetComponent<Collider> ();
		if (_col != null)
			_col.enabled = true;
		

		//Create spawn effect
		GameObject _spawnEffect = (GameObject)Instantiate (spawnEffect, transform.position, Quaternion.identity);
		Destroy (_spawnEffect, 3f);
	}

	[ClientRpc]
	public void RpcTakeDamage(int _amount, string _sourceID){
		if (isDead)
			return;
		currentHealth -= _amount;
		Debug.Log (transform.name + " current health = " + currentHealth);

		if (currentHealth <= 0) {
			PlayerManager sourcePlayer = GameManager.GetPlayer (_sourceID);
			Die (_sourceID);
		}
	}

	private void Die(string _sourceID){
		isDead = true;
		PlayerManager sourcePlayer = GameManager.GetPlayer (_sourceID);
		if (sourcePlayer != null) {
			sourcePlayer.kills++;
			GameManager.instance.onPlayerKilledCallback.Invoke (username, sourcePlayer.username);
		}

		deaths++;
		//Disable components
		for (int i = 0; i < disableOnDeath.Length; i++) {
			disableOnDeath [i].enabled = false;
		}

		//Disable GameObjects
		for (int i = 0; i < disableGameObjectsOnDeath.Length; i++) {
			disableGameObjectsOnDeath [i].SetActive(false);
		}

		Collider _col = GetComponent<Collider> ();
		if (_col != null)
			_col.enabled = false;

		GameObject _explosion = (GameObject)Instantiate (deathEffect, transform.position, Quaternion.identity);
		Destroy (_explosion, 3f);
		//Switch cameras
		if (isLocalPlayer) {
			GameManager.instance.SetSceneCameraState (true);
			GetComponent<PlayerSetup> ().playerUIInstance.SetActive (false);
		}

		Debug.Log (transform.name + " is dead!");

		StartCoroutine (Respawn ());
	}

	private IEnumerator Respawn(){
		yield return new WaitForSeconds (GameManager.instance.matchSettings.respawnTime);
		Transform _spawnPoint = NetworkManager.singleton.GetStartPosition ();
		transform.position = _spawnPoint.position;
		transform.rotation = _spawnPoint.rotation;

		yield return new WaitForSeconds (0.1f);

		SetupPlayer ();
		Debug.Log (transform.name + " respawned");
	}
}
