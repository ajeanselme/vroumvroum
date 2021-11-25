using ParsecGaming;
using ParsecUnity;
using Rewired;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [HideInInspector] public int m_PlayerNumber;
    [HideInInspector] public GameObject m_Instance;
    [HideInInspector] public Parsec.ParsecGuest m_AssignedGuest;

    [HideInInspector] public CustomController csController;
    [HideInInspector] public CustomController csKeyboard;
    [HideInInspector] public CustomController csMouse;
    
    [HideInInspector] public CarController carController;

    [HideInInspector] public int prefabIndex;

    public void Setup()
    {
        ParsecInput.AssignGuestToPlayer(m_AssignedGuest, m_PlayerNumber);

        if (carController != null)
        {
            if (csController != null) ParsecRewiredInput.AssignCustomControllerToUser(m_AssignedGuest, csController);
            if (csKeyboard != null) ParsecRewiredInput.AssignKeyboardControllerToUser(m_AssignedGuest, csKeyboard);
            if (csMouse != null) ParsecRewiredInput.AssignMouseControllerToUser(m_AssignedGuest, csMouse);
            // Assign a player with the main menu selection
        }
    }

    public void SetupInGame(CarController _carController)
    {
        carController = _carController;
        
        // ReSharper disable once Unity.NoNullPropagation
        carController?.InitReInput(m_PlayerNumber, csController, csKeyboard, csMouse);
    }

    public void BreakDown()
    {
        Destroy(m_Instance);
    }
}
