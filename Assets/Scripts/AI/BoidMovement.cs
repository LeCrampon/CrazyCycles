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
    private float _turningDelay;
    [SerializeField]
    private float _distanceToTarget;

    [Header("Animation Bools")]
    [SerializeField]
    private bool _turning;

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
    private Vector3 _targetDir;
    [SerializeField]
    private Vector3 _coherenceDir;
    [SerializeField]
    private Vector3 _separationDir;

    [Header("Priorities")]
    [SerializeField]
    private float _obstaclePriority;
    [SerializeField]
    private float _chasingPriority;
    [SerializeField]
    private float _coherencePriority;
    [SerializeField]
    private float _separationPriority;

    [Header("DEBUG")]
    [SerializeField]
    private Transform _transformDirection;

    public void StartTurning()
    {
        _turning = true;
        StartCoroutine(StopTurning(_turningDelay));
    }

    public IEnumerator StopTurning(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _turning = false;
    }

    private void CalculatePlayerDirection()
    {
        _goToTargetDir = (_playerTarget.position - transform.position).normalized;
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
        _currentSpeed = Mathf.SmoothDamp(_currentSpeed, targetSpeed, ref currentVelocity.x, _smoothDampDuration);
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
        //if(Time.frameCount % _frameFrequency == 0)
        //{
            if(!_turning)
                _avoidObstacleDir = (_obstacleDetection.DetectObstacles());
        //}

        CalculatePlayerDirection();
        CalculateCurrentSpeed();
        List<GameObject> closestBoids = GetClosestBoids();
        CalculateSeparation(closestBoids);
        //_targetDir = (_goToTargetDir * _forceToMoveTowards - _avoidObstacleDir * _forceToAvoid).normalized;
        DeleteYAxis();
        _targetDir = Vector3.Lerp(_targetDir,
                                    (_goToTargetDir * _chasingPriority  
                                    + _avoidObstacleDir * _obstaclePriority
                                    + _separationDir * _separationPriority
                                    ).normalized
                                    , Time.deltaTime *_turningSpeed);
        
        Debug.DrawRay(transform.position, _targetDir, Color.green, 1f);
        //Debug.DrawRay(transform.position, _separationDir * 3, Color.blue, 1f);
        RotateTowardsTarget(_targetDir);

        if(_distanceToTarget > _stoppingDistance)
        {
            transform.position += transform.forward * _currentSpeed * Time.deltaTime;
        }
        /*Vector3.Lerp(transform.position, directionToGo,Time.deltaTime);*/
        //transform.position += transform.forward * _maxSpeed * Time.deltaTime; /*Vector3.Lerp(transform.position, directionToGo,Time.deltaTime);*/
    }

    public List<GameObject> GetClosestBoids()
    {
        List<GameObject> closestBoids = new List<GameObject>();
        Collider[] colliders = Physics.OverlapSphere(transform.position, _boidsDetectionRange);
        foreach(Collider c in colliders)
        {
            if (c.CompareTag("Boid"))
            {
                closestBoids.Add(c.gameObject);
            }
        }

        return closestBoids;
    }

    public void CalculateSeparation(List<GameObject> closestBoids)
    {
        if(closestBoids.Count == 0)
        {
            _separationDir =  Vector3.zero;
            return;
        }
        foreach(GameObject g in closestBoids)
        {
            if(g.transform.position == transform.position)
            {
                //ignore thyself
                break;
            }

            _separationDir += (transform.position - g.transform.position);
            _separationDir = _separationDir.normalized;
        }
    }

    private void DeleteYAxis()
    {
        _avoidObstacleDir = new Vector3(_avoidObstacleDir.x, 0, _avoidObstacleDir.z);
        _goToTargetDir = new Vector3(_goToTargetDir.x, 0, _goToTargetDir.z);
        _separationDir = new Vector3(_separationDir.x, 0, _separationDir.z);
    }
}
