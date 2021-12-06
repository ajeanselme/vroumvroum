using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P4 : MonoBehaviour
{
    public int currentCarIndex;
    public GameObject[] carModels;

    void Start()
    {
        currentCarIndex = PlayerPrefs.GetInt("SelectedCarP4", 0);
        foreach (GameObject car in carModels) car.SetActive(false);

        carModels[currentCarIndex].SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            ChangePrevious();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            ChangeNext();
        }
    }

    public void ChangeNext()
    {
        carModels[currentCarIndex].SetActive(false);
        currentCarIndex++;
        if (currentCarIndex == carModels.Length) currentCarIndex = 0;

        carModels[currentCarIndex].SetActive(true);
        PlayerPrefs.SetInt("SelectedCarP4", currentCarIndex);
        PlayerPrefs.Save();
    }

    public void ChangePrevious()
    {
        carModels[currentCarIndex].SetActive(false);
        currentCarIndex--;
        if (currentCarIndex == -1) currentCarIndex = carModels.Length - 1;

        carModels[currentCarIndex].SetActive(true);
        PlayerPrefs.SetInt("SelectedCarP4", currentCarIndex);
        PlayerPrefs.Save();
        Debug.LogError("Previous");
    }
}