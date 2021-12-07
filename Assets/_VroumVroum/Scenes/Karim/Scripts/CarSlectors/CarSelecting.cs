using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class CarSelecting : MonoBehaviour
{
    public Rewired.Player rewiredPlayer;

    private bool isJoined = false;
    
    [HideInInspector] public int currentCarIndex = 0;
    [SerializeField] private GameObject[] carModels;

    private void Start()
    {
        foreach (GameObject car in carModels) car.SetActive(false);

        carModels[currentCarIndex].SetActive(true);
    }
    
    private void Update()
    {
        if (rewiredPlayer == null) return;

        if (!isJoined)
        {
            if (rewiredPlayer.GetButtonDown("Join")) isJoined = true;
            else return;
        }
        
        if (rewiredPlayer.GetButtonDown("LeftArrow")) ChangePrevious();
        if (rewiredPlayer.GetButtonDown("RightArrow")) ChangeNext();
    }

    public void InitReInput(int player)
    {
        rewiredPlayer = ReInput.players.GetPlayer(player);
    }
    
    public void ChangeNext()
    {
        carModels[currentCarIndex].SetActive(false);
        currentCarIndex++;
        
        if (currentCarIndex == carModels.Length) currentCarIndex = 0;
        
        carModels[currentCarIndex].SetActive(true);
    }

    public void ChangePrevious()
    {
        carModels[currentCarIndex].SetActive(false);
        currentCarIndex--;
        
        if (currentCarIndex == -1) currentCarIndex = carModels.Length -1;

        carModels[currentCarIndex].SetActive(true);
    }
}
