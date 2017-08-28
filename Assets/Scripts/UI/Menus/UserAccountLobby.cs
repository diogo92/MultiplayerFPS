using UnityEngine;
using UnityEngine.UI;

public class UserAccountLobby : MonoBehaviour {

	public Text usernameText;

	void Start () {
		if(DatabaseHandler.IsLoggedIn)
			usernameText.text = DatabaseHandler.LoggedIn_Username;
	}

	public void LogOut(){
		if(DatabaseHandler.IsLoggedIn)
			DatabaseHandler.instance.LogOut ();
		else
			LoadingScreen.LoadScene("MainMenu");
	}
}
