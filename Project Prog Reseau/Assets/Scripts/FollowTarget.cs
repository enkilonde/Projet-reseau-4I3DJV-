using UnityEngine;
using System.Collections;

public class FollowTarget : MonoBehaviour
{

    public bool followX;
    public bool followY;
    public bool followZ;

    public Transform targetToFollow;

    public bool keepOffset;
    Vector3 baseOffset;

    // Use this for initialization
    void Awake ()
    {

        SetOffset();

	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!targetToFollow) return;


        if (followX)
        {
            transform.position = new Vector3(targetToFollow.position.x + baseOffset.x, transform.position.y, transform.position.x);
        }

        if (followY)
        {
            transform.position = new Vector3(transform.position.x, targetToFollow.position.y + baseOffset.y, transform.position.x);
        }

        if (followZ)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, targetToFollow.position.z + baseOffset.z);
        }

    }

    void SetOffset()
    {
        if (keepOffset && targetToFollow)
            baseOffset = transform.position - targetToFollow.position;
    }
}
