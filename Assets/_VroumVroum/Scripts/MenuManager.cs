using System;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    private bool isGameLaunched = false;
    
    [SerializeField] private CarSelecting[] carSelectings;

    private List<GameObject> carMeshes;

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

    public void InitializeSelectionMenu()
    {
        if (carSelectings == null) return;
        
        for (int i = 0; i < carSelectings.Length; i++)
        {
            carSelectings[i].InitReInput(i);
        }
    }

    public bool LaunchGame()
    {
        if (isGameLaunched) return true;
        
        bool isReady = true;
        carMeshes = new List<GameObject>();

        for (int i = 0; i < carSelectings.Length; i++)
        {
            if (carSelectings[i].rewiredPlayer.controllers.joystickCount == 1 && !carSelectings[i].isLocked)
            {
                isReady = false;
                break;
            }
            
            if (carSelectings[i].rewiredPlayer.controllers.joystickCount == 1 && carSelectings[i].isLocked)
                carMeshes.Add(carSelectings[i].carModels[carSelectings[i].currentCarIndex].transform.GetChild(2).gameObject);
        }

        if (isReady)
        {
            LoadGameScene();
        }
        else
        {
            Debug.Log("One player or more are not ready");
        }

        return isReady;
    }
    
    private void LoadGameScene()
    {
        isGameLaunched = true;
        
        AsyncOperation asc = SceneManager.LoadSceneAsync(1);
        asc.completed += InitializeGameScene;
    }

    private void InitializeGameScene(AsyncOperation _asc)
    {
        if (_asc.isDone)
        {
            // Initialize game
            /*for (int i = 0; i < carMeshes.Count; i++)
            {
                Instantiate(carMeshes[i], Vector3.zero, Quaternion.identity);
            }*/
            
            Debug.Log(carMeshes.Count);
            GameObject.CreatePrimitive(PrimitiveType.Cube);
        }
    }
}
