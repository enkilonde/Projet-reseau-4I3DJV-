using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerMove : NetworkBehaviour {

	public GameObject bulletPrefab;
	public Transform bulletSpawn;

	public override void OnStartLocalPlayer()
	{
		base.OnStartLocalPlayer();
		name = "Local Player";

		GetComponent<Renderer>().material.color = Color.blue;

	}

	public override void OnStartClient()
	{
		base.OnStartClient();
		name = "Other Player";

		GetComponent<Renderer>().material.color = Color.red;
	}

	// Update is called once per frame
	void Update ()
	{
		if (!isLocalPlayer) return;


		var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
		var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

		transform.Rotate(0, x, 0);
		transform.Translate(0, 0, z);

		if (Input.GetKeyDown(KeyCode.Space))
		{
			Cmd_Fire();
		}
	}

	[Command]
	void Cmd_Fire()
	{
		// Create the Bullet from the Bullet Prefab
		var bullet = (GameObject)Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
		bullet.transform.rotation = bulletSpawn.rotation;

		// Add velocity to the bullet
		bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 6;

		// Destroy the bullet after 2 seconds
		Destroy(bullet, 2.0f);

		NetworkServer.Spawn(bullet);
	}

}
