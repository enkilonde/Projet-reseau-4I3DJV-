using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Health : NetworkBehaviour
{
    [SyncVar(hook = "OnChangeHealth")]
    public int currentHealth = 100;

    public const int maxHealth = 100;

    RectTransform healthBar;

	// Use this for initialization
	void Awake ()
    {
        healthBar = GameObject.Find("Frontground").GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update ()
    {

        if (!isServer) return;
        
        if (Input.GetKeyDown(KeyCode.H)) TakeDamages(-10);

	}

    public void TakeDamages(int Value)
    {
        if (isServer)
        {
            currentHealth = (int)Mathf.Clamp(currentHealth + Value, 0, maxHealth);
            if (currentHealth == 0) Death();
        }


    }

    void OnChangeHealth(int currentHealth)
    {
        if(isLocalPlayer)
        healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
    }

    void Death()
    {
        switch (tag)
        {
            case "Player":
                PlayerDeath();
                break;

            default:
                Destroy(gameObject);
                break;
        }
    }

    void PlayerDeath()
    {
        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
        print("Death of player " + playerControllerId);
        FindObjectOfType<ServerManager>().Cmd_AddScore(playerControllerId, 1);
        for (int i = 0; i < Players.Length; i++)
        {
            Players[i].GetComponent<PlayerMove>().Respawn();
            Players[i].GetComponent<Health>().currentHealth = maxHealth;
        }
    }

}
