using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CameraSmoothFollow : MonoBehaviour
{

    public float smoothPosFactor = 1f;
    public float smoothRotFactor = 1.5f;

    [SerializeField]
    private float _heightOffset;
    [SerializeField]
    private float _distanceOffset;
    [SerializeField]
    private Transform _target;


    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        transform.position = _target.position - _target.forward * _distanceOffset;
        transform.position = new Vector3(transform.position.x, transform.position.y + _heightOffset, transform.position.z);
        transform.rotation = Quaternion.LookRotation(_target.position - transform.position);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 newPos = _target.position - _target.forward * _distanceOffset;
        newPos = new Vector3(newPos.x, newPos.y + _heightOffset, newPos.z);
        transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, smoothPosFactor );
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_target.position - transform.position), smoothRotFactor * Time.fixedDeltaTime);
        
    }
}
