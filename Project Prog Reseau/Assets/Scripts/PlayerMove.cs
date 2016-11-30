using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;

public class PlayerMove : NetworkBehaviour {

    [SyncVar]
    public int PlayerID = -1;

    bool isPlayerAndServer = false;

	public GameObject bulletPrefab;
	public Transform bulletSpawn;

    ServerManager serverManagerScript;

	public override void OnStartLocalPlayer()
	{
		base.OnStartLocalPlayer();
		name = "Local Player";

        if (isServer) isPlayerAndServer = true;

        GetComponent<Renderer>().material.color = Color.blue;
        StartCoroutine(WaitForConnection(ClientStart, 0));


    }

    [Command]
    void Cmd_SetPlayerID(bool playerAndServer)
    {
        int id = connectionToClient.connectionId;
        if (!playerAndServer) id--;

        PlayerID = id;
    }

    void Awake()
    {
        serverManagerScript = FindObjectOfType<ServerManager>();
    }


    public override void OnStartClient()
	{
		base.OnStartClient();
        if (isLocalPlayer) return;
        name = "Other Player";

        if (isServer)
        {
            serverManagerScript.numberOfPlayers++;
        }

        GetComponent<Renderer>().material.color = Color.red;
    }

    IEnumerator WaitForConnection(Action CallbackFunction, int waitTime)
    {
        while (connectionToServer == null) yield return null;

        for (int i = 0; i < waitTime; i++)
        {
            yield return null;
        }

        CallbackFunction();
    }

    IEnumerator WaitToBeIdentified(Action callback)
    {
        while (PlayerID == -1) yield return null;

        callback();
    }

    void ClientStart()
    {

        Cmd_SetPlayerID(isPlayerAndServer);

        StartCoroutine(WaitToBeIdentified(Respawn));
    }

    // Update is called once per frame
    void Update ()
	{
		if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.R)) Rpc_Respawn();

        if (serverManagerScript.numberOfPlayers <= 1) return;

		var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
		var z = Input.GetAxis("Vertical") * Time.deltaTime * 150;

		transform.Rotate(0, x, 0);
        //transform.Translate(0, 0, z);

        GetComponent<Rigidbody>().velocity = transform.forward * z;

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

    void OnTriggerEnter(Collider coll)
    {
        if(coll.tag == "Bullet")
        {
            GetComponent<Health>().TakeDamages(-10, coll.GetComponent<BulletsProperties>().OriginPlayer);
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
        Respawn();
    }

    void Respawn()
    {
        if (!isLocalPlayer) return;

        Debug.Log("Respawn player " + PlayerID);

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
