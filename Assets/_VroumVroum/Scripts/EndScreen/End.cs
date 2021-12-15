using System;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class End : MonoBehaviour
{
    public static End instance;

    private bool endScroll = false;
    private int divided;

    private int carIndex = 5;
    private float timer = -10f;
    private float timeWait;
    private Vector3 lastCamPos, nextCamPos;
    private bool showScore = false;

    private float[] scores;
    
    [SerializeField] private Camera camera;
    [SerializeField] private Transform[] carSpawnPoints;
    [SerializeField] private TextMesh[] scoresTexts;
    [Space]
    [SerializeField] private float timeTransitionCar;
    [SerializeField] private float timeTransitionEnd;
    [SerializeField] private float timePause;
    [Space]
    [SerializeField] private Text endText;

    [SerializeField] private RectTransform endMenu;
    [SerializeField] private Button firstButton;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        endMenu.gameObject.SetActive(false);
        for (int i = 0; i < ReInput.players.playerCount; i++)
        {
            ReInput.players.Players[i].controllers.maps.SetMapsEnabled(false, "Default");
            ReInput.players.Players[i].controllers.maps.SetMapsEnabled(false, "Minigame");
            ReInput.players.Players[i].controllers.maps.SetMapsEnabled(true, "Menu");
        }
    }

    private void Update()
    {
        if (timer > -5f)
        {
            timer -= Time.deltaTime;
            
            camera.transform.position = Vector3.Lerp(lastCamPos, nextCamPos, 1f - timer / timeWait);

            if (!showScore && timer <= 0f)
            {
                showScore = true;
                if (carIndex < scoresTexts.Length)
                    scoresTexts[carIndex].text = (int)scores[carIndex] + "cm";
                // add effects
            }
            
            if (timer <= -timePause)
            {
                NextStep();
            }
        }

        if (endScroll && ReInput.players.GetPlayer(0).GetAnyButton())
        {
            endMenu.gameObject.SetActive(true);
            firstButton.Select();
            endMenu.position = new Vector2(Screen.width / 2f, Screen.height / 2f);
        }
    }

    public void SetEnd(GameObject[] _cars, float[] _scores)
    {
        scores = _scores;
        for (int i = 0; i < _cars.Length; i++)
        {
            _cars[i].transform.parent = carSpawnPoints[i];
            _cars[i].transform.localPosition = Vector3.zero;
        }

        carIndex = _cars.Length;
        divided = carIndex;
        camera.transform.position = new Vector3(carSpawnPoints[carIndex-1].position.x - 5f, camera.transform.position.y, camera.transform.position.z);
        lastCamPos = camera.transform.position;
        
        NextStep();
    }

    private void NextStep()
    {
        carIndex--;
        showScore = false;
        
        if (carIndex < -1) return;

        if (carIndex >= 0)
        {
            nextCamPos = new Vector3(carSpawnPoints[carIndex].position.x, camera.transform.position.y, camera.transform.position.z);
            timeWait = timeTransitionCar;
            timer = timeTransitionCar;
        }
        else
        {
            float xPos = 0f;
            for (int i = 0; i < divided; i++)
                xPos += carSpawnPoints[i].position.x;
            
            nextCamPos = new Vector3(xPos / divided, 4.5f, 6.5f);
            timeWait = timeTransitionEnd;
            timer = timeTransitionEnd;

            endScroll = true;
            endText.text = "Press any button to continue";
            showScore = true;
        }
        
        lastCamPos = camera.transform.position;
    }
}
