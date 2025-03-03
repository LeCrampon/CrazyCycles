using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSpline : MonoBehaviour
{
    float interpolationFactor = 0;
    public Waypoint firstWayPoint;
    public Transform middlePoint;
    public Waypoint lastWayPoint;
    float interpolationSpeed = .2f;


	public Vector3 CalculateLerp()
	{
        interpolationFactor = (interpolationFactor + Time.deltaTime * interpolationSpeed) % 1f;
        Vector3 firstLerp = Vector3.Lerp(firstWayPoint.transform.position, middlePoint.position, interpolationFactor);
        Vector3 secondLerp = Vector3.Lerp(middlePoint.position, lastWayPoint.transform.position, interpolationFactor);

        return Vector3.Lerp(firstLerp, secondLerp, interpolationFactor);
	}

	public void ResetInterpolationFactor()
	{
        interpolationFactor = 0f;
	}

    public Quaternion CalculateRotationLerp()
	{
        interpolationFactor = (interpolationFactor + Time.deltaTime * interpolationSpeed) % 1f;
        Quaternion firstLerp = Quaternion.Lerp(firstWayPoint.transform.rotation, middlePoint.rotation, interpolationFactor);
        Quaternion secondLerp = Quaternion.Lerp(middlePoint.rotation, lastWayPoint.transform.rotation, interpolationFactor);

        return Quaternion.Lerp(firstWayPoint.transform.rotation, lastWayPoint.transform.rotation, interpolationFactor);
    }

}
