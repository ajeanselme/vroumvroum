using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;

    private int turn = 0;
    private int indexCarTurn = 0;

    [SerializeField] private CarController[] players;
    [SerializeField] private GameObject[] playersCamera;
    [Space]
    [SerializeField] private int maxTurn;

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
        for (int i = 1; i < players.Length; i++)
        {
            players[i].gameObject.SetActive(false);
            playersCamera[i].SetActive(false);
        }
    }

    private void Update()
    {
        // Debug
        if (Input.GetKeyDown(KeyCode.Return))
        {
            FinishTurn(players[indexCarTurn]);
        }
    }

    public void FinishTurn(CarController player)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (player == players[i])
            {
                player.enabled = false;
                playersCamera[i].SetActive(false);

                if (i + 1 == players.Length)
                {
                    turn++;
                    indexCarTurn = 0;
                    players[0].enabled = true;
                    playersCamera[0].SetActive(true);
                }
                else
                {
                    if (!players[i+1].gameObject.activeSelf)
                        players[i+1].gameObject.SetActive(true);
                    
                    players[i+1].enabled = true;
                    playersCamera[i+1].SetActive(true);
                    indexCarTurn = i+1;
                }
                
                break;
            }
        }
    }
}
