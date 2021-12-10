using System;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;
    
    private bool td = false;
    private bool isLoading = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            td = true;
            return;
        }
        
        Application.targetFrameRate = 60;
    }

    private void Update()
    {
        if (td || isLoading) return;
        
        for (int i = 0; i < ReInput.players.playerCount; i++)
        {
            if (ReInput.players.GetPlayer(i).GetAnyButton())
            {
                LoadSelectionScene();
            }
        }
    }

    private void LoadSelectionScene()
    {
        isLoading = true;
        
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        asyncOperation.completed += SelectingSceneLaunched;
    }

    private void SelectingSceneLaunched(AsyncOperation obj)
    {
        obj.completed -= SelectingSceneLaunched;
        
        MenuManager.instance.onGameSceneLaunched += ToDestroy;
    }
    

    private void ToDestroy()
    {
        MenuManager.instance.onGameSceneLaunched -= ToDestroy;
        Destroy(gameObject);
    }
}
