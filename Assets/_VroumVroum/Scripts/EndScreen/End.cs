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

    private int carIndex = 5;
    private float timer = 0f;
    private Vector3 lastCamPos, nextCamPos;
    private bool showScore = false;

    private float[] scores;
        
    [SerializeField] private Camera camera;
    [SerializeField] private Transform[] carSpawnPoints;
    [SerializeField] private TextMesh[] scoresTexts;
    [Space]
    [SerializeField] private Text endText;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (timer > -5f)
        {
            timer -= Time.deltaTime;
            
            camera.transform.position = Vector3.Lerp(lastCamPos, nextCamPos, 1f - timer);

            if (!showScore && timer <= 0f)
            {
                showScore = true;
                scoresTexts[carIndex].text = scores[carIndex].ToString();
                // add effects
            }
            
            if (timer <= -5f)
            {
                NextStep();
            }
        }

        if (endScroll && ReInput.players.GetPlayer(0).GetAnyButton())
        {
            SceneManager.LoadScene(1);
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
        }
        else
        {
            nextCamPos = new Vector3((carSpawnPoints[carSpawnPoints.Length - 1].position.x - carSpawnPoints[0].position.x) / 2f, 4.5f, 6.5f);

            endScroll = true;
            endText.text = "Press any button to continue";
            showScore = true;
        }
        
        timer = 1f;
        lastCamPos = camera.transform.position;
    }
}
