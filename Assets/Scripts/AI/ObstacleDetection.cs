using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDetection : MonoBehaviour
{
    [SerializeField]
    private LayerMask _layerMask;
    [SerializeField]
    private float _maxAngle;
    [SerializeField]
    private float _rayAngle;
    [SerializeField]
    private float _rayLength;
    [SerializeField]
    private BoidMovement _boid;

    public Vector3 DetectObstacles()
    {
        RaycastHit hit;
        // On cast devant. Si pas de hit, on return direct
        if(!Physics.Raycast(transform.position, transform.forward, out hit, _rayLength, _layerMask))
        {
            return Vector3.zero;
        }

        //sinon, on teste en éventail, et dans ce cas on renvoit la première direction où on ne tape pas.
        for(int i = 1; i < Mathf.RoundToInt(_maxAngle/_rayAngle)/2; i++)
        {
            Vector3 direction = Quaternion.AngleAxis(_rayAngle * i, Vector3.up) * transform.forward;
            Vector3 reverseDirection = Quaternion.AngleAxis(_rayAngle * -i, Vector3.up) * transform.forward;

            Debug.DrawRay(transform.position, direction * _rayLength, Color.red, .2f);
            Debug.DrawRay(transform.position, reverseDirection * _rayLength, Color.red, .2f);

            //positive direction
            if (!Physics.Raycast(transform.position, direction, out hit, _rayLength, _layerMask))
            {
                _boid.StartTurning();
                return direction;
            }
            //negative direction;
            if (!Physics.Raycast(transform.position, reverseDirection, out hit, _rayLength, _layerMask))
            {
                _boid.StartTurning();
                return reverseDirection;
            }
        }
        return Vector3.zero;
    }


}
