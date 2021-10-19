using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollider : MonoBehaviour
{
    private MovementController car;

    private void Start()
    {
        car = transform.parent.GetComponent<CollisionController>().car;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!other.transform.IsChildOf(car.transform) && other.gameObject != car.theRB.gameObject)
        {
            transform.GetComponentInParent<CollisionController>().callCollision(other);
        }
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (!other.transform.IsChildOf(car.transform) && other.gameObject != car.theRB.gameObject)
    //     {
    //         transform.GetComponentInParent<CollisionController>().callCollision(this, other);
    //     }
    // }
}
