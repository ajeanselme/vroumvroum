using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.UI;

public class CarSelecting : MonoBehaviour
{
    public Rewired.Player rewiredPlayer;

    private bool isJoined = false;
    [HideInInspector] public bool isLocked = false;

    [HideInInspector] public int currentCarIndex = 0;
    [HideInInspector] public GameObject[] carModels;
    [SerializeField] private Text statusText;

    private void Start()
    {
        foreach (GameObject car in carModels) car.SetActive(false);

        carModels[currentCarIndex].SetActive(true);

        statusText.text = "To Join";
    }
    
    private void Update()
    {
        if (rewiredPlayer == null) return;

        if (!isJoined)
        {
            if (rewiredPlayer.GetButtonDown("Join"))
            {
                isJoined = true;
                statusText.text = "";
            }
            else return;
        }

        if (rewiredPlayer.GetButtonDown("Lock"))
        {
            isLocked = !isLocked;
            statusText.text = isLocked ? "Ready" : "";
        }
        
        if (isLocked && rewiredPlayer.id != 0) return;
        else if (isLocked && rewiredPlayer.id == 0)
        {
            if (rewiredPlayer.GetButtonDown("Join"))
            {
                MenuManager.instance.LaunchGame();
            }
            return;
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