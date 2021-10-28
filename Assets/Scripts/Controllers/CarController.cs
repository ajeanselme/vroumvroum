using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Serialization;

public class CarController : MonoBehaviour
{
    public bool Debugging;

    [Header("Settings")]
    [Tooltip("Temps total de parcours avant décélération")]
    public float totalTime;
    
    [Header("Vitesses")]
    [Tooltip("Vitesse de départ")]
    public float initialSpeed = 8f;
    [Tooltip("Vitesse de décélération quand le temps total s'est écoulé")]
    public float decelerationSpeed = 10f;
    [Header("Forces")]
    [Tooltip("Force de turn")]
    public float turnStrength = 180; 
    [Tooltip("Force de gravité (vitesse à laquelle la voiture tombe quand en l'air")]
    public float gravityForce = 10f; 
    [Tooltip("Force appliquée vers le bas de la voiture pour régler le bounce")]
    public float antiBounceForce = 10f; 
    [Tooltip("Quantité de particules max")]
    public float trailMaxEmission = 50f;
    public float speedEmissionFactor = 50f;

    [Header("Tresholds")]
    [Tooltip("Si airtime en dessous de cette valeur, aucune animation")]
    public float airIgnoring = .2f;
    [Tooltip("Si airtime en dessous de cette valeur, small bump, sinon big")]
    public float airSmallBump = .7f;

    [Tooltip("Facteurs de vitesse dans les pentes")]
    [SerializeField] private AnimationCurve slideCurve = new AnimationCurve ( new Keyframe (0f, 0f), new Keyframe(90f, 1f));
    [SerializeField] private AnimationCurve uphillCurve = new AnimationCurve ( new Keyframe (0f, 1f), new Keyframe(90f, 0f));
    
    
    [Header("PAS TOUCHER")]
    public Rigidbody theRB;
    public ParticleSystem[] dustTrail;
    public LayerMask whatIsGround;
    
    [Range(.3f, .5f)]
    public float groundRayLength = .35f;
    public GameObject[] wheels;
    public float wheelOffset = 0f;

    public CinemachineVirtualCamera vcam;

    private float _turnInput, _remainingTime, _currentSpeed, _emissionRate, _airTime, _initialFOV;
    private bool _grounded, _bumped, _groundedLastFrame;

    private Animator _animator;
    
    private Quaternion _nextRotation;
    private float _slopeAngle = 0f;

    private MMFeedbacks _feedbacks;

    //Debug variables
    private float _rotationDamping;
    private GUIStyle debugWindowStyle, debugTextBoxStyle;

