using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
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
    private EventsManager _eventsManager;
    
    private List<PlayerLog> playerLogs = new List<PlayerLog>();

    private bool showPlayerList, showCheckPoints, showEvents;

    private Vector2 scrollPos;

    private int advancedToolbar = 0;

    private GUIStyle checkpointStyle;

    [MenuItem("Window/TurboEditor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(TurboEditor)).Show();
    }

    private void OnEnable()
    {
        FindManagers();

        if (_turnManager != null)
        {
            playerLogs.Clear();
            for (int i = 0; i < _turnManager.cars.Length; i++)
            {
                playerLogs.Add(new PlayerLog(/*_turnManager.cars[i].prefabIndex*/ 0));
            }
        }

        checkpointStyle = new GUIStyle();
        checkpointStyle.normal.textColor = Color.white;
        Texture2D newTex = new Texture2D(64, 64);

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

        if (_turnManager != null)
        {
            playerLogs.Clear();
            for (int i = 0; i < _turnManager.cars.Length; i++)
            {
                playerLogs.Add(new PlayerLog(/*_turnManager.cars[i].prefabIndex*/ 0));
            }
        }
    }

    private void OnGUI()
    {
        if (!Application.isPlaying)
        {
            FindManagers();

            if (_turnManager != null)
            {
                // _turnManager = (TurnManager) EditorGUILayout.ObjectField("Turn Manager", _turnManager, typeof(TurnManager), true);

                GUILayout.Space(20);

                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

                #region Map Settings

                GUILayout.Label("Map Settings", EditorStyles.whiteLargeLabel);
                GUILayout.Space(10);
                EditorGUI.indentLevel++;
                // _turnManager.spawnPoint = (Transform) EditorGUILayout.ObjectField("Spawn Transform", _turnManager.spawnPoint, typeof(Transform), true);
                _turnManager.maxTurn = EditorGUILayout.IntField("Turns Per Player", _turnManager.maxTurn);
                _turnManager.playMinigame = EditorGUILayout.Toggle("Activate Minigame", _turnManager.playMinigame);

                GUILayout.Space(10);
                showCheckPoints = EditorGUILayout.Foldout(showCheckPoints, "Checkpoints");
                if (showCheckPoints)
                {
                    showCheckpoints();
                }
                
                GUILayout.Space(10);
                showEvents = EditorGUILayout.Foldout(showEvents, "Events");
                if (showEvents)
                {
                    ShowChessEvent();
                }

                #endregion

                GUILayout.Space(20);

                if (_turnManager != null)
                {
                    #region Players Settings

                    GUILayout.Label("Players Settings", EditorStyles.whiteLargeLabel);
                    GUILayout.Space(10);


                    showPlayerList = EditorGUILayout.Foldout(showPlayerList, "Player List", EditorStyles.foldoutHeader);
                    if (showPlayerList)
                    {
                        GUILayout.Space(10);
                        List<string> prefabsNames = new List<string>();
                        for (int i = 0; i < _turnManager.cars.Length; i++)
                        {
                            prefabsNames.Add(_turnManager.cars[i].name);
                        }

                        for (int i = 0; i < _turnManager.cars.Length; i++)
                        {
                            if (_turnManager.cars[i] == null)
                            {
                                RemovePlayer(i);
                                Debug.Log("remove 1");
                                i--;
                            }
                            else
                            {
                                EditorGUILayout.BeginHorizontal();

                                if (GUILayout.Button("", "Radio"))
                                {
                                    Selection.activeGameObject = _turnManager.cars[i].gameObject;
                                }

                                if (playerLogs[i].ChangeIndex(EditorGUILayout.Popup("Player " + i,
                                    /*_turnManager.cars[i].prefabIndex*/ 0, prefabsNames.ToArray())))
                                {
                                    ChangeCarType(i, playerLogs[i].selectedIndex);
                                }

                                _turnManager.cars[i].gameObject.SetActive(
                                    GUILayout.Toggle(
                                        _turnManager.cars[i].gameObject.activeInHierarchy, ""));

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
                            if (_turnManager.cars.Length > 0)
                            {
                                RemovePlayer(_turnManager.cars.Length - 1);
                                Debug.Log("remove 2");
                            }
                        }

                        EditorGUILayout.EndHorizontal();
                    }

                    #endregion

                    GUILayout.Space(20);
                }

                #region Advanced Edit

                GUILayout.Label("Edit Mode", EditorStyles.whiteLargeLabel);

                advancedToolbar = GUILayout.Toolbar(advancedToolbar, new string[] {"Simple", "Advanced"});

                // If Advanced mode
                if (advancedToolbar == 1)
                {
                    GUILayout.Space(10);

                    _turnManager.endCamera = (GameObject) EditorGUILayout.ObjectField("End Camera", _turnManager.endCamera,
                        typeof(GameObject), true);
                    GUILayout.Space(10);
                    GUILayout.Label("Car prefabs");


                    for (int i = 0; i < _turnManager.carPrefabs.Count; i++)
                    {
                        _turnManager.carPrefabs[i] = (GameObject) EditorGUILayout.ObjectField("Prefab " + i,
                            _turnManager.carPrefabs[i], typeof(GameObject), true);
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

                    _turnManager.speedParticles = (ParticleSystem) EditorGUILayout.ObjectField("Speed Particles",
                        _turnManager.speedParticles, typeof(ParticleSystem), true);

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

                EditorGUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Space(maxSize.y / 10f);
                if (GUILayout.Button("Setup Scene"))
                {
                    GameObject components = Instantiate(Resources.Load("SceneComponents") as GameObject);
                    components.name = "SceneComponents";
                }
            }
        }
    }

    #region EditorMethods

    private void showCheckpoints()
    {
        for (int i = 0; i < _checkpointsController.points.Count; i++)
        {
            EditorGUILayout.BeginVertical(checkpointStyle);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("" + i);
            if (GUILayout.Button("", "Radio"))
            {
                FocusCP(i);
            }

            _checkpointsController.points[i].distance = EditorGUILayout.FloatField("",
                _checkpointsController.points[i].distance, EditorStyles.numberField);

            if (GUILayout.Button("▲", "MiniButtonLeft"))
            {
                if (i > 0) SwapList(_checkpointsController.points, i, i - 1);
            }

            if (GUILayout.Button("▼", "MiniButtonRight"))
            {
                if (i < _checkpointsController.points.Count - 1) SwapList(_checkpointsController.points, i, i + 1);
            }

            if (GUILayout.Button("✖", "MiniButtonRight"))
            {
                RemoveCheckpoint(i);
                return;
            }

            EditorGUILayout.EndHorizontal();

            Vector3 previous = _checkpointsController
                .points[i - 1 >= 0 ? i - 1 : (_checkpointsController.points.Count - 1)].position;
            Vector3 current = _checkpointsController.points[i].position;

            GUILayout.Label("Previous Direct Distance " + Vector3.Distance(current, previous), EditorStyles.miniLabel);

            _checkpointsController.points[i].foldout =
                EditorGUILayout.Foldout(_checkpointsController.points[i].foldout, "Transform");
            if (_checkpointsController.points[i].foldout)
            {
                _checkpointsController.points[i].position =
                    EditorGUILayout.Vector3Field("", _checkpointsController.points[i].position);
                _checkpointsController.points[i].rotation =
                    EditorGUILayout.Vector3Field("", _checkpointsController.points[i].rotation);
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(3);
        }

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Add", "MiniButtonLeft"))
        {
            AddCheckpoint();
        }

        EditorGUILayout.EndHorizontal();
    }

    private void ShowChessEvent()
    {
        GUILayout.Space(10);
        GUILayout.Label("Chess Event Steps");
        
        for (int i = 0; i < _eventsManager.stepList.Count; i++)
        {
            EditorGUILayout.BeginVertical(checkpointStyle);
            EditorGUILayout.BeginHorizontal();
                    GUILayout.Label(""+i);

                    _eventsManager.stepList[i].gameObject = (GameObject) EditorGUILayout.ObjectField("Game Object", _eventsManager.stepList[i].gameObject, typeof(GameObject), true);
                    
                    
                    if (GUILayout.Button("▲", "MiniButtonLeft"))
                    {
                        if(i > 0) SwapList(_eventsManager.stepList, i, i-1);
                    }
                    if (GUILayout.Button("▼", "MiniButtonRight"))
                    {
                        if(i < _eventsManager.stepList.Count - 1) SwapList(_eventsManager.stepList, i, i+1);
                    }
                    if (GUILayout.Button("✖", "MiniButtonRight"))
                    {
                        RemoveEventStep(i);
                        return;
                    }
                EditorGUILayout.EndHorizontal();
                
                _eventsManager.stepList[i].targetPosition = EditorGUILayout.Vector3Field("", _eventsManager.stepList[i].targetPosition);
                EditorGUILayout.EndVertical();
            GUILayout.Space(3);
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add", "MiniButtonLeft"))
        {
            AddEventStep();
        }
        EditorGUILayout.EndHorizontal();

    }

    #endregion

    #region Map Methods

    private void AddCheckpoint()
    {
        int newIndex = _checkpointsController.points.Count;
        if (newIndex > 0)
        {
            _checkpointsController.points.Add(
                new CheckpointsController.Checkpoint(_checkpointsController.points[newIndex - 1].position));
        }
        else
        {
            _checkpointsController.points.Add(new CheckpointsController.Checkpoint(new Vector3(0, 0, 0)));
        }

        FocusCP(newIndex);
    }

    private void RemoveCheckpoint(int index)
    {
        if (_checkpointsController.points.Count > index)
        {
            _checkpointsController.points.RemoveAt(index);
        }
    }

    private void AddEventStep()
    {
        _eventsManager.stepList.Add(new EventsManager.Step());
    }
    private void RemoveEventStep(int index)
    {
        if (_eventsManager.stepList.Count > index)
        {
            _eventsManager.stepList.RemoveAt(index);
        }
    }

    private void FocusCP(int index)
    {
        GameObject temp = new GameObject();
        temp.transform.position = _checkpointsController.points[index].position;
        Selection.activeTransform = temp.transform;
        SceneView.lastActiveSceneView.FrameSelected();
        DestroyImmediate(temp);
    }

    #endregion

    #region Players Methods

    private void AddPlayer()
    {
        if (_checkpointsController.points.Count > 0)
        {
            /*GameObject go = (GameObject) PrefabUtility.InstantiatePrefab(_turnManager.carPrefabs[0]);
            go.transform.position = _checkpointsController.points[0].position;
            go.transform.eulerAngles = _checkpointsController.points[0].rotation;
            go.name = "Player " + _turnManager.playerList.Count;
            TurnManager.Player newPlayer = new TurnManager.Player();
        
            newPlayer.carController = go.GetComponent<CarController>();
            newPlayer.prefabIndex = 0;
        
            _turnManager.playerList.Add(newPlayer);
        
            playerLogs.Add(new PlayerLog(0));*/
        }
        else
        {
            Debug.LogError("Tu dois créer le premier checkpoint avant !");
        }
    }

    private void RemovePlayer(int index)
    {
        /*if (_turnManager.playerList.Count > index)
        {
            if (_turnManager.playerList[index].carController != null)
            {
                DestroyImmediate(_turnManager.playerList[index].carController.gameObject);
            }
            playerLogs.RemoveAt(index);
            _turnManager.playerList.RemoveAt(index);
        }*/
    }

    private void ChangeCarType(int carIndex, int typeIndex)
    {
        /*Transform oldTransform = _turnManager.playerList[carIndex].carController.transform;

        GameObject newCar = (GameObject) PrefabUtility.InstantiatePrefab(_turnManager.carPrefabs[typeIndex], oldTransform);
        newCar.transform.SetParent(null);
        newCar.name = _turnManager.playerList[carIndex].carController.gameObject.name;
        DestroyImmediate(_turnManager.playerList[carIndex].carController.gameObject);
        _turnManager.playerList[carIndex].carController = newCar.GetComponent<CarController>();
        _turnManager.playerList[carIndex].prefabIndex = typeIndex;*/
    }

    #endregion

    private void OnSceneGUI(SceneView obj)
    {
        if (showCheckPoints)
        {
            if (_checkpointsController != null)
            {
                Handles.color = Color.blue;

                Handles.BeginGUI();
                for (int i = 0; i < _checkpointsController.points.Count; i++)
                {
                    Handles.Label(_checkpointsController.points[i].position, "Checkpoint " + i);
                    Handles.SphereHandleCap(0, _checkpointsController.points[i].position,
                        quaternion.Euler(_checkpointsController.points[i].rotation), .5f, EventType.Repaint);

                    if (i + 1 < _checkpointsController.points.Count)
                    {
                        Vector3 current = _checkpointsController.points[i].position;
                        Vector3 next = _checkpointsController.points[i + 1].position;
                        Handles.color = Color.blue;
                        Handles.DrawLine(current, next);

                        Handles.color = Color.green;
                        Vector3 middle = new Vector3((current.x + next.x) / 2f, (current.y + next.y) / 2f,
                            (current.z + next.z) / 2f);
                        Handles.SphereHandleCap(0, middle, quaternion.Euler(_checkpointsController.points[i].rotation), .5f,
                            EventType.Repaint);

                    }
                }

                for (int i = 0; i < _checkpointsController.points.Count; i++)
                {
                    // Handles.Label(checkpoints[i].position, "Checkpoint " + i);
                    // Handles.SphereHandleCap(0, checkpoints[i].position, quaternion.Euler(0,0,0), .5f, EventType.Repaint);
                    EditorGUI.BeginChangeCheck();
                    _checkpointsController.points[i].position =
                        Handles.DoPositionHandle(_checkpointsController.points[i].position, Quaternion.identity);
                    // if (EditorGUI.EndChangeCheck())
                    // {
                    //     Undo.RecordObject(this, "Free Move LookAt Point");
                    //     checkpoints[i].position = pos;
                    //     this.Update();
                    // }
                    EditorGUI.EndChangeCheck();
                }

                Handles.EndGUI();
                Repaint();
            }
        }
    }

    private void FindManagers()
    {
        if (_turnManager == null)
        {
            _turnManager = GameObject.Find("TurnManager")?.GetComponent<TurnManager>();
        }

        if (_checkpointsController == null)
        {
            _checkpointsController = GameObject.Find("CheckpointsController")?.GetComponent<CheckpointsController>();
        }
        if (_eventsManager == null)
        {
            if (GameObject.Find("EventsManager"))
            {
                _eventsManager = GameObject.Find("EventsManager").GetComponent<EventsManager>();
            }
        }
    }

    private void SwapList<T>(List<T> list, int indexA, int indexB)
    {
        T tmp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = tmp;
    }
}
#endif

