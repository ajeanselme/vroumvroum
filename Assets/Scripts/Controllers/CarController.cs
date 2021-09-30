using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CarController : MonoBehaviour
{
    public Rigidbody theRB;

    public float forwardAccel = 8f, reverseAccel = 4f, maxSpeed = 50f, turnStrength = 180, gravityForce = 10f, dragOnGround = 3f;

    private float _speedInput, _turnInput;
    private bool grounded;
    
    public LayerMask whatIsGround;
    public float groundRayLength = .5f;
    public Transform groundRayPoint;

    public ParticleSystem[] dustTrail;
    public float maxEmission = 50f;
    private float emissionRate;
    
    private void Start()
    {
        theRB.transform.SetParent(null);
    }

    private void Update()
    {
        _speedInput = 0;
        if (Input.GetAxis("Vertical") > 0)
        {
            _speedInput = Input.GetAxis("Vertical") * forwardAccel * 1000f;
        }
        else if(Input.GetAxis("Vertical") < 0)
        {
            _speedInput = Input.GetAxis("Vertical") * reverseAccel * 1000f;
        }

        _turnInput = Input.GetAxis("Horizontal");

        if (grounded)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, _turnInput * turnStrength * Time.deltaTime * Input.GetAxis("Vertical"), 0f));
        }
        transform.position = theRB.transform.position;
    }

    private void FixedUpdate()
    {
        grounded = false;
        RaycastHit hit;
        if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            grounded = true;
            
            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }

        emissionRate = 0;

        if (grounded)
        {
            theRB.drag = dragOnGround;
            if (Mathf.Abs(_speedInput) > 0)
            {
                theRB.AddForce(transform.forward * _speedInput);

                emissionRate = maxEmission;
            }
        }
        else
        {
            theRB.drag = 0.1f;
            theRB.AddForce(Vector3.up * -gravityForce * 100f);
        }

        foreach (ParticleSystem part in dustTrail)
        {
            var emissionModule = part.emission;
            emissionModule.rateOverTime = emissionRate;
        }
    }


    // private Rigidbody rb;

    // [Header("Timers")]
    // [Tooltip("Temps total de vie de la voiture, avant décélération")]
    // public float totalTime = 10;
    //
    // [Header("Speeds")]
    // [Tooltip("Vitesse de départ")]
    // public float initialSpeed = 10;
    // [Tooltip("Vitesse de décélération qquand le temps total s'est écoulé")]
    // public float decelerationSpeed = 1;
    // public float steering = 1f;
    //
    // private bool _moving;
    // private float _currentRotation, _rotate;
    // private void Start()
    // {
    //     rb = GetComponent<Rigidbody>();
    //     Debug.Log("dece " + decelerationSpeed);
    // }
    //
    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         Debug.Log("launch");
    //         _moving = true;
    //     }
    //
    //     if (Input.GetAxis("Horizontal") != 0 && _moving)
    //     {
    //         int dir = Input.GetAxis("Horizontal") > 0f ? 1 : -1;
    //         float amount = Mathf.Abs(Input.GetAxis("Horizontal"));
    //         Steer(dir, amount);
    //     }
    //
    //     _currentRotation = Mathf.Lerp(_currentRotation, _rotate, 8f * Time.deltaTime);
    //     _rotate = 0f;
    // }
    //
    // private void FixedUpdate()
    // {
    //     // Rotation
    //     transform.eulerAngles = Vector3.Lerp(transform.eulerAngles,
    //         new Vector3(0f, transform.eulerAngles.y + _currentRotation, 0f), 10f * Time.deltaTime);;
    //     
    //     if (_moving)
    //     {
    //         // Movement
    //         transform.position += transform.forward * initialSpeed;
    //         totalTime -= 1 * Time.deltaTime;
    //         if (totalTime <= 0)
    //         {
    //             initialSpeed -= decelerationSpeed * Time.deltaTime;
    //         }
    //         if (initialSpeed <= 0)
    //         {
    //             _moving = false;
    //             Debug.Log("stop");
    //         }
    //     }
    // }
    //
    // void Steer(int direction, float amount)
    // {
    //     _rotate = (steering * direction) * amount;
    // }
}
