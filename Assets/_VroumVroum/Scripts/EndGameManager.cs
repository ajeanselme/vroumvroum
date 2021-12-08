using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameManager : MonoBehaviour
{
    public static EndGameManager instance;

    private GameObject[] carsPodium;
    private float[] carsDistance;
    
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

    public void EndGame(GameObject[] cars, List<CheckpointsController.PlayerData> playerDatas)
    {
        carsPodium = new GameObject[cars.Length];
        carsDistance = new float[cars.Length];
        for (int i = 0; i < carsPodium.Length; i++)
        {
            carsPodium[playerDatas[i].ladderPosition - 1] = cars[playerDatas[i].index];
            cars[playerDatas[i].index].transform.parent = null;
            DontDestroyOnLoad(cars[playerDatas[i].index]);

            carsDistance[playerDatas[i].ladderPosition - 1] = playerDatas[i].CurrentDistance;
        }
        
        LaunchEndScene();
    }
    
    private void LaunchEndScene()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex+1);
        asyncOperation.completed += EndSceneInitialization;
    }

    private void EndSceneInitialization(AsyncOperation arg)
    {
        arg.completed -= EndSceneInitialization;

        for (int i = 0; i < carsPodium.Length; i++)
        {
            SceneManager.MoveGameObjectToScene(carsPodium[i], SceneManager.GetActiveScene());
            carsPodium[i].transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            carsPodium[i].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
        
        End endScript = End.instance;
        endScript.SetEnd(carsPodium, carsDistance);
        
        Destroy(gameObject);
    }
}
