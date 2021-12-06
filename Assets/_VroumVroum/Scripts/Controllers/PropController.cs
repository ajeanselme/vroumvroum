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
        float converted =  direction.magnitude * ((12f - weight) / 100f);
        
        Vector3 force = new Vector3( direction.x * converted, Mathf.Clamp(2f * converted, 2f, 10f),
            direction.z * converted);
        
        GetComponent<Rigidbody>().AddForce(force, ForceMode.VelocityChange);
    }
}
