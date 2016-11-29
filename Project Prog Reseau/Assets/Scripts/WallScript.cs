using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;

public class WallScript : NetworkBehaviour
{

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /* Pour la destruction au contact du mur
    [Command]
    void Cmd_DestroyBullet(GameObject bullet)
    {
        NetworkServer.Destroy(bullet);

    }
    */

    void OnCollisionEnter(Collision coll)
    {
        //Debug.Log("Collision");
        if (coll.gameObject.tag == "Bullet")
        {
            Vector3 normal = coll.contacts[0].normal;
            Vector3 vel = coll.relativeVelocity;
            coll.gameObject.GetComponent<Rigidbody>().velocity = Vector3.Reflect(vel, normal); // Rebond
            //Cmd_DestroyBullet(coll.gameObject); // Destruction au contact du mur
        }

    }

    /*
    void KillBullet(GameObject bullet)
    {
        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < Players.Length; i++)
        {
            Players[i].GetComponent<PlayerMove>().Cmd_DestroyBullet(bullet);
        }
    }
    */

}
