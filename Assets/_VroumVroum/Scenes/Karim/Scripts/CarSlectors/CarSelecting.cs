using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;


public class CarSelecting : MonoBehaviour
{
    public Rewired.Player rewiredPlayer;
    [HideInInspector] public int currentCarIndex = 0;
    [SerializeField] private GameObject[] carModels;
    [Space]
    [SerializeField] private string previousKey = "q";
    [SerializeField] private string nextKey = "d";
    
    private void Start()
    {
        foreach (GameObject car in carModels) car.SetActive(false);
        carModels[currentCarIndex].SetActive(true);
        
        
    }
    
    private void Update()
    {
        
        if (rewiredPlayer == null) return;
        if (rewiredPlayer.GetButtonDown("LeftArrow")) ChangePrevious();
        if (rewiredPlayer.GetButtonDown("RightArrow")) ChangeNext();
<<<<<<< HEAD
        Debug.Log(rewiredPlayer.GetAxis("MoveLeftRight") + ", " + rewiredPlayer.id + ", " + rewiredPlayer.controllers.customControllerCount);
        //if (Input.GetKeyDown(previousKey)) ChangePrevious();
        //if (Input.GetKeyDown(nextKey)) ChangeNext();
=======
        
>>>>>>> felix
    }

    public void InitReInput(int playerID)
    {
        rewiredPlayer = ReInput.players.GetPlayer(playerID);
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
