using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using Rewired;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    public GameObject lastRoundPostit;
    public AudioSource lastRoundRing;

    public Text roundText;
    
    public CarController[] cars;

    [HideInInspector] public List<GameObject> carPrefabs = new List<GameObject>();
    
    [HideInInspector] public GameObject endCamera;
    [HideInInspector] public int maxTurn;
    [HideInInspector] public ParticleSystem speedParticles;

    private bool playLastRound;
    private bool playedLastRound;
    
    //debug
    [HideInInspector] public bool playMinigame = true;

    [Serializable]
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
        lastRoundPostit.SetActive(false);
        
        for (int i = 0; i < ReInput.players.playerCount; i++)
        {
            ReInput.players.Players[i].controllers.maps.SetMapsEnabled(false, "Menu");
            ReInput.players.Players[i].controllers.maps.SetMapsEnabled(true, "Default");
        }
        
        roundText.text = (turn + 1) + "/" + maxTurn;

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

    public void SetPlayers(CarController[] _players, string[] _carsColor)
    {
        cars = _players;
        
        for (int i = 0; i < cars.Length; i++)
        {
            CheckpointsController.instance.InitPlayer();
            CheckpointsController.instance.SetPlayerColor(i, _carsColor[i]);
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
                if (turn == maxTurn - 1)
                    if (!playedLastRound)
                        playLastRound = true;
                    
                
                indexCarTurn = 0;
                cars[0].enabled = true;
                cars[0].vcam.gameObject.SetActive(true);
                StartCoroutine(WaitLaunch(cars[0], 2f));

                roundText.text = (turn + 1) + "/" + maxTurn;
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

        if (playLastRound)
        {
            playLastRound = false;
            playedLastRound = true;
            
            PlayLastRound();
            yield return new WaitForSeconds(5f);
            lastRoundPostit.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(sec);
        }
        
        
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
            if (carsArray[i].transform.childCount == 7)
            {
                Destroy(carsArray[i].transform.GetChild(6).gameObject);
            }
        }
        
        endCamera.SetActive(true);
        
        // when the cam is arrived launch new scene
        EndGameManager.instance.EndGame(carsArray, CheckpointsController.instance.GetPlayerDataList());
    }

    public CarController GetCurrentCar()
    {
        return cars[indexCarTurn];
    }

    private void PlayLastRound()
    {
        Debug.Log("hmm");
        lastRoundPostit.SetActive(true);

        lastRoundPostit.transform.DOPunchScale(new Vector3(.3f, .3f, 0f), 3f);
        lastRoundPostit.transform.DOPunchRotation(new Vector3(0f, 0f, -30f), 3f, 20, 2f);
        
        lastRoundRing.Play();
    }
}
