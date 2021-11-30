using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;
    public Vector3 spawnOrigin;
    public float timer;
    public List<Player> players;


    void Start()
    {
        players = new List<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
         {
            SpawnPlayers();
         } 
    }

    
    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(instance.gameObject);

        }

        else
        {
            instance = this;
        }
    }

    public void AddPlayerToEndScreen(GameObject car, int placement, float score)
    {
        Player newPlayer = new Player();
        newPlayer.gameObject = car;
        newPlayer.placement = placement;
        newPlayer.score = score;
    }

    private void ClassPlayers()
    {
        List<Player> rankedPlayers = new List<Player>(4);

        foreach (Player player in players)
        {
            rankedPlayers[player.placement] = player;
        }

        players = rankedPlayers;
    }

    public void SpawnPlayers()
    {
        ClassPlayers();
        StartCoroutine(ISpawnPlayer());

    }

    private IEnumerator ISpawnPlayer()
    {
        for (int i = players.Count-1; i >= 0; ++i) 
        {
            GameObject newPlayerObjectToSpawn = players[i].gameObject;
            GameObject.Instantiate(newPlayerObjectToSpawn, spawnOrigin, newPlayerObjectToSpawn.transform.rotation);
            CameraShaker.Instance.ShakeOnce(4f, 4f, .1f, 1f);
            yield return new WaitForSeconds(timer);

        }
        StopCoroutine("ISpawnPlayer");
    }

    
}
