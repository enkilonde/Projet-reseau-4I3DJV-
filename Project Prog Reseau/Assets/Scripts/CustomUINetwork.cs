using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class CustomNetworkManager : NetworkManager
{

    InputField portInputField;    
    
	// Use this for initialization
	void Start ()
    {
        ApplyIP();
	}
	
	// Update is called once per frame
	void Update ()
    {
        ApplyNewPort();

	}


    void ApplyNewPort()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0) return;

        if (!portInputField)
        {
            portInputField = GameObject.Find("PortSelection").GetComponent<InputField>();
        }

        if (!portInputField) return;

        if (portInputField.text != "")
        {
            int port = 0;
            if (int.TryParse(portInputField.text, out port))
            {
                networkPort = port;
            }
        }
        else
        {
            networkPort = 4343;
        }
    }

    void ApplyIP()
    {
        networkAddress = Network.player.ipAddress;
    }
}
