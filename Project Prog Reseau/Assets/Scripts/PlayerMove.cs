using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;

public class PlayerMove : NetworkBehaviour {

    [SyncVar]
    public int PlayerID = -1;

	public GameObject bulletPrefab;
	public Transform bulletSpawn;

    ServerManager serverManagerScript;

	public override void OnStartLocalPlayer()
	{
		base.OnStartLocalPlayer();
		name = "Local Player";

		GetComponent<Renderer>().material.color = Color.blue;

        

        //if (isServer) serverManagerScript.numberOfPlayers++;

    }

    [Command]
    void Cmd_SetPlayerID()
    {
        PlayerID = GetComponent<NetworkIdentity>().connectionToClient.connectionId;
    }

    void Awake()
    {
        serverManagerScript = FindObjectOfType<ServerManager>();
    }


    public override void OnStartClient()
	{
		base.OnStartClient();
		name = "Other Player";
        Cmd_SetPlayerID();

        if (isServer)
        {
            serverManagerScript.numberOfPlayers++;

            Rpc_Respawn();
        }

        StartCoroutine(WaitForConnection(Rpc_Respawn));


        GetComponent<Renderer>().material.color = Color.red;
    }

    IEnumerator WaitForConnection(Action CallbackFunction)
    {
        while (connectionToServer == null) yield return null;

        CallbackFunction();
    }

    // Update is called once per frame
    void Update ()
	{
		if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.R)) Rpc_Respawn();

        if (serverManagerScript.numberOfPlayers <= 1) return;

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

        bullet.GetComponent<BulletsProperties>().OriginPlayer = PlayerID;

		// Destroy the bullet after 2 seconds
		Destroy(bullet, 2.0f);

		NetworkServer.Spawn(bullet);
	}

    void OnCollisionEnter(Collision coll)
    {
        if(coll.gameObject.tag == "Bullet")
        {
            GetComponent<Health>().TakeDamages(-10, coll.gameObject.GetComponent<BulletsProperties>().OriginPlayer);
            Cmd_DestroyBullet(coll.gameObject);
        }
    }

    [Command]
    void Cmd_DestroyBullet(GameObject bullet)
    {
        NetworkServer.Destroy(bullet);

    }

    [Command]
    public void Cmd_Respawn()
    {
        Rpc_Respawn();
    }

    [ClientRpc]
    void Rpc_Respawn()
    {
        if (!isLocalPlayer) return;

        Vector3 respawnPosition = Vector3.zero + new Vector3(1, 0, 0);
        switch (PlayerID)
        {

            case 0:
                respawnPosition = new Vector3(-6, 1, 4);
                break;

            case 1:
                respawnPosition = new Vector3(6, 1, -4);
                break;

            case 2:
                respawnPosition = new Vector3(-6, 1, -4);
                break;

            case 3:
                respawnPosition = new Vector3(6, 1, 4);
                break;

        }

        transform.position = respawnPosition;
        transform.LookAt(new Vector3(0, transform.position.y, 0));
        transform.Rotate(0, 0, 0);
    }


}
