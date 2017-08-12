using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour
{
	//Shoot
	public bool Reload = false;
	public bool Fire1Down = false;
	public bool Fire1Up = false;

	public float MouseScrollWheel;

	//UI
	public bool Escape = false;
	public bool TabDown = false;
	public bool TabUp = false;


	void Update ()
	{
		/* Shoot */
		//Reload
		Reload = Input.GetKeyDown (KeyCode.R);

		//Fire1
		Fire1Down = Input.GetButtonDown ("Fire1");
		Fire1Up = Input.GetButtonUp ("Fire1");

		//Weapon Switch
		MouseScrollWheel = Input.GetAxis ("Mouse ScrollWheel");

		/* UI */
		//Escape
		Escape = Input.GetKeyDown (KeyCode.Escape);
		TabDown = Input.GetKeyDown (KeyCode.Tab);
		TabUp = Input.GetKeyUp (KeyCode.Tab);
	}
}

