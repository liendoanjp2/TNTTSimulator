using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    private static GameObject player;
    public GameObject playerPrefab;

    // use this if we do not want to set the player position in prefab
    public Vector3 startingPoint = new Vector3(0, 0, 0);

    GameObject getPlayer()
    {
        GameObject player = GameObject.Find("Player");
        if (!player)
        {
            player = Instantiate(playerPrefab.gameObject);
            player.transform.Find("Character").position = startingPoint;
            player.name = "Player";
        }
        
        return player;
    }

    // Start is called before the first frame update
    void Start()
    {
        getPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
