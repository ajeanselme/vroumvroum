using System;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using UnityEngine.SceneManagement;
using Player = Rewired.Player;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    private bool isGameLaunched = false;
    
    [SerializeField] private GameObject carPrefab;
    
    [SerializeField] private CarSelecting[] carSelectings;

    private Rewired.Player[] slotsSet = new Rewired.Player[4];

    private List<PlayerStruct> carMeshes;

    public delegate void OnGameSceneLaunched();
    public event OnGameSceneLaunched onGameSceneLaunched;

    private struct PlayerStruct
    {
        public GameObject mesh;
        public Rewired.Player player;
    }

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

    private void Update()
    {
        for (int i = 0; i < ReInput.players.playerCount; i++)
        {
            if (ReInput.players.GetPlayer(i).GetButtonDown("Join"))
            {
                SetPlayerSlot(ReInput.players.GetPlayer(i));
            }
        }

        for (int i = 0; i < slotsSet.Length; i++)
        {
            if (slotsSet[i] != null)
            {
                if (carSelectings[i].isLocked && slotsSet[i].GetButtonDown("Join"))
                {
                    LaunchGame();
                }
                
                break;
            }
        }
    }

    private void SetPlayerSlot(Rewired.Player _player)
    {
        if (Array.IndexOf(slotsSet, _player) > -1) return; // Already register

        for (int i = 0; i < slotsSet.Length; i++)
        {
            if (slotsSet[i] == null)
            {
                slotsSet[i] = _player;
                carSelectings[i].PlayerJoin(_player);
                break;
            }
        }
    }

    public void UnsetPlayerSlot(Rewired.Player _player)
    {
        int index = Array.IndexOf(slotsSet, _player);
        
        if (index == -1) return;

        slotsSet[index] = null;
        carSelectings[index].PlayerLeave(_player);
    }

    public bool LaunchGame()
    {
        if (isGameLaunched) return true;
        
        bool isReady = true;
        carMeshes = new List<PlayerStruct>();

        for (int i = 0; i < slotsSet.Length; i++)
        {
            if (slotsSet[i] != null)
            {
                if (carSelectings[i].isLocked)
                {
                    carMeshes.Add(new PlayerStruct{mesh = carSelectings[i].gameObject, player = slotsSet[i]} );
                }
                else
                {
                    isReady = false;
                    break;
                }
            }
        }

        /*for (int i = 0; i < carSelectings.Length; i++)
        {
            if (carSelectings[i].rewiredPlayer.controllers.joystickCount == 1 && !carSelectings[i].isLocked)
            {
                isReady = false;
                break;
            }
            
            if (carSelectings[i].rewiredPlayer.controllers.joystickCount == 1 && carSelectings[i].isLocked)
                carMeshes.Add(carSelectings[i].carModels[carSelectings[i].currentCarIndex].transform.GetChild(2).gameObject);
        }*/

        if (isReady)
        {
            for (int i = 0; i < carMeshes.Count; i++)
            {
                carMeshes[i].mesh.transform.parent = null;
                Destroy(carMeshes[i].mesh.GetComponent<CarSelecting>());
                DontDestroyOnLoad(carMeshes[i].mesh);
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
        
        AsyncOperation asc = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex+1);
        asc.completed += InitializeGameScene;
    }

    private void InitializeGameScene(AsyncOperation _asc)
    {
        _asc.completed -= InitializeGameScene;

        onGameSceneLaunched();
        
        CarController[] cars = new CarController[carMeshes.Count];

        for (int i = 0; i < carMeshes.Count; i++)
        {
            SceneManager.MoveGameObjectToScene(carMeshes[i].mesh, SceneManager.GetActiveScene());

            GameObject nCar = Instantiate(carPrefab, CheckpointsController.instance.points[0].position, Quaternion.Euler(CheckpointsController.instance.points[0].rotation));
            CarController carController = nCar.GetComponent<CarController>();
            cars[i] = carController;
            
            carMeshes[i].mesh.transform.parent = nCar.transform; // change to the car prefab
            carMeshes[i].mesh.transform.localPosition = new Vector3(0f, 0.3f, 0f);
            carMeshes[i].mesh.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

            GameObject[] wheels = new GameObject[4];
            for (int x = 2; x < carMeshes[i].mesh.transform.childCount; x++)
            {
                wheels[x - 2] = carMeshes[i].mesh.transform.GetChild(x).gameObject;
            }
            carController.InitWheels(wheels);
            
            carController.InitReInput(carMeshes[i].player.id);
            carController.enabled = false;
            
            nCar.SetActive(false);
        }

        cars[0].gameObject.SetActive(true);
        cars[0].enabled = true;

        TurnManager.instance.SetPlayers(cars);
        
        Destroy(gameObject);
    }
}
