using UnityEngine;

[System.Serializable]
public class PlayerWeapon {

	public string name = "Glock";

	public int damage = 10;

	public float range = 100f;

	public float fireRate = 0f;

	public int maxBullets = 20;

	public float crosshairNormalSize = 0f;
	public float crosshairShootingSize = 0f;

	public float ADSFOV = 45f;
	public bool hasScope;

	public float ADSSpeed = 2f;
	//Current number of bullets
	[HideInInspector]
	public int bullets;

	//Parent object of the weapon
	public GameObject graphics;

	//Pivot of the weapon
	public Transform pivot;

	/*	Weapon specific animations
	 * 	
	 * 	0 - IKHolder animation (kickback effect)
	 * 	1 - Weapon model animation (Slider, etc)
	 * 
	 */
	public AnimationClip[] animations;

	/** Transforms for IK positioning **/

	//ADS positioning target on local player
	public Vector3 ADSPosition;
	public Vector3 ADSRotation;

	//ADS positioning target on remote player
	public Vector3 ADSRemotePosition;
	public Vector3 ADSRemoteRotation;

	//Hands' tranforms for local player
	public Transform LocalIKRightHandHold;
	public Transform LocalIKLeftHandHold;

	//Hands' transforms for remote player model
	public Transform RemoteIKRightHandHold;
	public Transform RemoteIKLeftHandHold;

	//Offets for correcting weapon pivot on remote player model
	public Vector3 PivotCorrectionOffset;
	public Vector3 RotationCorrectionOffset;

	/** Camera shake**/
	public float ShakeIntensity;
	public float ShakeDecayRate;
}
