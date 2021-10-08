using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MovementController : MonoBehaviour
{
    public Rigidbody theRB;

    [Header("Settings")]
    [Tooltip("Temps total de parcours avant décélération")]
    public float totalTime;
    
    [Header("Vitesses")]
    [Tooltip("Vitesse de départ")]
    public float initialSpeed = 8f;
    [Tooltip("Vitesse de décélération quand le temps total s'est écoulé")]
    public float decelerationSpeed = 10f;
    [Tooltip("Facteur de décélération en montée. + élevé = décélération moindre")]
    [Range(0.9f, .99f)]
    public float uphillDecelerationFactor = .95f;
    [Header("Forces")]
    [Tooltip("Force de turn")]
    public float turnStrength = 180; 
    [Tooltip("Force de gravité (vitesse à laquelle la voiture tombe quand en l'air")]
    public float gravityForce = 10f; 
    [Tooltip("Force de friction au sol")]
    public float dragOnGround = 3f;
    public ParticleSystem[] dustTrail;
    [Tooltip("Quantité de particules max")]
    public float maxEmission = 50f;

    private float _currentSpeed, _turnInput, _remainingTime;
    private bool grounded, _bumped, _groundedLastFrame;
    
    [Header("PAS TOUCHER")]
    public LayerMask whatIsGround;
    public float groundRayLength = .5f;
    public Transform groundRayPoint;

    [SerializeField] private AnimationCurve slideCurve = new AnimationCurve ( new Keyframe (-45f, 1f), new Keyframe(0f, 0f));

    private float emissionRate;
    
    private void Start()
    {
        theRB.transform.SetParent(null);
        _currentSpeed = initialSpeed;
        launchCar();
    }

    private void OnDrawGizmos()
    {
        Vector3 point_B = groundRayPoint.position + (-transform.up * groundRayLength);
        Debug.DrawLine(groundRayPoint.position, point_B, Color.black);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            launchCar();
        }
        
        _turnInput = Input.GetAxis("Horizontal");

        if (grounded && _currentSpeed > 0)
        {
            setDirection(_turnInput);
        }
        transform.position = theRB.transform.position;
    }

    private void FixedUpdate()
    {
        if (_remainingTime > 0)
        {
            _remainingTime -= 1 * Time.deltaTime;
        }
        
        float slopeAngle = (Vector3.Angle (Vector3.down, transform.forward)) - 90;
        float speedFactor = slideCurve.Evaluate(slopeAngle);
        
        grounded = false;
        RaycastHit hit;
        if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            grounded = true;
            emissionRate = 0;

            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }

        if (!_groundedLastFrame && grounded)
        {
            _bumped = false;
        }
        
        if (grounded && !_bumped)
        {
            if (_currentSpeed > .1f)
            {
                theRB.drag = dragOnGround;
                // If on flat ground
                if (slopeAngle == 0)
                {
                    if (_currentSpeed > 0)
                    {
                        // Decelerate if time expired
                        if (_remainingTime <= 0)
                        {
                            _currentSpeed -= decelerationSpeed * Time.deltaTime;
                        }
                    }
                }
                // If on slide
                else if (slopeAngle < 0)
                {
                    _currentSpeed = Mathf.Clamp(_currentSpeed + .5f * speedFactor, 0, initialSpeed * 1.2f);
                }
                // If on uphill
                else if (slopeAngle > 0)
                {
                    if (_remainingTime > 0)
                    {
                        _currentSpeed = Mathf.Clamp(_currentSpeed * uphillDecelerationFactor, 0, initialSpeed);
                    }
                    else
                    {
                        _currentSpeed = Mathf.Clamp(_currentSpeed * (uphillDecelerationFactor * .95f), 0, initialSpeed);
                        Debug.Log(_currentSpeed);
                    }
                }
                setCarSpeed(_currentSpeed);
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

        _groundedLastFrame = grounded;
    }

    public void setCarSpeed(float speed)
    {
        if (speed > 0.1f)
        {
            theRB.velocity = transform.forward * speed * 4f;
            emissionRate = maxEmission;
        }
        else
        {
            stopCar(true);
        }
    }

    public void setDirection(float turnInput)
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime, 0f));
    }

    public void launchCar()
    {
        theRB.useGravity = true;
        _remainingTime = totalTime;
        _currentSpeed = initialSpeed;
    }

    public void bumpCar(Vector3 direction, float force)
    {
        if (!_bumped)
        {
            _bumped = true;
            theRB.velocity = new Vector3((-theRB.velocity.normalized.x * force) +  (direction.normalized.x * force), 10f, (-theRB.velocity.normalized.z * force) +  (direction.normalized.z * force));
        }
    }

    public void stopCar(bool endingTurn = false)
    {
        _remainingTime = -1;
        _currentSpeed = 0;
        emissionRate = 0;
        
        theRB.useGravity = false;
        theRB.drag = 0.1f;
        theRB.velocity = Vector3.zero;
        
        if(endingTurn) TurnManager.instance.FinishTurn(this);
    }
}
