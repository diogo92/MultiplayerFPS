using UnityEngine;
using System.Collections;
public class WeaponIK : MonoBehaviour
{
	[SerializeField] Transform cameraTransform;
	[SerializeField] Transform handMount;
	[SerializeField] Transform shoulder;
	[SerializeField] Transform gunPivot;
	[SerializeField] Transform weaponHolder;
	[SerializeField] Transform IKHolders;
	[SerializeField] public Transform PlayerRightHandHold;
	[SerializeField] public Transform PlayerLeftHandHold;
	[SerializeField] float threshold = 10f;
	[SerializeField] float smoothing = 5f;

	float pitch;
	public Vector3 weaponCorrectionOffset;
	public Vector3 weapotRotationCorrection;
	float lastSyncedPitch;
	public Animator anim;

	bool isLocalPlayer;
	bool initiated = false;

	public bool DebugCorrectionOffset = false;
	Vector3 OriginalPivotPosition;
	Vector3 OriginalPivotRotation;

	public bool IsFirstPersonView;


	[SerializeField]
	private Transform spineAux;
	[SerializeField]
	private Transform spineBone;


	public float amount = 0.02f;
	public float maxAmount = 0.06f;
	public float smoothAmount =6f;

	private Vector3 initialPosition;

	public float movementX;
	public float movementY;


	public WeaponManager weaponManager;

	/*void Start()
	{
		
		//if (isLocalPlayer)
		//gunPivot.parent = handMount;
		//else
			//lastOffset = handMount.position - transform.position;
	}*/

	void Update()
	{
		if (initiated) {
			if (!isLocalPlayer && DebugCorrectionOffset) {
				gunPivot.localPosition = OriginalPivotPosition + weaponCorrectionOffset;
				gunPivot.localRotation = Quaternion.Euler (OriginalPivotRotation + weapotRotationCorrection);
			}
		}
	}

	void CmdUpdatePitch(float newPitch)
	{
		pitch = newPitch;
	}


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
		if (!IsFirstPersonView && initiated) {
			spineBone.rotation = spineAux.rotation;
		}
	}

	public void SwitchWeapon(PlayerWeapon _weapon){
		if (isLocalPlayer) {
			PlayerRightHandHold = _weapon.LocalIKRightHandHold;
			PlayerLeftHandHold = _weapon.LocalIKLeftHandHold;
		} else {
			gunPivot = _weapon.pivot;
			OriginalPivotPosition = gunPivot.localPosition;
			OriginalPivotRotation = gunPivot.localRotation.eulerAngles;
			PlayerRightHandHold = _weapon.RemoteIKRightHandHold;
			PlayerLeftHandHold = _weapon.RemoteIKLeftHandHold;
		}
	}

	public void SetWeaponTransformParent(Transform[] _weaponParents){
		for (int i = 0; i < _weaponParents.Length; i++) {
			_weaponParents [i].parent = handMount;
		}
	}

	public void Setup(bool _isLocalPlayer){
		isLocalPlayer = _isLocalPlayer;
		anim = GetComponent<Animator> ();
		if (IKHolders != null && shoulder != null) {
			IKHolders.parent = shoulder;
			IKHolders.localPosition = Vector3.zero;
			foreach (Transform child in IKHolders) {
				child.localPosition = Vector3.zero;
			}
		}
		initiated = true;
	}

}