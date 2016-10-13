using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public class MoveCube : NetworkBehaviour
{

	// Use this for initialization
	void Start ()
	{
		Destroy(gameObject, 5);
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.Translate(Vector3.right * Time.deltaTime * 0.5f);
	}
}
