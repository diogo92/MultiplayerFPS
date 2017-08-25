using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;
using System.Data.SqlClient;
public class DatabaseHandler : MonoBehaviour {

	[SerializeField]
	private string registerURL = "http://recorded-interview.000webhostapp.com/RegisterPlayer.php";
	[SerializeField]
	private string loginURL = "http://recorded-interview.000webhostapp.com/Login.php";

	public static DatabaseHandler instance;

	public static string LoggedIn_Username { get; protected set; } //stores username once logged in

	public static bool IsLoggedIn { get; protected set; }

	public string loggedInSceneName = "Lobby";

	public string loggedOutSceneName = "MainMenu";

	void Awake(){
		instance = this;
		DontDestroyOnLoad (instance);
	}

	void Start(){
		
	}

	public IEnumerator CreatePlayer(string _username, string _password){
		WWWForm form = new WWWForm ();
		form.AddField ("user",_username);
		form.AddField ("pass",_password);

		WWW www = new WWW (registerURL, form);
		yield return www;
		Debug.Log (www.text);
		if (www.text.Length > 1) {
			LoginMenuUI.instance.RegisterError ("This username is already in use, please try again");
		} else {
			//TODO login and load lobby scene
		}
	}


	public IEnumerator Login(string _username, string _password){
		WWWForm form = new WWWForm ();
		form.AddField ("user",_username);
		form.AddField ("pass",_password);

		WWW www = new WWW (loginURL, form);
		yield return www;
		Debug.Log (www.text);
		if (www.text.Length > 0 && www.text.Length < 2) {
			Debug.Log ("LOGIN SUCCESS");
			DoLogin (_username);
		} else if(www.text.Length > 1) {
			LoginMenuUI.instance.LoginError ("An error ocurred, please try again");
		}

	}


	void DoLogin(string _username){
		LoggedIn_Username = _username;
		IsLoggedIn = true;

		Debug.Log("Logged in as " + _username);

		LoadingScreen.LoadScene(loggedInSceneName);
	}

	public void LogOut()

	{

		LoggedIn_Username = "";

		IsLoggedIn = false;

		Debug.Log("User logged out!");


		LoadingScreen.LoadScene(loggedOutSceneName);

	}
}
