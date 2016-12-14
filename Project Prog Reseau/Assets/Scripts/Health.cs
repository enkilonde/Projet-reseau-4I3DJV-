using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Health : NetworkBehaviour
{
    [SyncVar(hook = "OnChangeHealth")]
    public int currentHealth = 100;

    public const int maxHealth = 100;

    RectTransform healthBar;

    RectTransform localLifeBar;


    // Use this for initialization
    void Awake ()
    {
        healthBar = GameObject.Find("Frontground").GetComponent<RectTransform>();
        localLifeBar = transform.Find("LocalPlayerCanvas").Find("LifeFront").GetComponent<RectTransform>();

        transform.Find("LocalPlayerCanvas").SetParent(null);


    }

    // Update is called once per frame
    void Update ()
    {
        if (!isLocalPlayer) return;

	}

    public void TakeDamages(int Value, int OriginPLayer)
    {
        if (isServer)
        {
            currentHealth = (int)Mathf.Clamp(currentHealth + Value, 0, maxHealth);
            if (currentHealth == 0) Death(OriginPLayer);
        }


    }

    void OnChangeHealth(int currentHealth)
    {
        localLifeBar.sizeDelta = new Vector2((float)currentHealth / 100.0f, localLifeBar.sizeDelta.y);

        if (isLocalPlayer)
        healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
    }

    void Death(int OriginPLayer)
    {
        switch (tag)
        {
            case "Player":
                PlayerDeath(OriginPLayer);
                break;

            default:
                Destroy(gameObject);
                break;
        }
    }

    void PlayerDeath(int OriginPLayer)
    {
        print("death of player" + GetComponent<PlayerMove>().PlayerID + " by player " + OriginPLayer);

        if(GetComponent<PlayerMove>().PlayerID != OriginPLayer)
        {
            FindObjectOfType<ServerManager>().AddScore(OriginPLayer - 1, 1);
        }
        else
        {
            FindObjectOfType<ServerManager>().AddScore(OriginPLayer - 1, -1);
        }

        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
        //print("Death of player " + GetComponent<PlayerMove>().PlayerID);

        currentHealth = maxHealth;
        GetComponent<PlayerMove>().Rpc_RespawnRandom();

        //for (int i = 0; i < Players.Length; i++)
        //{
        //    Players[i].GetComponent<PlayerMove>().Rpc_Respawn();
        //    Players[i].GetComponent<Health>().currentHealth = maxHealth;
        //}
    }

}
