using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class MenuControllerManager : MonoBehaviour
{
    public static MenuControllerManager instance;

    public int nbGamepad = 0;
    
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

        nbGamepad = 0;
        
        ReInput.ControllerConnectedEvent += OnControllerConnected;
        ReInput.ControllerDisconnectedEvent += OnControllerDisconnected;

        // Check all joysticks that are already connected
        for(int i = 0; i < ReInput.controllers.joystickCount; i++)
        {
            if(ReInput.controllers.Joysticks[i].ImplementsTemplate<IGamepadTemplate>())
            {
                nbGamepad++;
                // Gamepad connected
                // Call menu function to add a player in the screen + controllers -> Karim
            }
        }
    }
    
    void OnControllerConnected(ControllerStatusChangedEventArgs args)
    {
        if(ReInput.controllers.GetController(args.controllerType, args.controllerId).ImplementsTemplate<IGamepadTemplate>())
        {
            // New gamepad connected
            // Call menu function to add a player in the screen + controllers -> Karim
            nbGamepad++;
        }
    }
    
    void OnControllerDisconnected(ControllerStatusChangedEventArgs args)
    {
        if(args.controller == null)
        {
            // Gamepad disconnected
            // Call menu function to delete a player in the screen + controllers -> Karim
            nbGamepad--;
        }
    }
    
    void OnDestroy()
    {
        ReInput.ControllerConnectedEvent -= OnControllerConnected;
        ReInput.ControllerDisconnectedEvent -= OnControllerDisconnected;
    }
}
