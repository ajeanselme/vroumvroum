using ParsecGaming;
using ParsecUnity;
using Rewired;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [HideInInspector] public int m_PlayerNumber;
    [HideInInspector] public GameObject m_Instance;
    [HideInInspector] public Parsec.ParsecGuest m_AssignedGuest;
    private CarController carController;

    public void Setup(CustomController controller, CustomController keyboard, CustomController mouse)
    {
        ParsecInput.AssignGuestToPlayer(m_AssignedGuest, m_PlayerNumber);
        carController = m_Instance.GetComponent<CarController>();
        
        if (carController != null)
        {
            if (controller != null) ParsecRewiredInput.AssignCustomControllerToUser(m_AssignedGuest, controller);
            if (keyboard != null) ParsecRewiredInput.AssignKeyboardControllerToUser(m_AssignedGuest, keyboard);
            if (mouse != null) ParsecRewiredInput.AssignMouseControllerToUser(m_AssignedGuest, mouse);
            carController.InitReInput(m_PlayerNumber, controller, keyboard, mouse);
        }
        else
        {
            BreakDown();
        }
    }

    public void BreakDown()
    {
        Destroy(m_Instance);
    }
}
