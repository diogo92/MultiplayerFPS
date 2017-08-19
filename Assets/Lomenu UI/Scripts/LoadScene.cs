using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
	public void ChangeToScene (string sceneName)
	{		
		LoadingScreen.LoadScene(sceneName);
	}
}
