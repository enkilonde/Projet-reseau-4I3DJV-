using UnityEngine;
using System.Collections;

public class GetIp : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		Debug.Log(Network.player.ipAddress);
	}
}
