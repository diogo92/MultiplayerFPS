using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;
using System.Data.SqlClient;
public class DatabaseHandler : MonoBehaviour {

	public enum DataType
	{
		KillCount,
		DeathCount,
		Avatar
	}

	[SerializeField]
	private string registerURL = "http://recorded-interview.000webhostapp.com/RegisterPlayer.php";
	[SerializeField]
	private string loginURL = "http://recorded-interview.000webhostapp.com/Login.php";
	[SerializeField]
	private string getDataURL = "http://recorded-interview.000webhostapp.com/GetDataHandler.php";
	[SerializeField]
	private string setDataURL = "http://recorded-interview.000webhostapp.com/SetDataHandler.php";

	public static DatabaseHandler instance;

	public static string LoggedIn_Username { get; protected set; } //stores username once logged in

	public static bool IsLoggedIn { get; protected set; }

	public string loggedInSceneName = "Lobby";

	public string loggedOutSceneName = "MainMenu";

	public delegate void OnDataReceivedCallback(string data);

	void Awake(){
		instance = this;
		DontDestroyOnLoad (instance);
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
			DoLogin (_username);
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

	#region data GET queries
	/*
	public void GetData(OnDataReceivedCallback onDataReceived,DataType dataToGet,string _username = ""){
		if (IsLoggedIn)
			_username = LoggedIn_Username;
		switch (dataToGet) {
		case DataType.KillCount:
		StartCoroutine (GetKillCount (_username, onDataReceived));
			break;
		case DataType.DeathCount:
		StartCoroutine (GetDeathCount (_username, onDataReceived));
			break;
		case DataType.Avatar:
		StartCoroutine (GetAvatar (_username, onDataReceived));
			break;
		default:
			break;
		}
		
	}*/

	public IEnumerator GetKillCount(OnDataReceivedCallback onDataReceived){
		WWWForm form = new WWWForm ();
		form.AddField ("GET","GetKills");
		form.AddField ("user",LoggedIn_Username);
		WWW www = new WWW (getDataURL, form);
		yield return www;
		if (onDataReceived != null)
			onDataReceived.Invoke(www.text);
	}

	public IEnumerator GetDeathCount(OnDataReceivedCallback onDataReceived){
		WWWForm form = new WWWForm ();
		form.AddField ("GET","GetDeaths");
		form.AddField ("user",LoggedIn_Username);
		WWW www = new WWW (getDataURL, form);
		yield return www;
		if (onDataReceived != null)
			onDataReceived.Invoke(www.text);
	}

	public IEnumerator GetAvatar(OnDataReceivedCallback onDataReceived, string _username){
		WWWForm form = new WWWForm ();
		form.AddField ("GET","GetAvatar");
		form.AddField ("user",_username);
		WWW www = new WWW (getDataURL, form);
		yield return www;
		if (onDataReceived != null)
			onDataReceived.Invoke(www.text);
	}

	#endregion

	#region data INSERT/UPDATE queries
	/*
	public void SetData(string _newVal, DataType dataToSet){
			switch (dataToSet) {
			case DataType.KillCount:
				StartCoroutine (SetKillCount (LoggedIn_Username, _newVal));
				break;
			case DataType.DeathCount:
				StartCoroutine (SetDeathCount (LoggedIn_Username, _newVal));
				break;
			case DataType.Avatar:
				StartCoroutine (SetAvatar (LoggedIn_Username, _newVal));
				break;
			default:
				break;
			}

	}*/

	public IEnumerator SetKillCount(int _newVal){
		WWWForm form = new WWWForm ();
		form.AddField ("SET","SetKills");
		form.AddField ("user",LoggedIn_Username);
		form.AddField ("newVal", _newVal);
		WWW www = new WWW (setDataURL, form);
		yield return www;
	}

	public IEnumerator SetDeathCount(int _newVal){
		WWWForm form = new WWWForm ();
		form.AddField ("SET","SetDeaths");
		form.AddField ("user",LoggedIn_Username);
		form.AddField ("newVal", _newVal);
		WWW www = new WWW (setDataURL, form);
		yield return www;
	}

	public IEnumerator SetAvatar(string _newVal){
		WWWForm form = new WWWForm ();
		form.AddField ("SET","SetAvatar");
		form.AddField ("user",LoggedIn_Username);
		form.AddField ("newVal", _newVal);
		WWW www = new WWW (setDataURL, form);
		yield return www;
	}

	#endregion

}
