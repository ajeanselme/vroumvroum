using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollider : MonoBehaviour
{
    private CarController car;

    private void Start()
    {
        car = transform.parent.GetComponent<CollisionController>().car;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<PropController>())
        {
            other.gameObject.GetComponent<PropController>().callCollision(car.theRB.velocity);
        }
        else
        {
            if (!other.transform.IsChildOf(car.transform) && other.gameObject != car.theRB.gameObject)
            {
                transform.GetComponentInParent<CollisionController>().callCollision(other);
            }
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
