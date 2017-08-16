using UnityEngine;
using System.Collections;
/*
 * Handle Weapon IK and positioning correction, both for local and remote player objects
 * For the local player it is active on the FPS view player model, and for remote player objects it is active on the full player model
 */
public class WeaponIK : MonoBehaviour
{
	//Reference to the righ hand of the player model, which will parent the weapon's graphics components
	[SerializeField] Transform handMount;
	//Reference to the right shoulder of the player model, which will parent the IK target transforms
	[SerializeField] Transform shoulder;
	//Reference to the gun pivot, for position correction on the remote player object
	[SerializeField] Transform gunPivot;
	//Reference to the hands' IK targets parent
	[SerializeField] Transform IKHolders;
	/* References to the right and left hands' IK target transforms*/
	[SerializeField] public Transform PlayerRightHandHold;
	[SerializeField] public Transform PlayerLeftHandHold;

	//Reference to the animator component
	private Animator anim;

	//Check if it is the local player
	bool isLocalPlayer;

	//Check if the component was properly initialized for the first time
	bool initiated = false;

	//Manual checking if the model the component is acting upon is the FPS view or the full player model
	public bool IsFirstPersonView;

	/* Full player model look direction correction references */
	[SerializeField]
	private Transform spineAux;
	[SerializeField]
	private Transform spineBone;

	//Reference to the WeaponManager component
	public WeaponManager weaponManager;

	/* Debugging values for correction of the weapon positioning on the remote player object */
	public Vector3 weaponCorrectionOffset;
	public Vector3 weapotRotationCorrection;
	Vector3 OriginalPivotPosition;
	Vector3 OriginalPivotRotation;
	public bool DebugCorrectionOffset = false;

	void Update()
	{
		//For debugging only, to correct the positioning on the remote player object
		if (initiated) {
			if (!isLocalPlayer && DebugCorrectionOffset) {
				gunPivot.localPosition = OriginalPivotPosition + weaponCorrectionOffset;
				gunPivot.localRotation = Quaternion.Euler (OriginalPivotRotation + weapotRotationCorrection);
			}
		}
	}

	/* Handle the IK targeting */
	void OnAnimatorIK(){
		if (!anim || !initiated || weaponManager.isReloading || PlayerRightHandHold == null || PlayerLeftHandHold==null){
			return;
		}
		anim.SetIKPositionWeight (AvatarIKGoal.RightHand, 1f);
		anim.SetIKRotationWeight (AvatarIKGoal.RightHand, 1f);
		anim.SetIKPosition (AvatarIKGoal.RightHand, PlayerRightHandHold.position);
		anim.SetIKRotation (AvatarIKGoal.RightHand, PlayerRightHandHold.rotation);
		anim.SetIKPositionWeight (AvatarIKGoal.LeftHand, 1f);
		anim.SetIKRotationWeight (AvatarIKGoal.LeftHand, 1f);
		anim.SetIKPosition (AvatarIKGoal.LeftHand, PlayerLeftHandHold.position);
		anim.SetIKRotation (AvatarIKGoal.LeftHand, PlayerLeftHandHold.rotation);


	}

	void LateUpdate(){
		/*
		 * The spineAux is a reference to a transform that is updated by the PlayerMotor component on the remote player client, to match its vertical direction 
		 * The full player model of the remote player in the local client is then updated to match that same transform
		 */
		if (!IsFirstPersonView && initiated) {
			spineBone.rotation = spineAux.rotation;
		}
	}

	/* Set up the references upon switching weapon, depending if it's the local or remote player object */
	public void SwitchWeapon(PlayerWeapon _weapon){
		if (isLocalPlayer) {
			PlayerRightHandHold = _weapon.LocalIKRightHandHold;
			PlayerLeftHandHold = _weapon.LocalIKLeftHandHold;
		} else {
			PlayerRightHandHold = _weapon.RemoteIKRightHandHold;
			PlayerLeftHandHold = _weapon.RemoteIKLeftHandHold;

			/*** These references are setup only for debugging the positioning of the weapon ***/
			gunPivot = _weapon.pivot;
			OriginalPivotPosition = gunPivot.localPosition;
			OriginalPivotRotation = gunPivot.localRotation.eulerAngles;
		}
	}

	/* Set the weapons' transform parents to be the right hand of the player model */
	public void SetWeaponTransformParent(Transform[] _weaponParents){
		for (int i = 0; i < _weaponParents.Length; i++) {
			_weaponParents [i].parent = handMount;
		}
	}

	/* Set up the initial references hierarchy and positioning, as well as the check for if it is the local player or not */
	public void Setup(bool _isLocalPlayer){
		isLocalPlayer = _isLocalPlayer;
		anim = GetComponent<Animator> ();
		if (IKHolders != null && shoulder != null) {
			IKHolders.parent.parent = shoulder;
			IKHolders.parent.localPosition = Vector3.zero;
			foreach (Transform child in IKHolders) {
				child.localPosition = Vector3.zero;
			}
		}
		initiated = true;
	}

}