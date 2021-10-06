using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;

    private int turn = 0;

    [SerializeField] private GameObject[] players;
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

    public void FinishTurn(GameObject player)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (player == players[i])
            {
                player.SetActive(false);

                if (i + 1 == players.Length)
                {
                    turn++;
                    players[0].SetActive(true);
                }
                else
                {
                    players[i].SetActive(true);
                }
                
                break;
            }
        }
    }
}
