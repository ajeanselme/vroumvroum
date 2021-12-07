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

    private int turn = 0;
    private float startTimerBoostCar = 0f;
    private float timerBoostCar = 0f;
    private float timeBoostCar = 0f;

    private float zActualCam = 0f;
    private float halfTimer = 0f;
    public int indexCarTurn = 0;

    public CarController[] cars;

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
        endCamera.SetActive(false);
        
        //StartCoroutine(WaitLaunch(gameManager.m_Players[0].carController, 2f));
    }

    private void Update()
    {
        // Debug
        if (Input.GetKeyDown(KeyCode.Return))
        {
            FinishTurn(cars[indexCarTurn]);
        }

        if (timerBoostCar > 0f)
        {
            timerBoostCar -= Time.deltaTime;

            if (timerBoostCar > halfTimer)
            {
                float z = Mathf.Lerp(zActualCam, -12f, Mathf.Abs(timerBoostCar / startTimerBoostCar - 1f) * 2f);
                cars[indexCarTurn].vcam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z = z;
            }
            else
            {
                float z = Mathf.Lerp(-12f, zActualCam, Mathf.Abs(timerBoostCar / startTimerBoostCar - 0.5f) * 2f);
                cars[indexCarTurn].vcam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z = z;
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

    public void SetPlayers(CarController[] _players)
    {
        cars = _players;
        
        for (int i = 0; i < cars.Length; i++)
        {
            CheckpointsController.instance.InitPlayer();
        }
    }

    public void BoostCarEffects(float time)
    {
        halfTimer = time / 2f;
        timerBoostCar = time;
        startTimerBoostCar = time;
        speedParticles.Play();

        zActualCam = cars[indexCarTurn].vcam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z;
    }

    public void FinishTurn(CarController player)
    {
        if (speedParticles.isPlaying)
            speedParticles.Stop();
        
        cars[indexCarTurn].vcam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z = zActualCam;
        
        for (int i = 0; i < cars.Length; i++)
        {
            if (player.gameObject.GetInstanceID() == cars[indexCarTurn].gameObject.GetInstanceID())
            {
                player.stopCar();
                player.enabled = false;
                cars[indexCarTurn].vcam.gameObject.SetActive(false);

                if (i + 1 == cars.Length)
                {
                    turn++;
                    
                    if (turn >= maxTurn)
                    {
                        EndGame();
                    }
                    else
                    {
                        indexCarTurn = 0;
                        cars[0].enabled = true;
                        cars[0].vcam.gameObject.SetActive(true);
                        StartCoroutine(WaitLaunch(cars[0], 2f));
                    }
                }
                else
                {
                    if (!cars[i+1].gameObject.activeSelf)
                        cars[i+1].gameObject.SetActive(true);
                    
                    indexCarTurn = i+1;
                    cars[i+1].enabled = true;
                    cars[i+1].vcam.gameObject.SetActive(true);
                    StartCoroutine(WaitLaunch(cars[i+1], 2f));
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
        for (int i = 0; i < cars.Length; i++)
        {
            cars[i].enabled = true;
            cars[i].stopCar();
            cars[i].vcam.gameObject.SetActive(false);
        }
        
        endCamera.SetActive(true);
    }

    public CarController GetCurrentCar()
    {
        return cars[indexCarTurn];
    }
}
