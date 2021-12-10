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

    [SerializeField] private float waitTransitionNextScene;
    
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

        StartCoroutine(IELaunchEndScene());
    }

    private void EndSceneInitialization()
    {
        for (int i = 0; i < carsPodium.Length; i++)
        {
            SceneManager.MoveGameObjectToScene(carsPodium[i], SceneManager.GetActiveScene());
            carsPodium[i].transform.rotation = Quaternion.Euler(Vector3.zero);
            carsPodium[i].transform.localScale = Vector3.one;
        }
        
        End endScript = End.instance;
        endScript.SetEnd(carsPodium, carsDistance);
        
        Destroy(gameObject);
    }

    private IEnumerator IELaunchEndScene()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex+1);
        asyncOperation.allowSceneActivation = false;

        yield return new WaitForSeconds(waitTransitionNextScene);

        asyncOperation.allowSceneActivation = true;
        
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
        
        EndSceneInitialization();
    }
}
