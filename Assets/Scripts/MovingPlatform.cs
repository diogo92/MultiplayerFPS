using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour 
{
	private Rigidbody rb;
	public Transform[] Waypoints;
	public float speed = 2;

	public int CurrentPoint = 0;

	void Start(){
		rb = GetComponent<Rigidbody> ();
		speed = Random.Range (0.5f, 3f);
	}
	void FixedUpdate () 
	{
		if(transform.position.y != Waypoints[CurrentPoint].transform.position.y)
		{
				rb.MovePosition(transform.position + (Waypoints[CurrentPoint].transform.position - transform.position).normalized * speed * Time.deltaTime);
		}

		if(Vector3.Distance(transform.position,Waypoints[CurrentPoint].transform.position)<= 0.1f)
		{
			CurrentPoint +=1;
		}
		if( CurrentPoint >= Waypoints.Length)
		{
			CurrentPoint = 0; 
		}
	}
}
