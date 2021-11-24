using System;
using UnityEngine;
using ParsecGaming;
using Rewired;

public class ParsecGameManager : MonoBehaviour
{
    public PlayerManager[] m_Players;
    
    private ParsecStreamGeneral streamer;
    private ParsecUnity.API.SessionResultDataData authData;
    [System.NonSerialized] private bool initialized = false;

    private void Awake()
    {
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
                break;
            }
        }
    }

    private void Streamer_GuestConnected(object sender, Parsec.ParsecGuest guest)
    {
        int iPlayer = GetFreePlayer();
        if (iPlayer == 0) return; // no place left or array is null
        
        CustomController csController = ReInput.controllers.CreateCustomController(0, "Parsec_" + guest.id);
        CustomController csKeyboard = ReInput.controllers.CreateCustomController(1, "Parsec_" + guest.id);
        CustomController csMouse = ReInput.controllers.CreateCustomController(2, "Parsec_" + guest.id);
        ParsecUnity.ParsecRewiredInput.AssignCustomControllerToUser(guest, csController);
        //ParsecUnity.ParsecRewiredInput.AssignKeyboardControllerToUser(guest, csKeyboard);
        //SpawnPlayer(iPlayer, guest, csController, csKeyboard, csMouse);
    }
    
    public void SpawnPlayer(int player, Parsec.ParsecGuest guest, CustomController controller, CustomController keyboard, CustomController mouse)
    {
        if (m_Players == null) return;
        if (player >= 0 && player < m_Players.Length)
        {
            m_Players[player] = gameObject.AddComponent<PlayerManager>();
            //m_Players[player].m_Instance = Instantiate(m_PlayerObjectPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)) as GameObject;
            m_Players[player].m_PlayerNumber = player;
            m_Players[player].m_AssignedGuest = guest;
            //m_Players[player].Setup(controller, keyboard, mouse);
        }
    }

    public void SpawnPlayer(int player, Parsec.ParsecGuest guest)
    {
        if (m_Players == null) return;
        if (player >= 0 && player < m_Players.Length)
        {
            m_Players[player] = gameObject.AddComponent<PlayerManager>();
            //m_Players[player].m_Instance = Instantiate(m_PlayerObjectPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)) as GameObject;
            m_Players[player].m_PlayerNumber = player;
            m_Players[player].m_AssignedGuest = guest;
            //m_Players[player].Setup(null, null, null);
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
        }
    }

    public void AuthenticationPoll(ParsecUnity.API.SessionResultDataData data, ParsecUnity.API.SessionResultEnum status)
    {
        /*switch (status)
        {
            case ParsecUnity.API.SessionResultEnum.PolledTooSoon:
                break;
            case ParsecUnity.API.SessionResultEnum.Pending:
                StatusField.text = "Waiting for User";
                break;
            case ParsecUnity.API.SessionResultEnum.CodeApproved:
                StatusField.text = "Code Approved";
                PanelAuthentication.gameObject.SetActive(false);
                authdata = data;
                PanelParsecControl.gameObject.SetActive(true);
                break;
            case ParsecUnity.API.SessionResultEnum.CodeInvallidExpiredDenied:
                StatusField.text = "Code Expired";
                break;
            case ParsecUnity.API.SessionResultEnum.Unknown:
                StatusField.text = "Unknown State";
                break;
            default:
                break;
        }*/
    }

    public void StartParsec()
    {
        streamer.StartParsec(m_Players.Length, /*IsPublicGame.isOn*/ false, "Unity Test", "An Example Unity Project", authData.id);
        /*ShortLinkUri.text = */streamer.GetInviteUrl(authData);
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
}