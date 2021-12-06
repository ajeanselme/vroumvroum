using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropController : MonoBehaviour
{
    [Range(1,10)]
    public int weight = 1;

    private Rigidbody theRB;
    
    private void Awake()
    {
        if (GetComponent<Rigidbody>())
        {
            theRB = GetComponent<Rigidbody>();
        }
        else
        {
            theRB = gameObject.AddComponent<Rigidbody>();
        }

        theRB.drag = 1f;
    }

    public void callCollision(Vector3 direction)
    {
        // float converted = (direction.magnitude / 5f) * (1f - weight / 10f);
        float converted = direction.magnitude * (weight / 100f) / 20f;
        if (converted == 0) converted = 1;
        
        Vector3 force = new Vector3( direction.x * converted, Mathf.Clamp(2f * converted, 2f, 10f),
            direction.z * converted);
        // GetComponent<Rigidbody>().AddForce(direction.x * 2f, 5, direction.z * 2f, ForceMode.Impulse);
        GetComponent<Rigidbody>().AddForce(force, ForceMode.VelocityChange);
    }
}
