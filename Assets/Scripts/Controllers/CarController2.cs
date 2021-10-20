using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController2 : MonoBehaviour
{
    public float initialSpeed = 10f;
    public float torque = 10f;
    [Range(0f,1f)]
    public float friction = .5f;
    public GameObject centerOfMass;
    
    [Tooltip("Force de gravité (vitesse à laquelle la voiture tombe quand en l'air")]
    public float gravityForce = 10f; 
    
    [Header("PAS TOUCHER")]
    public LayerMask whatIsGround;
    [Range(.3f, .5f)]
    public float groundRayLength = .35f;
    public Transform groundRayPoint;

    public bool debug;

    private Rigidbody tractor;
    private bool _grounded;
    private Vector3 _lastVelocity;
    
    private void Start()
    {
        tractor = GetComponent<Rigidbody>();
        tractor.centerOfMass = centerOfMass.transform.localPosition;
        
        launchCar();
    }

    private void OnDrawGizmos()
    {
        
        Vector3 point_B = groundRayPoint.transform.position + (-transform.up * groundRayLength);
        Debug.DrawLine(groundRayPoint.transform.position, point_B, Color.red);
    }

    private void OnGUI()
    {
        if (debug)
        {
            GUILayout.BeginArea(new Rect(10,10,100,200));
            GUILayout.Space(2);
            GUILayout.Box("Debug Window");
            GUILayout.Box("State : " + (_grounded ? "ground" : "air"));
            GUILayout.Box("Current Speed : " + tractor.velocity.magnitude);
            GUILayout.EndArea();
        }
    }
    
    private void Update()
    {
        
        RaycastHit hit;
        if (Physics.Raycast(groundRayPoint.transform.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            _grounded = true;
        }
        else
        {
            _grounded = false;
        }

        // if (Input.GetAxis("Vertical") != 0)
        // {
        //     tractor.AddForce(transform.forward * initialSpeed * Input.GetAxis("Vertical"));
        // }
        
    }

    private void FixedUpdate()
    {
        if (_grounded)
        {
            // Set forward force
            tractor.velocity = transform.forward * initialSpeed * 4f;
            
            if (Input.GetAxis("Horizontal") != 0)
            {
                tractor.AddTorque(transform.up * (torque / 100f) * Input.GetAxis("Horizontal"), ForceMode.VelocityChange);
            }
            
            // Add lateral force
            // float lateralSpeed = Vector3.Dot(transform.right, _lastVelocity) * 2f;
            // Debug.Log(lateralSpeed);
            // tractor.velocity = tractor.velocity + (-lateralSpeed * transform.right * friction);
        }
        else
        {
            tractor.AddForce(Vector3.up * -gravityForce * 100f);
        }

        _lastVelocity = tractor.velocity;
    }

    private void launchCar()
    {
        // tractor.AddForce(transform.forward * initialSpeed, ForceMode.Impulse);
    }
}
