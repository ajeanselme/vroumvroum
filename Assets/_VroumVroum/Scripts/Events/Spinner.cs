using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    public Vector3 positionA;
    public Vector3 positionB;

    public float speed = 1f;
    
    private bool _movingForward = true;
    

    private void Start()
    {
        transform.localPosition = positionA;
    }

    private void FixedUpdate()
    {
        if (_movingForward)
        {
            if(transform.localPosition != positionB){
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, positionB, speed / 10f);
            }
            else
            {
                _movingForward = false;
            }
        }
        else
        {
            if(transform.localPosition != positionA){
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, positionA, speed / 10f);
            }
            else
            {
                _movingForward = true;
            }
        }
    }
}
