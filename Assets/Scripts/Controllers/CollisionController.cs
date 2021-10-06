using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionController : MonoBehaviour
{
    public MovementController car;
    
    public void callCollision(CarCollider collider, Collider other)
    {
        Debug.Log(collider.gameObject.name);

        RaycastHit hit;
        if (Physics.Raycast(collider.transform.position, transform.forward, out hit, 2))
        {
            car.transform.Rotate(Vector3.Reflect(car.transform.forward, hit.normal));
        }
    }
}
