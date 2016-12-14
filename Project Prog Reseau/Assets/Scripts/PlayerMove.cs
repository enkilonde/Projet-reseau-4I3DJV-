using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerMove : NetworkBehaviour {

    [SyncVar]
    public int PlayerID = -1;

    bool isPlayerAndServer = false;

    static bool playerAndServer = false;


	public GameObject bulletPrefab;
	public Transform bulletSpawn;

    ServerManager serverManagerScript;

    float cooldownFire = 0.2f;
    float coolDownEffective = 0;

    Vector3[] respawPoints = new Vector3[4] { new Vector3(27, 1, 17), new Vector3(-27, 1, 17), new Vector3(27, 1, -17), new Vector3(-27, 1, -17) };


	public override void OnStartLocalPlayer()
	{
		base.OnStartLocalPlayer();
		name = "Local Player";

        if (isServer)
        {
            isPlayerAndServer = true;
            playerAndServer = true;
            serverManagerScript.serverIsPlayer = true;
        }

        GetComponent<Renderer>().material.color = Color.blue;
        StartCoroutine(WaitForConnection(ClientStart, 0));


    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        serverManagerScript.numberOfPlayers++;

        playerAndServer = false;
    }

    [Command]
    void Cmd_SetPlayerID(bool playerAndServer)
    {
        int id = connectionToClient.connectionId;

        PlayerID = id + (1 - (playerAndServer ? 0 : 1));
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

        Cmd_SetPlayerID(serverManagerScript.serverIsPlayer);

        StartCoroutine(WaitToBeIdentified(Respawn));
        StartCoroutine(WaitToBeIdentified(AttributeScoreColor));


    }

    // Update is called once per frame
    void Update ()
	{
		if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.R)) Rpc_Respawn();

        if (serverManagerScript.numberOfPlayers <= 1) return;

		var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
		var z = Input.GetAxis("Vertical") * Time.deltaTime * 300;

		transform.Rotate(0, x, 0);
        //transform.Translate(0, 0, z);

        GetComponent<Rigidbody>().velocity = transform.forward * z;

        coolDownEffective -= Time.deltaTime;

		if (Input.GetKey(KeyCode.Space) && coolDownEffective <=0)
		{
			Cmd_Fire();
            coolDownEffective = cooldownFire;
		}

		

	}

	[Command]
	void Cmd_Fire()
	{
		// Create the Bullet from the Bullet Prefab
		var bullet = (GameObject)Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
		bullet.transform.rotation = bulletSpawn.rotation;

		// Add velocity to the bullet
		bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 10;

        bullet.GetComponent<BulletsProperties>().OriginPlayer = PlayerID;

		// Destroy the bullet after 2 seconds
		Destroy(bullet, 6.0f);

		NetworkServer.Spawn(bullet);
	}

    void OnCollisionEnter(Collision coll)
    {
        if (!isServer) return;
        if(coll.transform.tag == "Bullet")
        {
            int damages = 10;
            if (coll.transform.GetComponent<BulletsProperties>().OriginPlayer == PlayerID) damages = 5;
            GetComponent<Health>().TakeDamages(-damages, coll.transform.GetComponent<BulletsProperties>().OriginPlayer);
            DestroyBullet(coll.gameObject);
        }
    }

    void DestroyBullet(GameObject bullet)
    {
        NetworkServer.Destroy(bullet);
    }

    [Command]
    public void Cmd_Respawn()
    {
        Rpc_Respawn();
    }

    [ClientRpc]
    public void Rpc_Respawn()
    {
        Respawn();
    }

    [ClientRpc]
    public void Rpc_RespawnRandom()
    {
        RespawnRand();
    }

    void Respawn()
    {
        if (!isLocalPlayer) return;

        Debug.Log("Respawn player " + PlayerID);

        Vector3 respawnPosition = Vector3.zero + new Vector3(1, 0, 0);

        switch (PlayerID)
        {
            case 4:
            case 0:
                respawnPosition = new Vector3(-27, 1, 17);
                break;

            case 1:
                respawnPosition = new Vector3(27, 1, -17);
                break;

            case 2:
                respawnPosition = new Vector3(-27, 1, -17);
                break;

            case 3:
                respawnPosition = new Vector3(27, 1, 17);
                break;
        } 

        transform.position = respawnPosition;
        transform.LookAt(new Vector3(0, transform.position.y, 0));
        transform.Rotate(0, 0, 0);
    }

    void RespawnRand()
    {
        if (!isLocalPlayer) return;

        Debug.Log("Respawn player " + PlayerID);

        Vector3 respawnPosition = Vector3.zero + new Vector3(1, 0, 0);

       respawnPosition = respawPoints[UnityEngine.Random.Range(0, 4)];

        transform.position = respawnPosition;
        transform.LookAt(new Vector3(0, transform.position.y, 0));
        transform.Rotate(0, 0, 0);
    }

    void AttributeScoreColor()
    {

        switch (PlayerID)
        {
            case 1:
                GameObject.Find("ScorePlayer1").GetComponent<Text>().color = Color.blue;
                GameObject.Find("ScorePlayer2").GetComponent<Text>().color = Color.red;

                break;

            case 2:
                GameObject.Find("ScorePlayer2").GetComponent<Text>().color = Color.blue;
                GameObject.Find("ScorePlayer1").GetComponent<Text>().color = Color.red;
                break;

            default:
                break;
        }

    }

}
