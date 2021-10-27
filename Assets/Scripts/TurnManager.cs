using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;

    private int turn = 0;
    private int indexCarTurn = 0;
    
    private Player[] rewiredPlayers;
    [Space]

    [SerializeField] private CarController[] players;
    [SerializeField] private GameObject[] playersCamera;
    [Space]
    [SerializeField] private GameObject endCamera;
    [Space]
    [SerializeField] private int maxTurn;
    [Space]
    public ParticleSystem speedParticles;

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

        Debug.Log(ReInput.players.playerCount);
        rewiredPlayers = new Player[players.Length];
        
        for (int i = 0; i < 2; i++)
        {
            rewiredPlayers[i] = ReInput.players.GetPlayer(i);
        }
        
        for (int i = 1; i < players.Length; i++)
        {
            players[i].stopCar();
            players[i].gameObject.SetActive(false);
            playersCamera[i].SetActive(false);
        }

        StartCoroutine(WaitLaunch(players[0], 2f));
    }

    private void Update()
    {
        // Debug
        if (rewiredPlayers[indexCarTurn].GetButtonDown("QTE"))
            Debug.Log("QTE + " + rewiredPlayers[indexCarTurn] + ", player" + indexCarTurn);
        
        // Debug
        if (Input.GetKeyDown(KeyCode.Return))
        {
            FinishTurn(players[indexCarTurn]);
        }
    }

    public void FinishTurn(CarController player)
    {
        if (speedParticles.isPlaying)
            speedParticles.Stop();
        
        for (int i = 0; i < players.Length; i++)
        {
            if (player == players[i])
            {
                player.stopCar();
                player.enabled = false;
                playersCamera[i].SetActive(false);

                if (i + 1 == players.Length)
                {
                    turn++;
                    
                    if (turn >= maxTurn)
                    {
                        EndGame();
                    }
                    else
                    {
                        indexCarTurn = 0;
                        players[0].enabled = true;
                        playersCamera[0].SetActive(true);
                        StartCoroutine(WaitLaunch(players[0], 2f));
                    }
                }
                else
                {
                    if (!players[i+1].gameObject.activeSelf)
                        players[i+1].gameObject.SetActive(true);
                    
                    indexCarTurn = i+1;
                    players[i+1].enabled = true;
                    playersCamera[i+1].SetActive(true);
                    StartCoroutine(WaitLaunch(players[i+1], 2f));
                }
                
                break;
            }
        }
    }

    IEnumerator WaitLaunch(CarController player, float sec)
    {
        player.stopCar();
        
        yield return new WaitForSeconds(sec);
        
        player.launchCar();
        speedParticles.Play();
    }

    private void EndGame()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].enabled = true;
            players[i].stopCar();
            playersCamera[i].SetActive(false);
        }
        
        endCamera.SetActive(true);
    }
}
