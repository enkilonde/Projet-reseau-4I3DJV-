using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public class BulletsProperties : NetworkBehaviour
{

    [SyncVar] public int OriginPlayer = -1;

}
