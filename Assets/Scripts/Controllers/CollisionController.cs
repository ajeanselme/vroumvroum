using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionController : MonoBehaviour
{
    public MovementController car;
    public float bumpAngle = 45f;
    public float bumpForce = 10f;
    
    public void callCollision(CarCollider collider, Collider other)
    {
        RaycastHit hit;
        if (Physics.Raycast(collider.transform.position, collider.transform.forward, out hit, 20))
        {
            Debug.DrawRay(hit.point, hit.normal, Color.red, 5);

            float angle = Vector3.Angle(Vector3.ProjectOnPlane(car.transform.forward, hit.normal), car.transform.forward);

            if (angle < bumpAngle)
            {
                Vector3 reflection = Vector3.Reflect(car.transform.forward, hit.normal);
                car.transform.rotation = Quaternion.LookRotation(reflection, Vector3.up);
            }
            else
            {
                car.bumpCar(hit.normal, bumpForce);
            }
            
        }
    }
}
