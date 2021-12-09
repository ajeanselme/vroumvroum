using System;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        Application.targetFrameRate = 60;
    }

    private void Update()
    {
        if (ReInput.players.GetPlayer(0).GetAnyButton())
        {
            LoadSelectionScene();
        }
    }

    private void LoadSelectionScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
