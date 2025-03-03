using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Waypoint : MonoBehaviour
{
	public Vector3 direction;

	[Header("Curve")]
	public bool isCurve = false;
	public Waypoint nextCurveWaypoint;
	public Transform curveHandle;

	[Header("Red Light")]
	public bool isRedLight = false;
	public List<Waypoint> redLightChoices;
	public Transform redLightHandle;
	public bool isOn;
	public bool isFirstHalf;
	public Light pointLight;

	private void Awake()
	{

		direction = transform.forward;
		pointLight = GetComponent<Light>();
	}


	private void Update()
	{
		if (isRedLight)
		{
			if (isFirstHalf)
				isOn = RedLightManager.Instance.firstHalfOn;
			else
				isOn = RedLightManager.Instance.secondHalfOn;

			if (isOn)
			{
				pointLight.color = Color.green;
			}
			else
			{
				pointLight.color = Color.red;
			}
		}
	}

}
