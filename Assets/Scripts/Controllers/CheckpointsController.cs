using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CheckpointsController : MonoBehaviour
{
    public static CheckpointsController instance;

    [Serializable]
    public class Checkpoint
    {
        public GameObject GO;
        public int listIndex;

        public float km = 0f;

        public Checkpoint(GameObject gameObject, int index)
        {
            GO = gameObject;
            listIndex = index;
        }
    }

    class PlayerData
    {
        public int index;
        public int currentCP;
        public float CPDistance, previousDistance, lastProgress;
        public float CurrentDistance => CPDistance + previousDistance;

        public PlayerData(int index)
        {
            this.index = index;
        }
    }

    [HideInInspector] public List<Checkpoint> points = new List<Checkpoint>();
    [HideInInspector] public GameObject CPPrefab;

    public GameObject UI;
    public Image progressBarFill;

    private PlayerData _playingPlayer;
    private int firstCar;
    // private float _CPDistance, _previousDistance, _lastProgress;
    public float CurrentCP => _playingPlayer.currentCP;
    public float CurrentDistance => _playingPlayer.CurrentDistance;

    private List<PlayerData> _playerDatas = new List<PlayerData>();

    public GameObject debug;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // private void Start()
    // {
    //     for (int i = 0; i < points.Count; i++)
    //     {
    //         totalDistance += points[i].km;
    //     }
    // }

    private void Update()
    {
        UpdateUI();

        if (_playingPlayer.currentCP > -1)
        {
            
            Checkpoint currentCP = points[_playingPlayer.currentCP];
            Checkpoint nextCP;
            
            if (_playingPlayer.currentCP + 1 < points.Count)
            {
                nextCP = points[_playingPlayer.currentCP + 1];
            }
            else
            {
                nextCP = points[0];
            }
            

            Vector3 pointA = currentCP.GO.transform.position;
            Vector3 pointB = nextCP.GO.transform.position;

            Vector3 player = TurnManager.instance.playerList[TurnManager.instance.indexCarTurn].carController.transform.position;

            Vector3 projected = Vector3.Project((player - pointA), (pointB - pointA)) + pointA;
            
            
            float CPDistance = Vector3.Distance(pointA, pointB);
            float projetedDistance = Vector3.Distance(pointA, projected);

            float progress = projetedDistance / CPDistance;

            // debug.transform.position = projected;
            
            _playingPlayer.CPDistance = nextCP.km * progress;

            if (_playingPlayer.CurrentDistance > _playerDatas[firstCar].CurrentDistance)
            {
                firstCar = _playingPlayer.index;
                UI.GetComponent<MMFeedbacks>().PlayFeedbacks();
            }
            
            if (progress > 1f)
            {
                NextCP();
            }

            // if (_lastProgress > progress)
            // {
            //     Debug.Log("wrong way");
            // }
            
            _playingPlayer.lastProgress = progress;
        }
        
        //Debug
        if (Input.GetKeyDown(KeyCode.E))
        {
            UI.GetComponent<MMFeedbacks>().PlayFeedbacks();
        }
    }

    private void UpdateUI()
    {
        List<float> scores = new List<float>();
        
        float imageHeight = progressBarFill.rectTransform.sizeDelta.y - 100f;

        for (int i = 0; i < _playerDatas.Count; i++)
        {
            if (i == TurnManager.instance.indexCarTurn)
            {
                scores.Add(CurrentDistance);
            }
            else
            {
                scores.Add(_playerDatas[i].CurrentDistance);
            }
        }
        
        for (int i = 0; i < scores.Count; i++)
        {
            float progress = scores[i] / ((_playerDatas[firstCar].CurrentDistance < 200) ? 200 : _playerDatas[firstCar].CurrentDistance);
            
            float newPos = Mathf.Clamp(progress * imageHeight, progressBarFill.transform.position.y + 20f, imageHeight);
            Transform bar = progressBarFill.transform.GetChild(i);

            bar.position = Vector3.Lerp(bar.position, new Vector2(bar.position.x, newPos), Time.deltaTime * 10f);
        }
    }

    private void NextCP()
    {
        _playingPlayer.previousDistance += _playingPlayer.CPDistance;
        _playingPlayer.CPDistance = 0;

        if (points.Count > _playingPlayer.currentCP + 1)
        {
            _playingPlayer.currentCP++;
        }
        else
        {
            _playingPlayer.currentCP = 0;
        }
    }

    public void InitPlayer()
    {
        int index = _playerDatas.Count;
        _playerDatas.Add(new PlayerData(index));
    }

    public void LoadPlayer(int index)
    {
        if (_playerDatas.Count > index)
        {
            _playingPlayer = _playerDatas[index];
        }
        else
        {
            InitPlayer();
            _playingPlayer = _playerDatas[index];
        }
    }
}