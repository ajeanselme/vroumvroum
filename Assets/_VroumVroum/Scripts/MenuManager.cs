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
    
    [SerializeField] private GameObject carPrefab;
    
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
            for (int i = 0; i < carMeshes.Count; i++)
            {
                carMeshes[i].transform.parent = null;
                DontDestroyOnLoad(carMeshes[i]);
            }
            
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
        _asc.completed -= InitializeGameScene;

        CarController[] cars = new CarController[carMeshes.Count];

        for (int i = 0; i < carMeshes.Count; i++)
        {
            SceneManager.MoveGameObjectToScene(carMeshes[i], SceneManager.GetActiveScene());

            GameObject nCar = Instantiate(carPrefab, CheckpointsController.instance.points[0].position, Quaternion.Euler(CheckpointsController.instance.points[0].rotation));
            CarController carController = nCar.GetComponent<CarController>();
            cars[i] = carController;
            
            carMeshes[i].transform.parent = nCar.transform; // change to the car prefab
            carMeshes[i].transform.localPosition = Vector3.zero;

            GameObject[] wheels = new GameObject[4];
            for (int x = 0; x < carMeshes[i].transform.childCount - 1; x++)
            {
                wheels[x] = carMeshes[i].transform.GetChild(x).gameObject;
            }
            carController.InitWheels(wheels);
            
            carController.InitReInput(i);
        }

        TurnManager.instance.SetPlayers(cars);
        
        Destroy(gameObject);
    }
}
