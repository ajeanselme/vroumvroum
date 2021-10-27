using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TurboEditor : EditorWindow
{
    private class Player
    {
        public int index;
        public int selectedIndex = 0;
    }
    
    
    private string[] playerTypes = { "Orange", "Ambulance", "Police" };
    private List<Player> players = new List<Player>();
    

    [MenuItem("Window/TurboEditor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(TurboEditor));
    }
    
    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("Players", "WhiteLargeLabel");

        for (int i = 0; i < players.Count; i++)
        {
            EditorGUILayout.Popup("Player " + (i + 1), players[i].selectedIndex, playerTypes);
        }
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Add", "MiniButtonLeft"))
        {
            players.Add(new Player());
        }
        if (GUILayout.Button("Remove", "MiniButtonRight"))
        {
            if (players.Count > 0)
            {
                players.RemoveAt(players.Count - 1);
            }   
        }
        
        EditorGUILayout.EndHorizontal();
    }
}
