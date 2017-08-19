using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadSceneTrigger : MonoBehaviour 
{
	[Header("SETTINGS")]
	public bool onTriggerExit;
	public bool onlyLoadWithTag;

	[Header("STRINGS")]
	public string objectTag;
	public string sceneName;

	private void OnTriggerEnter(Collider other)
	{		
		if(onlyLoadWithTag == true && onTriggerExit == false)
		{
			if (other.gameObject.tag == objectTag) 
			{
				LoadingScreen.LoadScene(sceneName);
			}
		}
		else if (onTriggerExit == false)
		{
			LoadingScreen.LoadScene(sceneName);
		}
	}

	private void OnTriggerExit(Collider other)
	{		
		if(onlyLoadWithTag == true && onTriggerExit == true)
		{
			if (other.gameObject.tag == objectTag) 
			{
				LoadingScreen.LoadScene(sceneName);
			}
		}
		else if (onTriggerExit == true)
		{
			LoadingScreen.LoadScene(sceneName);
		}
	}
}