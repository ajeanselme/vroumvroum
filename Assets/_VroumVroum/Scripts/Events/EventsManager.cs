using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsManager : MonoBehaviour
{
    public static EventsManager instance;
    
    [Serializable]
    public class Step
    {
        public GameObject gameObject;
        public Vector3 basePosition;
        public Vector3 targetPosition;
    }
    public List<Step> stepList = new List<Step>();
    public float chessSpeed = 1f;
    private int chessStep = 0;
    private bool order = true;
    
    public bool isOn;

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
        foreach (var step in stepList)
        {
            step.basePosition = step.gameObject.transform.localPosition;
        }
    }

    private void FixedUpdate()
    {
        if (isOn)
        {
            if (order)
            {
                if (stepList.Count > chessStep)
                {
                    if (stepList[chessStep].gameObject.transform.localPosition != stepList[chessStep].targetPosition)
                    {
                        stepList[chessStep].gameObject.transform.localPosition = Vector3.MoveTowards(stepList[chessStep].gameObject.transform.localPosition, stepList[chessStep].targetPosition, chessSpeed / 10f);
                    }
                    else
                    {
                        chessStep++;
                    }
                }
                else
                {
                    chessStep--;
                    order = false;
                }
            }
            else
            {
                if (chessStep >= 0)
                {
                    if (stepList[chessStep].gameObject.transform.localPosition != stepList[chessStep].basePosition)
                    {
                        stepList[chessStep].gameObject.transform.localPosition = Vector3.MoveTowards(stepList[chessStep].gameObject.transform.localPosition, stepList[chessStep].basePosition, chessSpeed / 10f);
                    }
                    else
                    {
                        chessStep--;
                    }
                }
                else
                {
                    order = true;
                    chessStep++;
                }
            }
        }
    }
}
