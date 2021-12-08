using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Rewired;
using UnityEditor;
using UnityEngine.SceneManagement;

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
    private float startTimerBoostCar;
    private float timerBoostCar;

    private float zActualCam;
    private float halfTimer;
    public int indexCarTurn;

    [HideInInspector] public List<Player> playerList = new List<Player>();
    [HideInInspector] public List<GameObject> carPrefabs = new List<GameObject>();
    
    [HideInInspector] public GameObject endCamera;
    [HideInInspector] public int maxTurn;
    [HideInInspector] public ParticleSystem speedParticles;
    
    //debug
    [HideInInspector] public bool playMinigame = true;

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

        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        endCamera.SetActive(false);

        for (int i = 0; i < playerList.Count; i++)
        {
            playerList[i].rewiredPlayer = ReInput.players.GetPlayer(i);
            playerList[i].carController.rewiredPlayer = playerList[i].rewiredPlayer;
            CheckpointsController.instance.InitPlayer();
        }
        
        for (int i = 1; i < playerList.Count; i++)
        {
            playerList[i].carController.stopCar();
            playerList[i].carController.gameObject.SetActive(false);
            playerList[i].carController.vcam.gameObject.SetActive(false);
        }

        StartCoroutine(WaitLaunch(playerList[0].carController, 2f));
    }

    private void Update()
    {
        // Debug
        if (Input.GetKeyDown(KeyCode.Return))
        {
            FinishTurn(playerList[indexCarTurn].carController);
        }

        if (timerBoostCar > 0f)
        {
            timerBoostCar -= Time.deltaTime;

            if (timerBoostCar > halfTimer)
            {
                float z = Mathf.Lerp(zActualCam, -12f, Mathf.Abs(timerBoostCar / startTimerBoostCar - 1f) * 2f);
                playerList[indexCarTurn].carController.vcam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z = z;
            }
            else
            {
                float z = Mathf.Lerp(-12f, zActualCam, Mathf.Abs(timerBoostCar / startTimerBoostCar - 0.5f) * 2f);
                playerList[indexCarTurn].carController.vcam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z = z;
            }
        }
        else if (timerBoostCar <= 0f && speedParticles.isPlaying)
        {
            speedParticles.Stop();
            // stop cam movement
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void BoostCarEffects(float time)
    {
        halfTimer = time / 2f;
        timerBoostCar = time;
        startTimerBoostCar = time;
        speedParticles.Play();

        zActualCam = playerList[indexCarTurn].carController.vcam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z;
    }

    public void FinishTurn(CarController player)
    {
        if (speedParticles.isPlaying)
            speedParticles.Stop();
        
        playerList[indexCarTurn].carController.vcam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z = zActualCam;
        
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

        EventsManager.instance.isOn = false;
    }

    IEnumerator WaitLaunch(CarController player, float sec)
    {
        player.stopCar();
        CheckpointsController.instance.LoadPlayer(indexCarTurn);

        yield return new WaitForSeconds(sec);

        if (playMinigame)
        {
            minigame.beginMinigame(player);
        }
        else
        {
            player.launchCar();
        }
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

    public Player GetCurrentPlayer()
    {
        return playerList[indexCarTurn];
    }
}
