using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTEIcon : MonoBehaviour
{
    public bool isOn = false;
    public GameObject canvas;
    public GameObject trigger;

    public float scaleMultiplier = 1;

    private Vector3 initialCanvasScale;

    private void Start()
    {
        initialCanvasScale = canvas.transform.localScale;
        canvas.SetActive(false);
    }

    private void Update()
    {
        CarController carController = TurnManager.instance.cars[TurnManager.instance.indexCarTurn];
        canvas.transform.LookAt(carController.vcam.transform);
        
        float distance = Vector3.Distance(carController.transform.position, transform.position);

        Vector3 newScale = initialCanvasScale * distance / 100f * scaleMultiplier;
        if (newScale.x > initialCanvasScale.x)
        {
            newScale = initialCanvasScale;
        }
        
        canvas.transform.localScale = newScale;
    }

    public void Toggle()
    {
        canvas.SetActive(!isOn);
        isOn = !isOn;
    }
}
