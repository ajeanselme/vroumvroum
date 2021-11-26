using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSelecting : MonoBehaviour
{
    public int currentCarIndex;
    public GameObject[] carModels;
    public PlayerPrefs GetPlayerPrefs;
    
    void Start()
    {
      
        currentCarIndex = PlayerPrefs.GetInt("SelectedCarP1", 0);
        currentCarIndex = PlayerPrefs.GetInt("SelectedCarP2", 0);
        currentCarIndex = PlayerPrefs.GetInt("SelectedCarP3", 0);
        currentCarIndex = PlayerPrefs.GetInt("SelectedCarP4", 0);

        foreach (GameObject car in carModels) car.SetActive(false);

        carModels[currentCarIndex].SetActive(true);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ChangePreviousP1();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            ChangeNextP1();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ChangePreviousP2();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            ChangeNextP2();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            ChangePreviousP3();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            ChangeNextP3();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            ChangePreviousP4();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            ChangeNextP4();
        }
    }


    public void ChangeNextP1()
    {
        carModels[currentCarIndex].SetActive(false);
        currentCarIndex++;
        if (currentCarIndex == carModels.Length) currentCarIndex = 0;
        carModels[currentCarIndex].SetActive(true);
        PlayerPrefs.SetInt("SelectedCarP1", currentCarIndex);
        PlayerPrefs.Save();
    }

    public void ChangePreviousP1()
    {
        carModels[currentCarIndex].SetActive(false);
        currentCarIndex--;
        if (currentCarIndex == -1) currentCarIndex = carModels.Length -1;

        carModels[currentCarIndex].SetActive(true);
        PlayerPrefs.SetInt("SelectedCarP1", currentCarIndex);
        PlayerPrefs.Save();
        
    }

    public void ChangeNextP2()
    {
        carModels[currentCarIndex].SetActive(false);
        currentCarIndex++;
        if (currentCarIndex == carModels.Length) currentCarIndex = 0;
        carModels[currentCarIndex].SetActive(true);
        PlayerPrefs.SetInt("SelectedCarP2", currentCarIndex);
        PlayerPrefs.Save();
    }



    public void ChangePreviousP2()
    {
        carModels[currentCarIndex].SetActive(false);
        currentCarIndex--;
        if (currentCarIndex == -1) currentCarIndex = carModels.Length - 1;

        carModels[currentCarIndex].SetActive(true);
        PlayerPrefs.SetInt("SelectedCarP2", currentCarIndex);
        PlayerPrefs.Save();
        
    }


    public void ChangeNextP3()
    {
        carModels[currentCarIndex].SetActive(false);
        currentCarIndex++;
        if (currentCarIndex == carModels.Length) currentCarIndex = 0;
        carModels[currentCarIndex].SetActive(true);
        PlayerPrefs.SetInt("SelectedCarP3", currentCarIndex);
        PlayerPrefs.Save();
    }

    public void ChangePreviousP3()
    {
        carModels[currentCarIndex].SetActive(false);
        currentCarIndex--;
        if (currentCarIndex == -1) currentCarIndex = carModels.Length - 1;

        carModels[currentCarIndex].SetActive(true);
        PlayerPrefs.SetInt("SelectedCarP3", currentCarIndex);
        PlayerPrefs.Save();
        
    }


    public void ChangeNextP4()
    {
        carModels[currentCarIndex].SetActive(false);
        currentCarIndex++;
        if (currentCarIndex == carModels.Length) currentCarIndex = 0;
        carModels[currentCarIndex].SetActive(true);
        PlayerPrefs.SetInt("SelectedCarP4", currentCarIndex);
        PlayerPrefs.Save();
    }

    public void ChangePreviousP4()
    {
        carModels[currentCarIndex].SetActive(false);
        currentCarIndex--;
        if (currentCarIndex == -1) currentCarIndex = carModels.Length - 1;

        carModels[currentCarIndex].SetActive(true);
        PlayerPrefs.SetInt("SelectedCarP4", currentCarIndex);
        PlayerPrefs.Save();
        
    }
   

}
