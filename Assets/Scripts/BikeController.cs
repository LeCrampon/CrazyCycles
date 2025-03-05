using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BikeController : MonoBehaviour
{
    public Rigidbody rigidBody;
    private Collider _collider;

    public LayerMask groundMask;

    //Movement, acceleration & braking
    [Header("Movement values")]
    [SerializeField]
    private float _acceleration = .5f;
    [SerializeField]
    private float _maxSpeed = 15f;
    [SerializeField]
    private float _currentSpeed = 0f;

    public GameObject animeLines;

    public float CurrentSpeed   // property
    {
        get { return _currentSpeed; }   // get method
        set { _currentSpeed = value; }  // set method
    }
    [SerializeField]
    private float _currentAcceleration = 0f;
    [SerializeField]
    private float _maxTurnSpeed = 5f;
    [SerializeField]
    private float _minTurnSpeed = .5f;
    [SerializeField]
    private float _currentTurnSpeed = 0;
    [SerializeField]
    private float _turnValue;

    [SerializeField]
    private float _brakingMax = 10f;
    [SerializeField]
    private float _brakingStep = 1;
    [SerializeField]
    private float _currentBraking = 0;




    private BikeInput _bikeInput;

    public bool accelerating;

    public Vector2 cameraLook;
    public float cameraRotationSpeed;
    public Transform cameraTransform;

    [SerializeField]
    private bool _moving = false;
    [SerializeField]
    private bool _braking = false;



    public bool Braking   // property
    {
        get { return _braking; }   // get method
        set { }  // set method
    }

    [SerializeField]
    private bool _grounded = false;

    [SerializeField]
    private Transform _groundRaycastTransformFront;
    
    [SerializeField]
    private Transform _groundRaycastTransformBack;


    public Vector3 hitNormal;

    //The target the camera is following
    public Transform cameraTarget;
    public Vector3 cameraTargetPosition;
    //for storage => SmoothDamp
    private Vector3 velocity = Vector3.zero;

    //The model of the vehicle.
    [SerializeField]
    private GameObject _vehicleModel;


    //jump value
    [SerializeField]
    private float _jumpValue;


    [SerializeField]
    private BikeAnimation _bikeAnimation;

    public ParticleSystem turnLeftParticles;
    public ParticleSystem turnRightParticles;

    public ParticleSystem frictionParticles;

    [SerializeField]
    private Camera _mainCamera;

    [SerializeField]
    private BikeAudio _bikeAudio;

    [Header("RagDoll Management")]
    private Quaternion startingRotation;
    private Vector3 lastSavedPosition;
    [SerializeField]
    private Collider ragdollCollider;
    private Vector3 oldCenterOfMass;
    [SerializeField]
    public bool isRagdoll = false;

    private void Awake()
    {
        _bikeInput = new BikeInput();

        _bikeInput.Player.Accelerate.started += i => OnAccelerate();
        _bikeInput.Player.Accelerate.canceled += i => OnAccelerateStop();
        _bikeInput.Player.Turn.performed += i => _turnValue = i.ReadValue<float>() ;
        _bikeInput.Player.Brake.performed += i => OnBrake();
        _bikeInput.Player.Brake.canceled += i => OnBrakeStop();
        _bikeInput.Player.Jump.performed += i => OnJump();
        _bikeInput.Player.Pause.performed += i => OnPause();
        _mainCamera = Camera.main;
    }
    private void Start()
    {
        //Store the starting Position of the camera Target
        cameraTargetPosition = cameraTarget.transform.localPosition;
        oldCenterOfMass = rigidBody.centerOfMass;
        rigidBody.centerOfMass = new Vector3(rigidBody.centerOfMass.x, rigidBody.centerOfMass.y + .5f, rigidBody.centerOfMass.z);

        //Initialising positions & rotations
        startingRotation = transform.rotation;
        lastSavedPosition = transform.position;
    }

    private void OnEnable()
    {
        _bikeInput.Player.Enable();
    }


    private void ManageBikeAudio()
    {
        ////GESTION DE L'AUDIO
        if (_grounded )
        {
            if (_currentSpeed > 0.2)
            {
                _bikeAudio.StartGroundedAudio();
            }
            else
            {
                _bikeAudio.StopGroundedAudio();
            }

            if (_bikeAudio.IsFastRunningClipPlaying())
            {
                _bikeAudio.SetFastRunningAudioSpeed(_currentSpeed);
            }
            if (_bikeAudio.IsSlowRunningClipPlaying())
            {
                _bikeAudio.SetSlowRunningAudioSpeed(_currentSpeed);
            }

            if (_currentSpeed <= .2)
            {
                _bikeAudio.StopAllClips();
            }
            else if (_currentSpeed > 0.2 && _currentSpeed < _maxSpeed / 4 && !_bikeAudio.IsSlowRunningClipPlaying())
            {
                _bikeAudio.PlaySlowRunningAudio();
                Debug.Log("Playin'");
            }
            else if (_currentSpeed >= _maxSpeed / 4 && _currentSpeed < (_maxSpeed / 4) + .5f)
            {
                if (!_bikeAudio.IsFastRunningClipPlaying() && _bikeAudio.IsSlowRunningClipPlaying())
                {
                    Debug.Log("//switch To Higher Gear");
                    _bikeAudio.SwitchToHigherGear();
                }
                else if (!_bikeAudio.IsSlowRunningClipPlaying() && _bikeAudio.IsFastRunningClipPlaying())
                {
                    Debug.Log("//switch To Lower Gear");
                    _bikeAudio.SwitchToLowerGear();
                }
            }

            if (_braking)
            {
                _bikeAudio.StartBrakingLoopAudio();
            }
            else
            {
                _bikeAudio.StopBrakingLoopAudio();
            }
        }
        else
        {
            _bikeAudio.StopGroundedAudio();
        }

        if(_currentSpeed >= _maxSpeed / 3)
        {
            _bikeAudio.StartWindAudio();
        }
        else
        {
            _bikeAudio.StopWindAudio();
        }
      
    }

    private void FixedUpdate()
    {

        //Mise à jour des variables currentSpeed & bool moving
        _currentSpeed = rigidBody.velocity.magnitude; 
        _moving = _currentSpeed == 0 ? false : true;

        //Sound Management
        ManageBikeAudio();

        if (!isRagdoll)
        {
            if (_moving)
            {
                TurnBike();
            }

            if (_currentSpeed > _maxSpeed / 3)
            {
                if (animeLines.activeInHierarchy == false)
                    animeLines.SetActive(true);
            }

            else
            {
                if (animeLines.activeInHierarchy == true)
                    StartCoroutine(FadeOutCanvas());
            }
            if (_grounded)
            {
                if (accelerating)
                {
                    if (_currentSpeed < _maxSpeed)
                    {

                        //Acceleration
                        //rigidBody.velocity = Vector3.Lerp(rigidBody.velocity, _vehicleModel.transform.forward * _maxSpeed, Time.fixedDeltaTime * _acceleration);
                        rigidBody.AddForce(_vehicleModel.transform.forward * _acceleration, ForceMode.Acceleration);

                        _currentAcceleration += _acceleration;
                    }
                }
                else
                {
                    //Stop acceleration
                    _currentAcceleration = 0;
                }

                //Braking
                if (_braking)
                {
                    if (_currentSpeed > 0.01 && _moving && _currentBraking < _brakingMax)
                    {
                        _currentBraking += _brakingStep;
                        rigidBody.AddForce(-rigidBody.velocity * _brakingStep, ForceMode.Force);
                    }

                }
                else
                {
                    _currentBraking = 0;
                }

                //Debug.Log("============= Current acceleration : " + _currentAcceleration);
                //Debug.Log("xxxxxxxxxxxxxxxxxxxx Current braking : " + _currentBraking);
            }

            if (_braking && _currentSpeed > 0)
            {
                ActivateFrictionParticles(true);


            }
            else
            {
                ActivateFrictionParticles(false);

            }
        }
       

        Debug.DrawRay(new Vector3(_vehicleModel.transform.position.x, _vehicleModel.transform.position.y + .5f, _vehicleModel.transform.position.z), -_vehicleModel.transform.forward, Color.red);
        Debug.DrawRay(_vehicleModel.transform.position, _vehicleModel.transform.forward, Color.blue);

        _mainCamera.fieldOfView = Mathf.Lerp(60, 75, _currentSpeed/_maxSpeed);
    }

    IEnumerator FadeOutCanvas()
	{
        animeLines.GetComponent<Animator>().SetTrigger("FadeOut");
        yield return new WaitForSeconds(1.5f);
        animeLines.SetActive(false);
    }

    private void Update()
    {
        CheckGround();


        if (!isRagdoll)
        {
            if (_grounded)
                _bikeAnimation.RotateWheelBones(_currentSpeed / 3);
            _bikeAnimation.RotateFrontBone(_turnValue);
        }
      
    }
    private void CheckGround()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(_groundRaycastTransformFront.position, -_groundRaycastTransformFront.up, out hit, 1f, groundMask))
        {
            Debug.DrawRay(_groundRaycastTransformFront.position, -_groundRaycastTransformFront.up * hit.distance, Color.yellow);
            _grounded = true;
            hitNormal = hit.normal;
            //transform.up -= (transform.up - hit.normal) * 0.1f;
            RaycastHit hitBack;
            if (!isRagdoll)
            {
                if (Physics.Raycast(_groundRaycastTransformBack.position, -_groundRaycastTransformBack.up, out hitBack, 1f, groundMask))
                {
                    transform.up -= (transform.up - (hit.normal + hitBack.normal)) * 0.1f;
                    Debug.DrawRay(_groundRaycastTransformBack.position, -_groundRaycastTransformBack.up * hit.distance, Color.yellow);
                }
            }
           
        }
        else
        {
            Debug.DrawRay(_groundRaycastTransformFront.position, -_groundRaycastTransformFront.up * hit.distance, Color.red);
            _grounded = false;
        }
    }

    void ActivateFrictionParticles(bool on)
	{

        if(on && !frictionParticles.isPlaying)
            frictionParticles.Play();
        else if(!on && frictionParticles.isPlaying)
            frictionParticles.Stop();

        //frictionParticles.emission.
    }

    void TurnBike()
    {


        //turn the bike
        _currentTurnSpeed = Mathf.Lerp(_maxTurnSpeed, _minTurnSpeed, _currentSpeed/_maxSpeed);
        if (_braking)
            _currentTurnSpeed *= 1.5f;
        _vehicleModel.transform.Rotate(0, _turnValue * Time.deltaTime * _currentTurnSpeed, 0);

        //float turnFinalValue = _turnValue * Time.deltaTime * _currentTurnSpeed;
        //Debug.Log("Turn value" + turnFinalValue);
        //velocity = rigidBody.velocity;

        if (_turnValue != 0)
        {
            //Move the cameraTarget
            cameraTarget.transform.localPosition = Vector3.SmoothDamp(cameraTarget.transform.localPosition,
                       new Vector3(cameraTarget.transform.localPosition.x + _turnValue * .2f, cameraTarget.transform.localPosition.y, cameraTarget.transform.localPosition.z),
                       ref velocity, 20f * Time.deltaTime);
        }
        else
        {
            cameraTarget.transform.localPosition = Vector3.SmoothDamp(cameraTarget.localPosition, cameraTargetPosition, ref velocity, 40f * Time.deltaTime);
        }

  //      if(_turnValue == 1)
		//{
  //          if(turnLeftParticles.isPlaying)
  //              turnLeftParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
  //          if(!turnRightParticles.isPlaying)
  //              turnRightParticles.Play();
		//}
  //      else if (_turnValue == -1)
  //      {
  //          if (!turnLeftParticles.isPlaying)
  //              turnLeftParticles.Play();
  //          if (turnRightParticles.isPlaying)
  //              turnRightParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
  //      }
		//else
		//{
  //          if (turnLeftParticles.isPlaying)
  //              turnLeftParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
  //          if (turnRightParticles.isPlaying)
  //              turnRightParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
  //      }
       
    }

    void TurnCamera()
    {
        cameraTransform.Rotate(0, cameraLook.x * Time.deltaTime * cameraRotationSpeed, 0);
    }

   

    private void OnAccelerate()
    {
        accelerating = true;
        //_bikeAudio.PlayFastRunningAudio();
    }

    private void OnAccelerateStop()
    {
        accelerating = false;
    }

    private void OnBrake()
    {
        _braking = true;

    }

    private void OnBrakeStop()
    {
        _braking = false;
    }

    private void OnPause()
    {
        GameManager.Instance.OnPause();
    }

    private void OnJump()
    {
        if (!isRagdoll)
        {
            if (_grounded)
            {
                rigidBody.AddForce(Vector3.up * _jumpValue);
            }
        }
      
    }

    public void ActivateRagdoll()
    {
        if (!isRagdoll)
        {
            isRagdoll = true;
            rigidBody.freezeRotation = false;
            rigidBody.centerOfMass = oldCenterOfMass;
            ragdollCollider.enabled =true;
            StartCoroutine(DeactivateRagDollTimer(3f));
        }
      
    }

    public IEnumerator DeactivateRagDollTimer(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        DeactivateRagdoll();
    }

    public void DeactivateRagdoll()
    {
        isRagdoll = false;
        ragdollCollider.enabled = false;
        rigidBody.centerOfMass = new Vector3(rigidBody.centerOfMass.x, rigidBody.centerOfMass.y + .5f, rigidBody.centerOfMass.z);
        transform.rotation = startingRotation;
        rigidBody.velocity = Vector3.zero;
        rigidBody.freezeRotation = true;
        transform.position = lastSavedPosition;

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isRagdoll)
        {
            if (collision.collider.CompareTag("Bumper"))
            {
                if (_currentSpeed >= _maxSpeed / 2)
                {
                    rigidBody.AddForceAtPosition(transform.forward * -30f, collision.transform.position, ForceMode.Impulse);
                    ActivateRagdoll();
                }
            }
            else if (collision.collider.CompareTag("Car"))
            {
                rigidBody.AddForceAtPosition(transform.forward * -30f, collision.transform.position, ForceMode.Impulse);
                ActivateRagdoll();
            }
        }
     
    }

    public void SavePosition(Vector3 position)
    {
        lastSavedPosition = position;
    }
}
