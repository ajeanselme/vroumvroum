using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
public class SpawnManager : MonoBehaviour
{
    public GameObject[] spawnPoints;
    public GameObject player;
    public GameObject player2;

    private float spawnTime = 3.0f;
    private bool playerSpawn = false;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerSpawn) { 
            spawnTime -= Time.deltaTime; 
        }
        

        if (spawnTime <= 0f)
        {
            playerSpawn = false;
            spawnTime = 3.0f;
            Instantiate(player2, spawnPoints[1].transform.position, Quaternion.identity);
            CameraShaker.Instance.ShakeOnce(4f, 4f, .1f, 1f);
        }
    }
    public void CreatePlayer()
    {

       playerSpawn = true;
        Instantiate(player, spawnPoints[0].transform.position, Quaternion.identity);
            CameraShaker.Instance.ShakeOnce(4f, 4f, .1f, 1f);
        
        
       
    }
   
}
