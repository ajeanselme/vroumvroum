using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P3 : MonoBehaviour
{
    public int currentCarIndex;
    public GameObject[] carModels;

    void Start()
    {
        currentCarIndex = PlayerPrefs.GetInt("SelectedCarP3", 0);
        foreach (GameObject car in carModels) car.SetActive(false);

        carModels[currentCarIndex].SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ChangePrevious();
        }
        if (Input.GetKeyDown(KeyCode.D))
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
        PlayerPrefs.SetInt("SelectedCarP3", currentCarIndex);
        PlayerPrefs.Save();
    }

    public void ChangePrevious()
    {
        carModels[currentCarIndex].SetActive(false);
        currentCarIndex--;
        if (currentCarIndex == -1) currentCarIndex = carModels.Length - 1;

        carModels[currentCarIndex].SetActive(true);
        PlayerPrefs.SetInt("SelectedCarP3", currentCarIndex);
        PlayerPrefs.Save();
        Debug.LogError("Previous");
    }
}