    private void Start()
    {
        debugWindowStyle = new GUIStyle();
        debugWindowStyle.normal.background = Texture2D.grayTexture;

        debugTextBoxStyle = new GUIStyle();
        debugTextBoxStyle.normal.textColor = Color.white;
        Texture2D newTex = new Texture2D(64,64);
        
        for (int y = 0; y < newTex.height; y++)
        {
            for (int x = 0; x < newTex.width; x++)
            {
                newTex.SetPixel(x, y, new Color(45f / 255f, 45f / 255f, 45f / 255f));
            }
        }
        
        for (int x = 0; x < newTex.width; x++)
        {
            newTex.SetPixel(x, 0, Color.black);
        }
        
        newTex.Apply();
        debugTextBoxStyle.normal.background = newTex;

        /*
         * Set the RB as an independant GO
         * The mesh will then follow the RB position in the update
         */
        theRB.transform.SetParent(null);
        theRB.constraints = RigidbodyConstraints.FreezeRotation;
        
        /*
         * Setup runtime values
         */
        _currentSpeed = initialSpeed;
        _nextRotation = transform.rotation;
        _initialFOV = vcam.m_Lens.FieldOfView;

        _feedbacks = GetComponent<MMFeedbacks>();
        _animator = GetComponent<Animator>();
        
        launchCar();
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10,10,200, Screen.height));
        GUILayout.Space(2);
        GUILayout.Box("Debug Window [P]");
        if (Debugging)
        {
            GUILayout.BeginArea(new Rect(0,25,200,Screen.height), "", debugWindowStyle);

            GUILayout.BeginHorizontal(debugTextBoxStyle);
                GUILayout.Label("State : " + (_grounded ? "ground" : "air"));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal(debugTextBoxStyle);
                GUILayout.Label("Current Speed : " + _currentSpeed);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal(debugTextBoxStyle);
                GUILayout.Label("Slope : " + _slopeAngle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(debugTextBoxStyle);
                GUILayout.Label("Time Left : " + _remainingTime);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(debugTextBoxStyle);
                GUILayout.Label("Air time : " + _airTime);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal(debugTextBoxStyle);
                GUILayout.Label("Checkpoint : " + CheckpointsController.instance.CurrentCP);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(debugTextBoxStyle);
                GUILayout.Label("Distance : " + CheckpointsController.instance.CurrentDistance);
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }
        GUILayout.EndArea();

    }

    private void OnDrawGizmosSelected()
    {
        /*
         * Drawing Guizmos of each detectors for debug purposes
         */

        foreach (GameObject wheel in wheels)
        {
            Vector3 point_A = new Vector3(wheel.transform.position.x, 
                wheel.transform.position.y + wheelOffset,
                wheel.transform.position.z);
            Vector3 point_B = wheel.transform.position + (-transform.up * groundRayLength);
            Debug.DrawLine(point_A, point_B, Color.red);
        }
    }

    private void Update()
    {
        
        /*
         * Reset the grounded state and check each wheel for ground contact.
         * Attention: The front wheels must be the first in the wheel tables
         * otherwhise the car won't adapt correctly to the slope.
         */
        _grounded = false;
        
        foreach (GameObject wheel in wheels)
        {
            RaycastHit hit;
            Vector3 point_A = new Vector3(wheel.transform.position.x, 
                wheel.transform.position.y + wheelOffset,
                wheel.transform.position.z);
            if (Physics.Raycast(point_A, -wheel.transform.up, out hit, groundRayLength + wheelOffset, whatIsGround))
            {
                _grounded = true;
                _emissionRate = 0;
                _nextRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
                break;
            }
        }

        if (!_grounded)
        {
            _airTime += Time.deltaTime;
        }

        /*
         * Change car direction based on Horizontal input 
         */
        _turnInput = Input.GetAxis("Horizontal");
        if (_grounded && _currentSpeed > 0)
        {
            setDirection(_turnInput);
        }
        
        
        /*
         * Finally update the general GO to match the RB position, and Lerp the new rotation based on the ground
         */
        transform.position = theRB.transform.position;
        _rotationDamping = Mathf.Abs(90 - _slopeAngle);
        transform.rotation = Quaternion.Lerp(transform.rotation, _nextRotation, Time.deltaTime * 90f);

        if (Input.GetKeyDown(KeyCode.P))
        {
            Debugging = !Debugging;
        }
        
        if (Debugging)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                launchCar();
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                jumpCar();
            }
        }
    }

    private void FixedUpdate()
    {
        // Countdown before deceleration
        if (_remainingTime > 0)
        {
            _remainingTime -= 1 * Time.deltaTime;
        }


        if (!_groundedLastFrame && _grounded)
        {
            _bumped = false;
            landCar();
        }
        
        /*
         * Handling car speed
         */
        if (_grounded && !_bumped)
        {
            if (_currentSpeed > .1f)
            {
                // Manage speed based on slope
                _slopeAngle = (Vector3.Angle (Vector3.down, transform.forward)) - 90;
                
                var emission = TurnManager.instance.speedParticles.emission;

                // If on flat ground
                if (_slopeAngle > -1f && _slopeAngle < 1f)
                {
                    if (_currentSpeed > 0)
                    {
                        // Decelerate if time expired
                        if (_remainingTime > 0)
                        {
                            _currentSpeed = initialSpeed;
                        }
                        else
                        {
                            _currentSpeed -= decelerationSpeed * Time.deltaTime;
                        }
                    }
                    
                    emission.rateOverTime = speedEmissionFactor * (_currentSpeed / initialSpeed);
                }
                // If on slide
                else if (_slopeAngle < -1f)
                {
                    if (_remainingTime > 1f)
                    {
                        _currentSpeed = Mathf.Clamp(initialSpeed + .5f * slideCurve.Evaluate(-_slopeAngle), 0, initialSpeed * 1.2f);
                    }
                    else
                    {
                        _currentSpeed = Mathf.Clamp(_currentSpeed + slideCurve.Evaluate(-_slopeAngle) * Time.deltaTime, 0, initialSpeed * 1.2f);
                    }
                    
                    emission.rateOverTime = (speedEmissionFactor + speedEmissionFactor * (5 * (-_slopeAngle / 45))) * (_currentSpeed / initialSpeed);
                }
                // If on uphill
                else if (_slopeAngle > 1f)
                {
                    if (_remainingTime > 1f)
                    {
                        _currentSpeed = Mathf.Clamp(initialSpeed * uphillCurve.Evaluate(_slopeAngle), 0, initialSpeed);
                    }
                    else
                    {
                        _currentSpeed -= decelerationSpeed * Mathf.Clamp(uphillCurve.Evaluate(_slopeAngle), .95f, 1f) * Time.deltaTime;
                    }
                    emission.rateOverTime = (speedEmissionFactor * (1 - _slopeAngle / 45)) * (_currentSpeed / initialSpeed);
                }
                setCarSpeed(_currentSpeed);
            }
            
            // Change bouncing effect
            theRB.AddForce(-theRB.transform.up * antiBounceForce * 100f);
        }
        else
        {
            // If in the air apply gravity force
            theRB.AddForce(Vector3.up * -gravityForce * 100f);
        }

        foreach (ParticleSystem part in dustTrail)
        {
            var emissionModule = part.emission;
            emissionModule.rateOverTime = _emissionRate;
        }

        _groundedLastFrame = _grounded;
    }

    public void setCarSpeed(float speed)
    {
        if (speed > 0.1f)
        {
            theRB.velocity = transform.forward * speed * 4f;
            _emissionRate = trailMaxEmission;
        }
        else
        {
            stopCar(true);
        }
    }

    public void setDirection(float turnInput)
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Mathf.Clamp(_currentSpeed / 2f, 0f, 1f) * Time.deltaTime, 0f));
        wheels[0].transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * 45, 0f));
        wheels[1].transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * 45, 0f));
    }

    public void launchCar()
    {
        theRB.constraints = RigidbodyConstraints.FreezeRotation;
        _remainingTime = totalTime;
        _currentSpeed = initialSpeed;
    }

    public void landCar()
    {
        if (_airTime < airIgnoring)
        {
            // Debug.Log("No land");
        }
        else if(_airTime < airSmallBump)
        {
            // Debug.Log("Small bump");
            _feedbacks.Feedbacks[1].Active = false;
            _feedbacks.Feedbacks[2].Active = false;
            _animator.SetInteger("Power", 0);
            _feedbacks.PlayFeedbacks();
        }
        else
        {
            // Debug.Log("Full bounce");
            _feedbacks.Feedbacks[1].Active = true;
            _feedbacks.Feedbacks[2].Active = true;
            _animator.SetInteger("Power", 1);

            _feedbacks.PlayFeedbacks();
        }
        
        _airTime = 0f;
    }

    public void bumpCar(Vector3 direction, float force)
    {
        if (!_bumped)
        {
            _bumped = true;
            theRB.velocity = new Vector3((-theRB.velocity.normalized.x * force) +  (direction.normalized.x * force), force, (-theRB.velocity.normalized.z * force) +  (direction.normalized.z * force));
        }
    }

    public void stopCar(bool endingTurn = false)
    {
        _remainingTime = -1;
        _currentSpeed = 0;
        _emissionRate = 0;
        
        theRB.constraints = RigidbodyConstraints.FreezeAll;
        
        if(endingTurn) TurnManager.instance.FinishTurn(this);
    }

    void jumpCar()
    {
        theRB.MovePosition(new Vector3(theRB.position.x, theRB.position.y + 2, theRB.position.z));
    }
}
