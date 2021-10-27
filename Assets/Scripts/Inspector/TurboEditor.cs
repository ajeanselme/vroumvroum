using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TurboEditor : EditorWindow
{
    private class PlayerLog
    {
        public int oldSelectedIndex = 0;
        public int selectedIndex = 0;

        public PlayerLog(int index)
        {
            selectedIndex = index;
            oldSelectedIndex = index;
        }

        public bool ChangeIndex(int newIndex)
        {
            oldSelectedIndex = selectedIndex;
            selectedIndex = newIndex;
            return oldSelectedIndex != selectedIndex;
        }
    }

    private TurnManager _turnManager;
    private List<PlayerLog> playerLogs = new List<PlayerLog>();

    private bool showPlayerList = false;

    private int advancedToolbar = 0;

    private static Texture2D tex;
    
    [MenuItem("Window/TurboEditor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(TurboEditor));
        
    }

    private void OnEnable()
    {
        playerLogs.Clear();
        for (int i = 0; i < _turnManager.playerList.Count; i++)
        {
            playerLogs.Add(new PlayerLog(_turnManager.playerList[i].prefabIndex));
        }
        
        tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        tex.SetPixel(0, 0, new Color(0.2f, 0.4f, 0.25f));
        tex.Apply();
    }

    private void OnFocus()
    {
        _turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();

        playerLogs.Clear();
        for (int i = 0; i < _turnManager.playerList.Count; i++)
        {
            playerLogs.Add(new PlayerLog(_turnManager.playerList[i].prefabIndex));
        }
    }

    private void OnInspectorUpdate()
    {
        _turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
    }

    private void OnGUI()
    {
        if (!Application.isPlaying)
        {
            _turnManager = (TurnManager) EditorGUILayout.ObjectField("Turn Manager", _turnManager, typeof(TurnManager), true);
            
            GUILayout.Space(20);
            GUILayout.Label("Map Settings", EditorStyles.whiteLargeLabel);
            GUILayout.Space(10);
            _turnManager.endCamera = (GameObject) EditorGUILayout.ObjectField("End Camera", _turnManager.endCamera, typeof(GameObject), true);
            _turnManager.spawnPoint = (Transform) EditorGUILayout.ObjectField("Spawn Transform", _turnManager.spawnPoint, typeof(Transform), true);
            _turnManager.maxTurn = EditorGUILayout.IntField("Turns Per Player", _turnManager.maxTurn);
            
            GUILayout.Space(20);
            GUILayout.Label("Players Settings", EditorStyles.whiteLargeLabel);
            GUILayout.Space(10);

            showPlayerList = EditorGUILayout.Foldout(showPlayerList, "Player List", EditorStyles.foldoutHeader);
            if (showPlayerList)
            {
                GUILayout.Space(10);
                List<string> prefabsNames = new List<string>();
                for (int i = 0; i < _turnManager.carPrefabs.Count; i++)
                {
                    prefabsNames.Add(_turnManager.carPrefabs[i].name);
                }

                for (int i = 0; i < _turnManager.playerList.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button("", "Radio"))
                    {
                        Selection.activeGameObject = _turnManager.playerList[i].carController.gameObject;
                    }
                    
                     if(playerLogs[i].ChangeIndex(EditorGUILayout.Popup("Player " + (i + 1), _turnManager.playerList[i].prefabIndex, prefabsNames.ToArray())))
                     {
                         ChangeType(i, playerLogs[i].selectedIndex);
                     }
                     
                     _turnManager.playerList[i].carController.gameObject.SetActive(GUILayout.Toggle(_turnManager.playerList[i].carController.gameObject.activeInHierarchy, ""));
                     
                     EditorGUILayout.EndHorizontal();

                }
                
                GUILayout.Space(10);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Add", "MiniButtonLeft"))
                {
                    AddPlayer();
                }
                if (GUILayout.Button("Remove", "MiniButtonRight"))
                {
                    if (_turnManager.playerList.Count > 0)
                    {
                        RemovePlayer(_turnManager.playerList.Count - 1);
                    }   
                }
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(20);
            GUILayout.Label("Edit Mode", EditorStyles.whiteLargeLabel);
            
            advancedToolbar = GUILayout.Toolbar(advancedToolbar, new string[] {"Simple", "Advanced"});
            
            // If Advanced mode
            if (advancedToolbar == 1)
            {
                GUILayout.Space(10);
                GUILayout.Label("Car prefabs");
                
                
                for (int i = 0; i < _turnManager.carPrefabs.Count; i++)
                {
                    _turnManager.carPrefabs[i] = (GameObject) EditorGUILayout.ObjectField("Prefab " + (i + 1), _turnManager.carPrefabs[i], typeof(GameObject), true);
                }

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("+", "toolbarTextField"))
                {
                    _turnManager.carPrefabs.Add(_turnManager.carPrefabs[_turnManager.carPrefabs.Count - 1]);
                }
                if (GUILayout.Button("-", "toolbarTextField"))
                {
                    _turnManager.carPrefabs.RemoveAt(_turnManager.carPrefabs.Count - 1);
                }
                GUILayout.EndHorizontal();
                
                GUILayout.Space(10);
                
                _turnManager.speedParticles = (ParticleSystem) EditorGUILayout.ObjectField("Speed Particles", _turnManager.speedParticles, typeof(ParticleSystem), true);

                
                // if (GUILayout.Button("Reset"))
                // {
                //     for (int i = 0; i < _turnManager.playerList.Count; i++)
                //     {
                //         RemovePlayer(i);
                //         i--;
                //     }
                // }
            }
        }

    }

    private void AddPlayer()
    {
        GameObject go = Instantiate(_turnManager.carPrefabs[0], _turnManager.spawnPoint.position, _turnManager.spawnPoint.rotation);
        go.name = "Player " + (_turnManager.playerList.Count + 1);
        TurnManager.Player newPlayer = new TurnManager.Player();
        
        newPlayer.carController = go.GetComponent<CarController>();
        newPlayer.prefabIndex = 0;
        
        _turnManager.playerList.Add(newPlayer);
        
        playerLogs.Add(new PlayerLog(0));
    }

    private void RemovePlayer(int index)
    {
        DestroyImmediate(_turnManager.playerList[index].carController.gameObject);
        playerLogs.RemoveAt(index);
        _turnManager.playerList.RemoveAt(index);
    }

    private void ChangeType(int carIndex, int typeIndex)
    {
        Transform oldTransform = _turnManager.playerList[carIndex].carController.transform;

        GameObject newCar = Instantiate(_turnManager.carPrefabs[typeIndex], oldTransform.position, oldTransform.rotation);
        newCar.name = _turnManager.playerList[carIndex].carController.gameObject.name;
        DestroyImmediate(_turnManager.playerList[carIndex].carController.gameObject);
        _turnManager.playerList[carIndex].carController = newCar.GetComponent<CarController>();
        _turnManager.playerList[carIndex].prefabIndex = typeIndex;
    }
}
