using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using ReorderableList = UnityEditorInternal.ReorderableList;

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

    private Action<SceneView> sceneViewAction;
    private TurnManager _turnManager;
    private CheckpointsController _checkpointsController;
    
    private List<PlayerLog> playerLogs = new List<PlayerLog>();

    private bool showPlayerList, showCheckPoints;

    private int advancedToolbar = 0;

    private GUIStyle checkpointStyle;

    [MenuItem("Window/TurboEditor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(TurboEditor));
    }

    private void OnEnable()
    {
        FindManagers();
        
        playerLogs.Clear();
        for (int i = 0; i < _turnManager.playerList.Count; i++)
        {
            playerLogs.Add(new PlayerLog(_turnManager.playerList[i].prefabIndex));
        }

        checkpointStyle = new GUIStyle();
        checkpointStyle.normal.textColor = Color.white;
        Texture2D newTex = new Texture2D(64,64);
        
        for (int y = 0; y < newTex.height; y++)
        {
            for (int x = 0; x < newTex.width; x++)
            {
                newTex.SetPixel(x, y, new Color(45f / 255f, 45f / 255f, 45f / 255f));
            }
        }
        
        newTex.Apply();
        checkpointStyle.normal.background = newTex;

        sceneViewAction = new Action<SceneView>(this.OnSceneGUI);
        SceneView.duringSceneGui += sceneViewAction;
    }

    private void OnDestroy()
    {
        SceneView.duringSceneGui -= sceneViewAction;
    }

    private void OnFocus()
    {
        FindManagers();

        playerLogs.Clear();
        for (int i = 0; i < _turnManager.playerList.Count; i++)
        {
            playerLogs.Add(new PlayerLog(_turnManager.playerList[i].prefabIndex));
        }
    }

    private void OnGUI()
    {
        if (!Application.isPlaying)
        {
            FindManagers();

            // _turnManager = (TurnManager) EditorGUILayout.ObjectField("Turn Manager", _turnManager, typeof(TurnManager), true);
            
            GUILayout.Space(20);
            
            #region Map Settings
            
            GUILayout.Label("Map Settings", EditorStyles.whiteLargeLabel);
            GUILayout.Space(10);
            _turnManager.endCamera = (GameObject) EditorGUILayout.ObjectField("End Camera", _turnManager.endCamera, typeof(GameObject), true);
            // _turnManager.spawnPoint = (Transform) EditorGUILayout.ObjectField("Spawn Transform", _turnManager.spawnPoint, typeof(Transform), true);
            _turnManager.maxTurn = EditorGUILayout.IntField("Turns Per Player", _turnManager.maxTurn);

            GUILayout.Space(10);
            showCheckPoints = EditorGUILayout.Foldout(showCheckPoints, "Checkpoints");
            if (showCheckPoints)
            {
                for (int i = 0; i < _checkpointsController.points.Count; i++)
                {
                    String name;
                    if (_checkpointsController.points[i].GO != null)
                    {
                        name = _checkpointsController.points[i].GO.name;
                    }
                    else
                    {
                        name = "null";
                    }

                    
                    EditorGUILayout.BeginVertical(checkpointStyle);
                    EditorGUILayout.BeginHorizontal();
                            GUILayout.Label(name);
                            if (GUILayout.Button("", "Radio"))
                            {
                                FocusCP(i);
                            }

                            _checkpointsController.points[i].km = EditorGUILayout.FloatField("", _checkpointsController.points[i].km, EditorStyles.numberField);
                            
                            if (GUILayout.Button("▲", "MiniButtonLeft"))
                            {
                                if(i > 0) SwapList(_checkpointsController.points, i, i-1);
                            }
                            if (GUILayout.Button("▼", "MiniButtonRight"))
                            {
                                if(i < _checkpointsController.points.Count - 1) SwapList(_checkpointsController.points, i, i+1);
                            }
                            if (GUILayout.Button("✖", "MiniButtonRight"))
                            {
                                RemoveCheckpoint(i);
                            }
                        EditorGUILayout.EndHorizontal();

                        Vector3 previous = _checkpointsController.points[i - 1 >= 0 ? i - 1 : (_checkpointsController.points.Count - 1)].GO.transform.position;
                        Vector3 current = _checkpointsController.points[i].GO.transform.position;
                        
                        GUILayout.Label("Previous Direct Distance " + Vector3.Distance(current, previous), EditorStyles.miniLabel);
                        
                    EditorGUILayout.EndVertical();
                    GUILayout.Space(3);
                }
                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("Add", "MiniButtonLeft"))
                {
                    AddCheckpoint();
                }
                EditorGUILayout.EndHorizontal();

                if (GUILayout.Button("Beautify"))
                {
                    OrderCP();
                }
            }

            #endregion

            GUILayout.Space(20);
            
            #region Players Settings

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
                    if (_turnManager.playerList[i].carController == null)
                    {
                        RemovePlayer(i);
                        i--;
                    }
                    else
                    {
                        EditorGUILayout.BeginHorizontal();

                        if (GUILayout.Button("", "Radio"))
                        {
                            Selection.activeGameObject = _turnManager.playerList[i].carController.gameObject;
                        }
                    
                        if(playerLogs[i].ChangeIndex(EditorGUILayout.Popup("Player " + i, _turnManager.playerList[i].prefabIndex, prefabsNames.ToArray())))
                        {
                            ChangeCarType(i, playerLogs[i].selectedIndex);
                        }
                     
                        _turnManager.playerList[i].carController.gameObject.SetActive(GUILayout.Toggle(_turnManager.playerList[i].carController.gameObject.activeInHierarchy, ""));
                     
                        EditorGUILayout.EndHorizontal();
                    }
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
            
            #endregion

            GUILayout.Space(20);
            
            #region Advanced Edit
            
            GUILayout.Label("Edit Mode", EditorStyles.whiteLargeLabel);
            
            advancedToolbar = GUILayout.Toolbar(advancedToolbar, new string[] {"Simple", "Advanced"});
            
            // If Advanced mode
            if (advancedToolbar == 1)
            {
                GUILayout.Space(10);
                GUILayout.Label("Car prefabs");
                
                
                for (int i = 0; i < _turnManager.carPrefabs.Count; i++)
                {
                    _turnManager.carPrefabs[i] = (GameObject) EditorGUILayout.ObjectField("Prefab " + i, _turnManager.carPrefabs[i], typeof(GameObject), true);
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
                _checkpointsController.CPPrefab = (GameObject) EditorGUILayout.ObjectField("Checkpoint Prefab", _checkpointsController.CPPrefab, typeof(GameObject), true);
                
                // if (GUILayout.Button("Reset"))
                // {
                //     for (int i = 0; i < _turnManager.playerList.Count; i++)
                //     {
                //         RemovePlayer(i);
                //         i--;
                //     }
                // }
                
            }
            #endregion
        }
    }

    #region Map Methods

    private void AddCheckpoint()
    {
        int newIndex = _checkpointsController.points.Count;
        int prevIndex = 0;
        if (newIndex > 0) prevIndex = newIndex - 1;
        
        GameObject go = (GameObject) PrefabUtility.InstantiatePrefab(_checkpointsController.CPPrefab, _checkpointsController.transform);
        go.transform.position = _checkpointsController.points[prevIndex].GO.transform.position;
        go.name = "Checkpoint " + newIndex;
        
        go.GetComponent<CheckPoint>().setIndex(newIndex);
        
        _checkpointsController.points.Add(new CheckpointsController.Checkpoint(go, newIndex));
        
        FocusCP(newIndex);
        OrderCP();
    }
    
    
    private void RemoveCheckpoint(int index)
    {
        if (_checkpointsController.points.Count > index)
        {
            if (_checkpointsController.points[index].GO != null)
            {
                DestroyImmediate(_checkpointsController.points[index].GO);
            }

            _checkpointsController.points.RemoveAt(index);
            OrderCP();
        }
    }

    private void OrderCP()
    {
        for (int i = 0; i < _checkpointsController.points.Count; i++)
        {
            if (_checkpointsController.points[i].GO != null)
            {
                _checkpointsController.points[i].listIndex = i;
                _checkpointsController.points[i].GO.name = "Checkpoint " + i;
                _checkpointsController.points[i].GO.GetComponent<CheckPoint>().setIndex(i);
            }
            else
            {
                RemoveCheckpoint(i);
                i--;
            }
        }
    }

    private void FocusCP(int index)
    {
        Selection.activeGameObject = _checkpointsController.points[index].GO;
        SceneView.lastActiveSceneView.FrameSelected();
    }
    
    #endregion

    #region Players Methods
    
        private void AddPlayer()
        {
            GameObject go = (GameObject) PrefabUtility.InstantiatePrefab(_turnManager.carPrefabs[0], _checkpointsController.points[0].GO.transform);
            go.transform.SetParent(null);
            go.name = "Player " + _turnManager.playerList.Count;
            TurnManager.Player newPlayer = new TurnManager.Player();
            
            newPlayer.carController = go.GetComponent<CarController>();
            newPlayer.prefabIndex = 0;
            
            _turnManager.playerList.Add(newPlayer);
            
            playerLogs.Add(new PlayerLog(0));
        }

        private void RemovePlayer(int index)
        {
            if (_turnManager.playerList.Count > index)
            {
                if (_turnManager.playerList[index].carController != null)
                {
                    DestroyImmediate(_turnManager.playerList[index].carController.gameObject);
                }
                playerLogs.RemoveAt(index);
                _turnManager.playerList.RemoveAt(index);
            }
        }

        private void ChangeCarType(int carIndex, int typeIndex)
        {
            Transform oldTransform = _turnManager.playerList[carIndex].carController.transform;

            GameObject newCar = (GameObject) PrefabUtility.InstantiatePrefab(_turnManager.carPrefabs[typeIndex], oldTransform);
            newCar.transform.SetParent(null);
            newCar.name = _turnManager.playerList[carIndex].carController.gameObject.name;
            DestroyImmediate(_turnManager.playerList[carIndex].carController.gameObject);
            _turnManager.playerList[carIndex].carController = newCar.GetComponent<CarController>();
            _turnManager.playerList[carIndex].prefabIndex = typeIndex;
        }
    
    #endregion

    private void OnSceneGUI(SceneView obj)
    {
        Handles.color = Color.blue;
            
        Handles.BeginGUI();
        for (int i = 0; i < _checkpointsController.points.Count; i++)
        {
            if (_checkpointsController.points[i].GO != null)
            {
                Handles.Label(_checkpointsController.points[i].GO.transform.position, _checkpointsController.points[i].GO.name);
                Handles.SphereHandleCap(0, _checkpointsController.points[i].GO.transform.position, _checkpointsController.points[i].GO.transform.rotation, .5f, EventType.Repaint);
            }

            if ( i + 1 < _checkpointsController.points.Count)
            {
                Vector3 current = _checkpointsController.points[i].GO.transform.position;
                Vector3 next = _checkpointsController.points[i + 1].GO.transform.position;
                Handles.color = Color.blue;
                Handles.DrawLine(current, next);
                
                Handles.color = Color.green;
                Vector3 middle = new Vector3((current.x + next.x) / 2f, (current.y + next.y) / 2f,
                    (current.z + next.z) / 2f);
                Handles.SphereHandleCap(0, middle, _checkpointsController.points[i].GO.transform.rotation, .5f, EventType.Repaint);

            }
        }
        Handles.EndGUI();
    }
    
    private void FindManagers()
    {
        if(_turnManager == null) _turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        if(_checkpointsController == null) _checkpointsController = GameObject.Find("CheckpointsController").GetComponent<CheckpointsController>();
    }
    
    private void SwapList<T>(List<T> list, int indexA, int indexB)
    {
        T tmp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = tmp;
    }
}
