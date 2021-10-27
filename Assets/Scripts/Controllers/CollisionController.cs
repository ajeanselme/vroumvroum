using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class CollisionController : MonoBehaviour
{
    public CarController car;
    public float bumpAngle = 45f;
    public float bumpForce = 10f;

    private MMFeedbacks _feedbacks;

    private void Awake()
    {
        _feedbacks = GetComponent<MMFeedbacks>();
    }

    public void callCollision(Collision collision)
    {
        Debug.DrawRay(collision.contacts[0].point, collision.contacts[0].normal, Color.red, 5);

        float angle = Vector3.Angle(Vector3.ProjectOnPlane(car.transform.forward, collision.contacts[0].normal), car.transform.forward);

        if (angle < bumpAngle)
        {
            Vector3 reflection = Vector3.Reflect(car.transform.forward, collision.contacts[0].normal);
            car.transform.rotation = Quaternion.LookRotation(reflection, Vector3.up);
        }
        else
        {
            car.bumpCar(-transform.forward, bumpForce);
            _feedbacks.PlayFeedbacks();
        }
    }
}
