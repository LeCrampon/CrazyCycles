using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BikeMovement : MonoBehaviour
{
    public Rigidbody rigidBody;
    public LayerMask groundMask;
    private BikeInput _bikeInput;
    private float _turnValue;
    private bool _braking = false;
    private bool _accelerating = false;

    [Header("Speed Values")]
    public float _acceleration = 2f;
    public float maxSpeed = 15f;
    public float currentSpeed = 0;
    
    [SerializeField]
    private Camera _mainCamera;

    [SerializeField]
    private GameObject _vehicleModel;

    private void Awake()
    {
        _bikeInput = new BikeInput();

        _bikeInput.Player.Accelerate.started += i => OnAccelerate();
        _bikeInput.Player.Accelerate.canceled += i => OnAccelerateStop();
        _bikeInput.Player.Turn.performed += i => _turnValue = i.ReadValue<float>();
        _bikeInput.Player.Brake.performed += i => OnBrake();
        _bikeInput.Player.Brake.canceled += i => OnBrakeStop();
        _bikeInput.Player.Jump.performed += i => OnJump();
        _mainCamera = Camera.main;
    }
    private void Start()
    {
    }

    private void OnEnable()
    {
        _bikeInput.Player.Enable();
    }


    private void FixedUpdate()
    {
        if(_accelerating && !_braking)
		{
            rigidBody.velocity = Vector3.Lerp(rigidBody.velocity, maxSpeed * _vehicleModel.transform.forward, Time.fixedDeltaTime * _acceleration);

		}
       
    }

  

    private void Update()
    {
       
    }

    private void OnAccelerate()
    {
        _accelerating = true;
    }

    private void OnAccelerateStop()
    {
        _accelerating = false;
    }

    private void OnBrake()
    {
        _braking = true;
    }

    private void OnBrakeStop()
    {
        _braking = false;
    }

    private void OnJump()
    {
        Debug.Log("Called");
    }
}
