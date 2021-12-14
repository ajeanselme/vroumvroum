using System;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Player = Rewired.Player;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    [HideInInspector] public bool isGameLaunched = false;

    private string[] carsColor;
    
    [SerializeField] private GameObject carPrefab;
    
    [SerializeField] private CarSelecting[] carSelectings;

    [Space]
    [SerializeField] private GameObject selectionScreen;
    [SerializeField] private GameObject loadingScreen;

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

    private void Start()
    {
        if (CrownManager.instance != null)
            CrownManager.instance.isSelectingLoaded = true;
        
        for (int i = 0; i < ReInput.players.playerCount; i++)
        {
            ReInput.players.Players[i].controllers.maps.SetMapsEnabled(false, "Default");
            ReInput.players.Players[i].controllers.maps.SetMapsEnabled(true, "Menu");
        }
    }

    private void Update()
    {
        if (isGameLaunched) return;

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

    public GameObject IsPlayerSet(Rewired.Player _player)
    {
        for (int i = 0; i < slotsSet.Length; i++)
        {
            if (slotsSet[i] != null && _player.id == slotsSet[i].id)
            {
                return carSelectings[i].gameObject;
            }
        }

        return null;
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

    private bool LaunchGame()
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
            carsColor = new string[carMeshes.Count];
            
            for (int i = 0; i < carMeshes.Count; i++)
            {
                carMeshes[i].mesh.transform.parent = null;
                CarSelecting crs = carMeshes[i].mesh.GetComponent<CarSelecting>();
                carsColor[i] = ColorManager.instance.colorChanger[crs.colorIndex].color;
                Destroy(crs);
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
        asc.allowSceneActivation = false;
        asc.completed += InitializeGameScene;
        StartCoroutine(LoadingScreen(asc));
    }

    private IEnumerator LoadingScreen(AsyncOperation _asc)
    {
        selectionScreen.SetActive(false);
        loadingScreen.SetActive(true);

        Text nText = loadingScreen.transform.GetChild(1).gameObject.GetComponent<Text>();
        int nbPoint = 0;
        string pointText = "";
        
        while (_asc.progress < 0.9f)
        {
            nText.text = "Rewinding Cars" + pointText;
            yield return new WaitForSeconds(0.25f);

            pointText += ".";
            nbPoint += 1;

            if (nbPoint > 3)
            {
                nbPoint = 0;
                pointText = "";
            }
        }

        loadingScreen.transform.GetChild(1).gameObject.SetActive(false); // text Loading
        loadingScreen.transform.GetChild(2).gameObject.SetActive(true); // text Press start to continue

        yield return new WaitUntil(() =>
        {
            bool rValue = false;
            
            for (int i = 0; i < ReInput.players.playerCount; i++)
            {
                rValue = ReInput.players.Players[i].GetButtonDown("Join");
                
                if (rValue)
                    break;
            }

            return rValue;
        });
        
        _asc.allowSceneActivation = true;
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
            carController.carKey = carMeshes[i].mesh.transform.GetChild(1).gameObject;
            
            carMeshes[i].mesh.transform.parent = nCar.transform; // change to the car prefab
            carMeshes[i].mesh.transform.localPosition = Vector3.zero;
            carMeshes[i].mesh.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

            GameObject[] wheels = new GameObject[4];
            for (int x = 2; x < 6; x++)
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

        TurnManager.instance.SetPlayers(cars, carsColor);
        
        Destroy(gameObject);
    }
}
