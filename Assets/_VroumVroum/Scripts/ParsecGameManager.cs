using System;
using System.Collections;
using UnityEngine;
using ParsecGaming;
using Rewired;
using UnityEngine.SceneManagement;

public class ParsecGameManager : MonoBehaviour
{
    public static ParsecGameManager instance;

    private UInt16[] playerCarIndex;
    
    private TurnManager turnManager;

    public CarSelecting[] slotSelection;
    public PlayerManager[] m_Players;
    
    private ParsecStreamGeneral streamer;
    private ParsecUnity.API.SessionResultDataData authData;
    [NonSerialized] private bool initialized = false;

    [SerializeField] private GameObject authGo;
    [SerializeField] private GameObject controlGo;
    
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        streamer = FindObjectOfType<ParsecStreamGeneral>();
        if (streamer != null)
        {
            streamer.GuestConnected += Streamer_GuestConnected;
            streamer.GuestDisconnected += Streamer_GuestDisconnected;
        }
    }
    
    private int GetFreePlayer()
    {
        if (m_Players == null) return 0;
        for (int i = 1; i < m_Players.Length; i++)
            if (m_Players[i] == null)
                return i;
        return 0;
    }
    
    private void Streamer_GuestDisconnected(object sender, Parsec.ParsecGuest guest)
    {
        if (m_Players == null) return;
        for (int i = 0; i < m_Players.Length; i++)
        {
            if (m_Players[i] != null && m_Players[i].m_AssignedGuest.id == guest.id)
            {
                //m_Players[i].BreakDown(); // Destroy player gameobject
                Destroy(m_Players[i]);
                m_Players[i] = null;
                slotSelection[i].rewiredPlayer = null;
                break;
            }
        }
    }

    private void Streamer_GuestConnected(object sender, Parsec.ParsecGuest guest)
    {
        int iPlayer = GetFreePlayer();
        if (iPlayer == 0) return; // no place left or array is null
        
        turnManager = FindObjectOfType<TurnManager>();
        
        bool isMenu = turnManager == null;
        
        CustomController csController = ReInput.controllers.CreateCustomController(0, "Parsec_" + guest.id);
        CustomController csKeyboard = ReInput.controllers.CreateCustomController(1, "Parsec_" + guest.id);
        CustomController csMouse = ReInput.controllers.CreateCustomController(2, "Parsec_" + guest.id);
        ParsecUnity.ParsecRewiredInput.AssignCustomControllerToUser(guest, csController);
        //ParsecUnity.ParsecRewiredInput.AssignKeyboardControllerToUser(guest, csKeyboard);
        
        SetupPlayer(iPlayer, guest, csController, csKeyboard, csMouse);
    }
    
    public void SetupPlayer(int player, Parsec.ParsecGuest guest, CustomController controller, CustomController keyboard, CustomController mouse)
    {
        if (m_Players == null) return;
        if (player >= 0 && player < m_Players.Length)
        {
            m_Players[player] = gameObject.AddComponent<PlayerManager>();
            m_Players[player].m_PlayerNumber = player;
            m_Players[player].m_AssignedGuest = guest;
            
            m_Players[player].csController = controller;
            m_Players[player].csKeyboard = keyboard;
            m_Players[player].csMouse = mouse;
            
            m_Players[player].Setup();
            
            slotSelection[player].InitReInput(player);
        }
    }

    public void SpawnPlayer(int player, Parsec.ParsecGuest guest)
    {
        if (m_Players == null) return;
        if (player >= 0 && player < m_Players.Length)
        {
            m_Players[player] = gameObject.AddComponent<PlayerManager>();
            m_Players[player].m_PlayerNumber = player;
            m_Players[player].m_AssignedGuest = guest;
            m_Players[player].Setup();
            
            slotSelection[player].InitReInput(player);
            if (player == 0)
            {
                slotSelection[0].rewiredPlayer.controllers.AddController(ReInput.controllers.Joysticks[0], true);
            }
        }
    }

    public void GetAccessCode()
    {
        //Replace the Game ID with your own.
        ParsecUnity.API.SessionData sessionData = streamer.RequestCodeAndPoll();
        if (sessionData != null && sessionData.data != null)
        {
            /*VerificationUri.text = sessionData.data.verification_uri;
            UserCode.text = sessionData.data.user_code;
            StatusField.text = "Waiting for User";*/
            Debug.Log(sessionData.data.verification_uri + "/" +sessionData.data.user_code);
        }
    }

    public void AuthenticationPoll(ParsecUnity.API.SessionResultDataData data, ParsecUnity.API.SessionResultEnum status)
    {
        switch (status)
        {
            case ParsecUnity.API.SessionResultEnum.PolledTooSoon:
                break;
            case ParsecUnity.API.SessionResultEnum.Pending:
                //StatusField.text = "Waiting for User";
                Debug.Log("Waiting for User");
                break;
            case ParsecUnity.API.SessionResultEnum.CodeApproved:
                //StatusField.text = "Code Approved";
                //PanelAuthentication.gameObject.SetActive(false);
                authGo.SetActive(false);
                authData = data;
                //PanelParsecControl.gameObject.SetActive(true);
                controlGo.SetActive(true);
                Debug.Log("Code Approved");
                break;
            case ParsecUnity.API.SessionResultEnum.CodeInvallidExpiredDenied:
                //StatusField.text = "Code Expired";
                Debug.Log("Code Expired");
                break;
            case ParsecUnity.API.SessionResultEnum.Unknown:
                //StatusField.text = "Unknown State";
                Debug.Log("Unknown State");
                break;
            default:
                break;
        }
    }

    public void StartParsec()
    {
        streamer.StartParsec(m_Players.Length, /*IsPublicGame.isOn*/ false, "Vroom Vroom", "The best car game", authData.id);
        Debug.Log(streamer.GetInviteUrl(authData));
        //ShortLinkUri.text = streamer.GetInviteUrl(authData);
    }

    public void StopParsec()
    {
        streamer.StopParsec();
    }

    private void Initialize()
    {
        initialized = true;
        ParsecUnity.ParsecRewiredInput.SetReWiredInstance(ReInput.controllers);
        SpawnPlayer(0, new Parsec.ParsecGuest());
    }
    
    private void Update()
    {
        if (!ReInput.isReady) return; // Exit if Rewired isn't ready. This would only happen during a script recompile in the editor.
        if (!initialized) Initialize(); // Reinitialize after a recompile in the editor
    }

    public void LaunchGameScene(UInt16[] playerCars)
    {
        playerCarIndex = playerCars;
        StartCoroutine(LoadingScreen());
    }
    
    IEnumerator LoadingScreen()
    {
        AsyncOperation loadingAsync = SceneManager.LoadSceneAsync(1);
        loadingAsync.allowSceneActivation = false;

        while (!loadingAsync.isDone)
        {
            if (loadingAsync.progress >= 0.9f)
            {
                loadingAsync.allowSceneActivation = true;
            }

            yield return null;
        }

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        turnManager = TurnManager.instance;
        cube.transform.parent = turnManager.transform;
        
        //SetupGame();
    }

    private void SetupGame()
    {
        for (int i = 0; i < playerCarIndex.Length; i++)
        {
            // Spawn car
            //

            //m_Players[i].SetupInGame(/* Car previously created */);

            if (i != 0)
            {
                m_Players[i].carController.stopCar();
                m_Players[i].carController.gameObject.SetActive(false);
                m_Players[i].carController.vcam.gameObject.SetActive(false);
            }
        }
    }
}