using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropController : MonoBehaviour
{
    [Range(1,8)]
    public int weight = 1;

    public void callCollision(Vector3 direction)
    {
        float converted = (direction.magnitude / 5f) * (1f - weight / 10f);
        Debug.Log(converted);
        // GetComponent<Rigidbody>().AddForce(direction.x * 2f, 5, direction.z * 2f, ForceMode.Impulse);
        GetComponent<Rigidbody>().AddForce(direction.x * converted, 2f * converted, direction.z * converted, ForceMode.VelocityChange);
    }
}
