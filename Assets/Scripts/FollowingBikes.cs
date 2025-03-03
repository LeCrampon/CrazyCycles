using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingBikes : MonoBehaviour
{

    public float smoothPosFactor = 1f;
    public float smoothRotFactor = 1.5f;

    public GameObject _target;

    private Vector3 velocity = Vector3.zero;


    // Update is called once per frame
    void Update()
    {
        transform.position = _target.transform.position - _target.transform.forward * 3;
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        transform.rotation = Quaternion.LookRotation(_target.transform.position - transform.position);
    }

    void FixedUpdate()
    {
        Vector3 newPos = _target.transform.position - _target.transform.forward * 3;
        newPos = new Vector3(newPos.x, newPos.y, newPos.z);
        transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, smoothPosFactor);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_target.transform.position - transform.position), smoothRotFactor * Time.deltaTime);

    }
}
