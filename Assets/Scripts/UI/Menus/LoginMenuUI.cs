using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginMenuUI : MonoBehaviour {

	public InputField Input_Login_Username;
	public InputField Input_Login_Password;

	public InputField Input_Register_Username;
	public InputField Input_Register_Password;
	public InputField Input_Register_ConfirmPassword;

	public Text LoginErrorText;
	public Text RegisterErrorText;

	private string _userName;
	private string _password;
	private string _confirmPassword;

	public static LoginMenuUI instance;

	void Awake(){
		instance = this;
	}

	public void CreateNewPlayer(){
		_userName = Input_Register_Username.text;
		_password = Input_Register_Password.text;
		_confirmPassword = Input_Register_ConfirmPassword.text;

		if (_userName == "" || _password == "" || _confirmPassword == "") {
			RegisterError("One or more fields are empty!");
			return;
		} else if (_password != _confirmPassword) {
			RegisterError ("Passwords don't match!");
			return;
		} else if ( _userName != "" && _password == _confirmPassword){
			StartCoroutine (DatabaseHandler.instance.CreatePlayer (_userName, _password));
		}
	}

	public void Login(){
		_userName = Input_Login_Username.text;
		_password = Input_Login_Password.text;

		if (_userName == "" || _password == "") {
			LoginError("One or more fields are empty!");
			return;
		} else {
			StartCoroutine (DatabaseHandler.instance.Login (_userName, _password));
		}
	}

	public void LoadLobby(){
		LoadingScreen.LoadScene("Lobby");
	}

	IEnumerator DisableErrorTexts(){
		yield return new WaitForSeconds (3f);
		RegisterErrorText.text = "";
		LoginErrorText.text = "";
		RegisterErrorText.gameObject.SetActive (false);
		LoginErrorText.gameObject.SetActive (false);
	}

	public void RegisterError(string message){
		RegisterErrorText.text = message;
		RegisterErrorText.gameObject.SetActive (true);
		StartCoroutine (DisableErrorTexts ());
	}

	public void LoginError(string message){
		LoginErrorText.text = message;
		LoginErrorText.gameObject.SetActive (true);
		StartCoroutine (DisableErrorTexts ());
	}
}
