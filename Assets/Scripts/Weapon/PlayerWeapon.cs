using UnityEngine;

[System.Serializable]
/*
 *	Weapon stats and references
 */
public class PlayerWeapon {

	//Weapon name, must match the name of the corresponding firing animation on the parent of the weapon IK transforms
	public string name = "Glock";

	/* Weapon stats */
	public int damage = 10;
	public float range = 100f;
	public float fireRate = 0f;

	//Size of the weapon magazine
	public int maxBullets = 20;

	/* Crosshair minimum and maximum distance to screen center */
	public float crosshairNormalSize = 0f;
	public float crosshairShootingSize = 0f;

	/* Aim Down Sight values*/
	public float ADSFOV = 45f;
	public float ADSSpeed = 2f;

	//Check if weapon has a scope
	public bool hasScope;

	/* Recoil amount for each axis */
	public float VerticalRecoil;
	public float HorizontalRecoil;

	//Current number of bullets
	[HideInInspector]
	public int bullets;

	//Parent object of the weapon
	public GameObject graphics;

	//Pivot of the weapon
	public Transform pivot;

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
}
