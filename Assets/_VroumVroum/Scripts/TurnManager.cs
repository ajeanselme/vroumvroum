using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;

    public Minigame minigame;

    private int turn = 0;
    private float startTimerBoostCar;
    private float timerBoostCar;

    private float zActualCam;
    private float halfTimer;
    public int indexCarTurn;

    public CarController[] cars;

    [HideInInspector] public List<GameObject> carPrefabs = new List<GameObject>();
    
    [HideInInspector] public GameObject endCamera;
    [HideInInspector] public int maxTurn;
    [HideInInspector] public ParticleSystem speedParticles;
    
    //debug
    [HideInInspector] public bool playMinigame = true;
    
    [System.Serializable]
    public struct MyStruct
    {
        public int test;
    }

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
        
        StartCoroutine(WaitLaunch(cars[0], 2f));
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

        player.stopCar();
        player.enabled = false;
        cars[indexCarTurn].vcam.gameObject.SetActive(false);

        indexCarTurn++;

        if (indexCarTurn >= cars.Length)
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
            if (!cars[indexCarTurn].gameObject.activeSelf)
                cars[indexCarTurn].gameObject.SetActive(true);

            cars[indexCarTurn].enabled = true;
            cars[indexCarTurn].vcam.gameObject.SetActive(true);
            StartCoroutine(WaitLaunch(cars[indexCarTurn], 2f));
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
            player.launchCar(player.totalTime);
        }
    }

    private void EndGame()
    {
        GameObject[] carsArray = new GameObject[cars.Length];
        
        for (int i = 0; i < cars.Length; i++)
        {
            cars[i].enabled = true;
            cars[i].stopCar();
            cars[i].vcam.gameObject.SetActive(false);
            carsArray[i] = cars[i].gameObject.transform.GetChild(cars[i].gameObject.transform.childCount - 1).gameObject;
        }
        
        endCamera.SetActive(true);
        
        // when the cam is arrived launch new scene
        EndGameManager.instance.EndGame(carsArray, CheckpointsController.instance.GetPlayerDataList());
    }

    public CarController GetCurrentCar()
    {
        return cars[indexCarTurn];
    }
}
