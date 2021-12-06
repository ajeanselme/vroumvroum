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

    private ParsecGameManager gameManager;

    private int turn = 0;
    private float startTimerBoostCar = 0f;
    private float timerBoostCar = 0f;
    private float timeBoostCar = 0f;

    private float zActualCam = 0f;
    private float halfTimer = 0f;
    public int indexCarTurn = 0;

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
    }

    private void Start()
    {
        gameManager = ParsecGameManager.instance;
        
        endCamera.SetActive(false);

        for (int i = 0; i < gameManager.m_Players.Length; i++)
        {
            CheckpointsController.instance.InitPlayer();
        }

        StartCoroutine(WaitLaunch(gameManager.m_Players[0].carController, 2f));
    }

    private void Update()
    {
        // Debug
        if (Input.GetKeyDown(KeyCode.Return))
        {
            FinishTurn(gameManager.m_Players[indexCarTurn].carController);
        }

        if (timerBoostCar > 0f)
        {
            timerBoostCar -= Time.deltaTime;

            if (timerBoostCar > halfTimer)
            {
                float z = Mathf.Lerp(zActualCam, -12f, Mathf.Abs(timerBoostCar / startTimerBoostCar - 1f) * 2f);
                gameManager.m_Players[indexCarTurn].carController.vcam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z = z;
            }
            else
            {
                float z = Mathf.Lerp(-12f, zActualCam, Mathf.Abs(timerBoostCar / startTimerBoostCar - 0.5f) * 2f);
                gameManager.m_Players[indexCarTurn].carController.vcam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z = z;
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

        zActualCam = gameManager.m_Players[indexCarTurn].carController.vcam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z;
    }

    public void FinishTurn(CarController player)
    {
        if (speedParticles.isPlaying)
            speedParticles.Stop();
        
        gameManager.m_Players[indexCarTurn].carController.vcam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z = zActualCam;
        
        for (int i = 0; i < gameManager.m_Players.Length; i++)
        {
            if (player.gameObject.GetInstanceID() == gameManager.m_Players[i].carController.gameObject.GetInstanceID())
            {
                player.stopCar();
                player.enabled = false;
                gameManager.m_Players[i].carController.vcam.gameObject.SetActive(false);

                if (i + 1 == gameManager.m_Players.Length)
                {
                    turn++;
                    
                    if (turn >= maxTurn)
                    {
                        EndGame();
                    }
                    else
                    {
                        indexCarTurn = 0;
                        gameManager.m_Players[0].carController.enabled = true;
                        gameManager.m_Players[0].carController.vcam.gameObject.SetActive(true);
                        StartCoroutine(WaitLaunch(gameManager.m_Players[0].carController, 2f));
                    }
                }
                else
                {
                    if (!gameManager.m_Players[i+1].carController.gameObject.activeSelf)
                        gameManager.m_Players[i+1].carController.gameObject.SetActive(true);
                    
                    indexCarTurn = i+1;
                    gameManager.m_Players[i+1].carController.enabled = true;
                    gameManager.m_Players[i+1].carController.vcam.gameObject.SetActive(true);
                    StartCoroutine(WaitLaunch(gameManager.m_Players[i+1].carController, 2f));
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
        for (int i = 0; i < gameManager.m_Players.Length; i++)
        {
            gameManager.m_Players[i].carController.enabled = true;
            gameManager.m_Players[i].carController.stopCar();
            gameManager.m_Players[i].carController.vcam.gameObject.SetActive(false);
        }
        
        endCamera.SetActive(true);
    }

    public CarController GetCurrentCar()
    {
        return gameManager.m_Players[indexCarTurn].carController;
    }
}
