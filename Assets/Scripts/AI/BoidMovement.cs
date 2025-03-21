using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidMovement : MonoBehaviour
{
    [Header("Speed Values")]
    [SerializeField]
    private float _maxSpeed;
    [SerializeField] 
    private float _currentSpeed;
    [SerializeField]
    private float _accelerationDistance;
    [SerializeField]
    private float _decelerationDistance;
    [SerializeField]
    private float _smoothDampDuration;
    [SerializeField]
    private float _turningSpeed;
    [SerializeField]
    private float _lerpSpeed;
    [SerializeField]
    private float _turningDelay;
    [SerializeField]
    private float _distanceToTarget;

    [Header("Visual")]
    [SerializeField]
    private bool _obstacleTurning;
    [SerializeField]
    private Renderer _frameRenderer;
    [SerializeField]
    private BikeAnimation _bikeAnimation;


    [Header("Following Values")]
    [SerializeField]
    private Transform _playerTarget;
    [SerializeField]
    private BikeController _bikeController;
    [SerializeField]
    private float _stoppingDistance;

    [Header("Obstacle Detection")]
    [SerializeField]
    private ObstacleDetection _obstacleDetection;
    [SerializeField]
    private int _frameFrequency;

    [Header("Boids values")]
    [SerializeField]
    private float _boidsDetectionRange;



    [Header("Movement directions")]
    [SerializeField]
    private Vector3 _goToTargetDir;
    [SerializeField]
    private Vector3 _avoidObstacleDir;
    [SerializeField]
    private Vector3 _currentTargetDir;
    [SerializeField]
    private Vector3 _previousTargetDir;
    [SerializeField]
    private Vector3 _coherenceDir;
    [SerializeField]
    private Vector3 _separationDir;
    [SerializeField]
    private Vector3 _playerSeparationDir;
    [SerializeField]
    private Vector3 _playerForwardDir;

    [Header("Priorities")]
    [SerializeField]
    private float _obstaclePriority;
    [SerializeField]
    private float _chasingPriority;
    [SerializeField]
    private float _coherencePriority;
    [SerializeField]
    private float _separationPriority;
    [SerializeField]
    private float _playerSeparationPriority;
    [SerializeField]
    private float _playerForwardPriority;


    [Header("GroundCheck")]
    [SerializeField]
    private Transform _groundRaycastTransformFront;
    [SerializeField]
    private Transform _groundRaycastTransformBack;
    [SerializeField]
    private LayerMask _groundMask;
    private Vector3 hitNormal;

    [Header("DEBUG")]
    [SerializeField]
    private Transform _transformDirection;


    private void Awake()
    {
        _bikeController = GameManager._instance._player;
        _playerTarget = _bikeController._followTarget;
    }

    private void Start()
    {
        BoidManager._instance._boids.Add(gameObject);
        ChangeColor();
    }

    private void ChangeColor()
    {
        Color newColor = new Color((float)Random.Range(0, 255)/255, (float)Random.Range(0, 255)/255, (float)Random.Range(0, 255)/255);
        _frameRenderer.material.color = newColor;
    }

    public void StartTurning()
    {
        _obstacleTurning = true;
        StartCoroutine(StopTurning(_turningDelay));
    }

    public IEnumerator StopTurning(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _obstacleTurning = false;
    }

    private void CalculatePlayerDirection()
    {
        Vector3 direction = Vector3.zero;
        if (_distanceToTarget > _stoppingDistance)
            direction = (_playerTarget.position - transform.position).normalized;
        else
            direction = Vector3.zero;

        _goToTargetDir = Vector3.Lerp(_goToTargetDir, direction, Time.deltaTime * _lerpSpeed);
    }

    private void CheckGround()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(_groundRaycastTransformFront.position, -_groundRaycastTransformFront.up, out hit, 1f, _groundMask))
        {
            //Debug.DrawRay(_groundRaycastTransformFront.position, -_groundRaycastTransformFront.up * hit.distance, Color.yellow);
            hitNormal = hit.normal;
            RaycastHit hitBack;
            if (Physics.Raycast(_groundRaycastTransformBack.position, -_groundRaycastTransformBack.up, out hitBack, 1f, _groundMask))
            {
                //transform.up -= (transform.up - (hit.normal + hitBack.normal)) * 0.1f;
                //Debug.DrawRay(_groundRaycastTransformBack.position, -_groundRaycastTransformBack.up * hit.distance, Color.yellow);
            }

            transform.position = new Vector3(transform.position.x, hit.point.y , transform.position.z);

        }
    }

    private void CalculatePlayerSeparation()
    {
        Vector3 direction = Vector3.zero;
        if (Vector3.Distance(transform.position, _bikeController.transform.position) <= _boidsDetectionRange)
            direction = (transform.position - _bikeController.transform.position).normalized;
        else
            direction = Vector3.zero;

        _playerSeparationDir = Vector3.Lerp(_playerSeparationDir, direction, Time.deltaTime * _lerpSpeed);
    }

    private void CalculatePlayerForward()
    {
        _playerForwardDir = _bikeController.transform.forward;
    }

    private void RotateTowardsTarget(Vector3 targetDir)
    {
        if(targetDir != Vector3.zero)
        {
            Vector3 direction = targetDir.normalized;
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }

    }

    private void CalculateCurrentSpeed()
    {
         _distanceToTarget = Vector3.Distance(transform.position, _playerTarget.position);
        // Calculer la vitesse cible basée sur la distance
        float targetSpeed = CalculateTargetSpeed(_distanceToTarget);

        Vector3 currentVelocity = Vector3.zero;
        // Appliquer SmoothDamp à la vitesse
        _currentSpeed = Mathf.SmoothDamp(targetSpeed, targetSpeed, ref currentVelocity.x, _smoothDampDuration);
    }

    private float CalculateTargetSpeed(float distance)
    {
        if (distance > _accelerationDistance)
        {
            return _maxSpeed;
        }
        else if (distance < _decelerationDistance)
        {
            //return _maxSpeed * (distance / _decelerationDistance);
            return _bikeController.CurrentSpeed;
        }
        else
        {
            return _maxSpeed;
        }
    }

    private void FixedUpdate()
    {
        CalculateTargetDirection();
        
        //Debug.DrawRay(transform.position, _targetDir, Color.green, 1f);
        //Debug.DrawRay(transform.position, _separationDir * 3, Color.blue, 1f);
        RotateTowardsTarget(_currentTargetDir);

        transform.position += transform.forward * _currentSpeed * Time.deltaTime;

        CheckGround();
        ManageAnimation();
    }

    private void CalculateTargetDirection()
    {
        //if(Time.frameCount % _frameFrequency == 0)
        //{
        if (!_obstacleTurning)
            _avoidObstacleDir = (_obstacleDetection.DetectObstacles());
        //}

        CalculatePlayerDirection();
        CalculateCurrentSpeed();
        List<GameObject> closestBoids = BoidManager._instance.GetClosestBoids(gameObject, _boidsDetectionRange);
        CalculateSeparation(closestBoids);
        CalculatePlayerSeparation();
        DeleteYAxis();
        _previousTargetDir = _currentTargetDir;
        _currentTargetDir = Vector3.Lerp(_currentTargetDir,
                                    (_goToTargetDir * _chasingPriority
                                    + _avoidObstacleDir * _obstaclePriority
                                    + _separationDir * _separationPriority
                                    + _playerSeparationDir * _playerSeparationPriority
                                    + _playerForwardDir * _playerForwardPriority
                                    ).normalized
                                    , Time.deltaTime * _turningSpeed);
    }


    public void CalculateSeparation(List<GameObject> closestBoids)
    {
        Vector3 direction = Vector3.zero ;
        if(closestBoids.Count == 0)
        {
            _separationDir =  Vector3.Lerp(_separationDir, Vector3.zero, Time.deltaTime * _lerpSpeed);
            return;
        }
        foreach(GameObject g in closestBoids)
        {
            if(g.transform.position == transform.position)
            {
                //ignore thyself
                break;
            }

            direction+= (transform.position - g.transform.position);
            
        }
        _separationDir = Vector3.Lerp(_separationDir, direction.normalized, Time.deltaTime * _lerpSpeed);
    }

    private void DeleteYAxis()
    {
        _avoidObstacleDir = new Vector3(_avoidObstacleDir.x, 0, _avoidObstacleDir.z);
        _goToTargetDir = new Vector3(_goToTargetDir.x, 0, _goToTargetDir.z);
        _separationDir = new Vector3(_separationDir.x, 0, _separationDir.z);
        _playerSeparationDir = new Vector3(_playerSeparationDir.x, 0, _playerSeparationDir.z);
    }

    private void ManageAnimation()
    {
        _bikeAnimation.RotateWheelBones(_currentSpeed / 3);
        
        //_bikeAnimation.RotateFrontBone(turnValue);
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, _playerSeparationDir * _playerSeparationPriority, Color.red, .2f);
        Debug.DrawRay(transform.position, _coherenceDir * _coherencePriority, Color.red, .2f);
        Debug.DrawRay(transform.position, _separationDir * _separationPriority, Color.red, .2f);
        Debug.DrawRay(transform.position, _goToTargetDir * _chasingPriority, Color.red, .2f);
        Debug.DrawRay(transform.position, _currentTargetDir * 5f, Color.green, .2f);
    }
}
