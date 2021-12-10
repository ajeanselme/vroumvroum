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
            Debug.Log("a");
            if (order)
            {
                Debug.Log("b");
                if (stepList.Count > chessStep)
                {
                    Debug.Log("c");
                    if (stepList[chessStep].gameObject.transform.localPosition != stepList[chessStep].targetPosition)
                    {
                        Debug.Log("d");
                        stepList[chessStep].gameObject.transform.localPosition = Vector3.MoveTowards(stepList[chessStep].gameObject.transform.localPosition, stepList[chessStep].targetPosition, chessSpeed / 10f);
                        
                    }
                    else
                    {
                        Debug.Log("e");
                        chessStep++;
                    }
                }
                else
                {
                    Debug.Log("f");
                    chessStep--;
                    order = false;
                }
            }
            else
            {
                Debug.Log("g");
                if (chessStep >= 0)
                {
                    Debug.Log("h");
                    if (stepList[chessStep].gameObject.transform.localPosition != stepList[chessStep].basePosition)
                    {
                        Debug.Log("i");
                        stepList[chessStep].gameObject.transform.localPosition = Vector3.MoveTowards(stepList[chessStep].gameObject.transform.localPosition, stepList[chessStep].basePosition, chessSpeed / 10f);
                    }
                    else
                    {
                        Debug.Log("j");
                        chessStep--;
                    }
                }
                else
                {
                    Debug.Log("k");
                    order = true;
                    chessStep++;
                }
            }
        }
    }
}
