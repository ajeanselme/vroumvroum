using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;

    public Minigame minigame; 

    [Serializable]
    public class Player
    {
        public CarController carController;
        public Rewired.Player rewiredPlayer;
        public int prefabIndex;
    }
    
    private int turn = 0;
    public int indexCarTurn = 0;

    [HideInInspector] public List<Player> playerList = new List<Player>();
    [HideInInspector] public List<GameObject> carPrefabs = new List<GameObject>();
    
    [HideInInspector] public GameObject endCamera;
    [HideInInspector] public Transform spawnPoint;
    [HideInInspector] public int maxTurn;
    [HideInInspector] public ParticleSystem speedParticles;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        endCamera.SetActive(false);

        for (int i = 0; i < playerList.Count; i++)
        {
            CheckpointsController.instance.InitPlayer();
        }
        
        for (int i = 1; i < playerList.Count; i++)
        {
            playerList[i].rewiredPlayer = ReInput.players.GetPlayer(i);
            playerList[i].carController.stopCar();
            playerList[i].carController.gameObject.SetActive(false);
            playerList[i].carController.vcam.gameObject.SetActive(false);
        }

        StartCoroutine(WaitLaunch(playerList[0].carController, 2f));
    }

    private void Update()
    {
        // Debug
        if (playerList[indexCarTurn].rewiredPlayer.GetButtonDown("Cross"))
            Debug.Log("QTE + " + playerList[indexCarTurn].rewiredPlayer + ", Cross");
        if (playerList[indexCarTurn].rewiredPlayer.GetButtonDown("Circle"))
            Debug.Log("QTE + " + playerList[indexCarTurn].rewiredPlayer + ", Circle");
        if (playerList[indexCarTurn].rewiredPlayer.GetButtonDown("Triangle"))
            Debug.Log("QTE + " + playerList[indexCarTurn].rewiredPlayer + ", Triangle");
        if (playerList[indexCarTurn].rewiredPlayer.GetButtonDown("Square"))
            Debug.Log("QTE + " + playerList[indexCarTurn].rewiredPlayer + ", Square");
        if (playerList[indexCarTurn].rewiredPlayer.GetButtonDown("Start"))
            Debug.Log("Start");
        
        // Debug
        if (Input.GetKeyDown(KeyCode.Return))
        {
            FinishTurn(playerList[indexCarTurn].carController);
        }
    }

    public void FinishTurn(CarController player)
    {
        if (speedParticles.isPlaying)
            speedParticles.Stop();
        
        for (int i = 0; i < playerList.Count; i++)
        {
            if (player.gameObject.GetInstanceID() == playerList[i].carController.gameObject.GetInstanceID())
            {
                player.stopCar();
                player.enabled = false;
                playerList[i].carController.vcam.gameObject.SetActive(false);

                if (i + 1 == playerList.Count)
                {
                    turn++;
                    
                    if (turn >= maxTurn)
                    {
                        EndGame();
                    }
                    else
                    {
                        indexCarTurn = 0;
                        playerList[0].carController.enabled = true;
                        playerList[0].carController.vcam.gameObject.SetActive(true);
                        StartCoroutine(WaitLaunch(playerList[0].carController, 2f));
                    }
                }
                else
                {
                    if (!playerList[i+1].carController.gameObject.activeSelf)
                        playerList[i+1].carController.gameObject.SetActive(true);
                    
                    indexCarTurn = i+1;
                    playerList[i+1].carController.enabled = true;
                    playerList[i+1].carController.vcam.gameObject.SetActive(true);
                    StartCoroutine(WaitLaunch(playerList[i+1].carController, 2f));
                }
                
                break;
            }
        }
    }

    IEnumerator WaitLaunch(CarController player, float sec)
    {
        player.stopCar();
        CheckpointsController.instance.LoadPlayer(indexCarTurn);

        yield return new WaitForSeconds(sec);
        speedParticles.Play();

        minigame.beginMinigame(player);
    }

    private void EndGame()
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            playerList[i].carController.enabled = true;
            playerList[i].carController.stopCar();
            playerList[i].carController.vcam.gameObject.SetActive(false);
        }
        
        endCamera.SetActive(true);
    }
}
