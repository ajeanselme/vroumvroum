using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
        public int listIndex;
        public Vector3 position;
        public Vector3 rotation;
        public float distance;
        public bool foldout = false;

        public Checkpoint(Vector3 vector3)
        {
            this.position = vector3;
        }
    }

    class PlayerData
    {
        public int index;
        public int ladderPosition, currentCP;
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
    public GameObject ladderBar;

    public AudioClip[] ladderAudioClips;
    public AudioSource audio;

    
    private PlayerData _playingPlayer;
    private int firstCar = 0;
    // private float _CPDistance, _previousDistance, _lastProgress;
    public float CurrentCP => _playingPlayer.currentCP;
    public float CurrentDistance => _playingPlayer.CurrentDistance;

    private List<PlayerData> _playerDatas = new List<PlayerData>();

    private float _lastUpdate;
    private Slider[] sliders;
    
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

    private void Start()
    {
        audio = GetComponent<AudioSource>();

        sliders = new Slider[ladderBar.transform.childCount];
        for (int i = 0; i < ladderBar.transform.childCount; i++)
        {
            sliders[i] = ladderBar.transform.GetChild(i).GetComponent<Slider>();
            sliders[i].value = 0f;
        }
    }

    private void Update()
    {
        if (_playingPlayer != null)
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
                

                Vector3 pointA = currentCP.position;
                Vector3 pointB = nextCP.position;

                Vector3 player = TurnManager.instance.playerList[TurnManager.instance.indexCarTurn].carController.transform.position;

                Vector3 projected = Vector3.Project((player - pointA), (pointB - pointA)) + pointA;
                
                
                float CPDistance = Vector3.Distance(pointA, pointB);
                float projetedDistance = Vector3.Distance(pointA, projected);

                float progress = projetedDistance / CPDistance;

                // debug.transform.position = projected;
                
                _playingPlayer.CPDistance = nextCP.distance * progress;

                if (_playingPlayer.CurrentDistance > _playerDatas[firstCar].CurrentDistance)
                {
                    firstCar = _playingPlayer.index;
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
            
            if (_lastUpdate < Time.fixedTime)
            {
                OrderPositions();
            }
        }
        else
        {
            Debug.LogError("No Player created");
            Application.Quit();
        }
    }

    private void UpdateUI()
    {
        float[] scores = new float[_playerDatas.Count];
        for (int i = 0; i < _playerDatas.Count; i++)
        {
            if (i == TurnManager.instance.indexCarTurn)
            {
                scores[i] = (CurrentDistance);
            }
            else
            {
                scores[i] = (_playerDatas[i].CurrentDistance);
            }
        }
        
        for (int i = 0; i < scores.Length; i++)
        {
            float progress = Mathf.Clamp(scores[i] / (_playerDatas[firstCar].CurrentDistance > 200f ? _playerDatas[firstCar].CurrentDistance : 200f), .1f, 1f);
            if(progress > 0)
                sliders[i].value = progress;
            
            sliders[i].transform.SetSiblingIndex(_playerDatas[i].ladderPosition);
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

    public void OrderPositions()
    {
        List<PlayerData> newlist = new List<PlayerData>();
        newlist.AddRange(_playerDatas);
        newlist.Sort(delegate(PlayerData c1, PlayerData c2) { return c2.CurrentDistance.CompareTo(c1.CurrentDistance); });

        for (int i = 0; i < newlist.Count; i++)
        {
            if (newlist[i].index == _playingPlayer.index)
            {            
                int oldLadder = _playerDatas[newlist[i].index].ladderPosition;
                if (oldLadder > i + 1 && i < 3)
                {
                    audio.clip = ladderAudioClips[i];
                    audio.Play();
                    if (i == 0)
                    {
                        UI.GetComponent<Animator>().SetTrigger("Confetti");
                        ladderBar.transform.DOShakePosition(1.3f, new Vector3(5, 0, 0), 10, 0, true, false);
                        ladderBar.transform.DOShakeRotation(1.3f, new Vector3(0, 0, 2), 20, 90, false);
                    }
                    else
                    {
                        ladderBar.transform.DOPunchScale(new Vector3(.7f, 0, 0), .7f, 2, .2f);
                        UI.GetComponent<Animator>().SetTrigger("Blow");
                    }
                }
            }
            _playerDatas[newlist[i].index].ladderPosition = i + 1;
        }
        // Debug.Log("Position " + _playingPlayer.ladderPosition);
        _lastUpdate = Time.fixedTime + .5f;
    }
}
