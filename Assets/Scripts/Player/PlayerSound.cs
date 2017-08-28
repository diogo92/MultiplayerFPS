using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour {

	private AudioSource audioSource;
	private WeaponManager wm;
	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource> ();
		wm = GetComponent<WeaponManager> ();
	}

	public void PlayShootSound(){
		if(wm.GetCurrentWeapon().shootSound != null)
			audioSource.PlayOneShot(wm.GetCurrentWeapon().shootSound);
	}

	public void PlayReloadSound(){
		if(wm.GetCurrentWeapon().reloadSound != null)
			audioSource.PlayOneShot(wm.GetCurrentWeapon().reloadSound);
	}
}
