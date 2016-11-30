using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;

public class WallScript : NetworkBehaviour
{

	void OnCollisionEnter(Collision coll)
    {
        if(coll.gameObject.tag == "Bullet")
        {
            Vector3 vel = coll.relativeVelocity;
            Vector3 normal = coll.contacts[0].normal;

            coll.gameObject.GetComponent<Rigidbody>().velocity = Vector3.Reflect(vel, normal);
        }
    }
}
