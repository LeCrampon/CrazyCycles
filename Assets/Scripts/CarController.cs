using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CarController : MonoBehaviour
{
    public enum CarState
	{
        SearchingForWaypoint,
        SearchingForWaypointRedLight,
        SearchingForWaypointInCurve,
        MovingTowardsNextWaypointInCurve,
        MovingTowardsNextWaypointStraight,
        Stopped

	}


    [Header("States")]
    public CarState currentState;
    public CarState previousState;


    public GameObject[] waypoints;
    public Waypoint nextWayPoint;
    public Transform previousWayPoint;
    public float distanceFromPrevToNext;
    public float delta;
    public float ratio;

    [SerializeField]
    private bool _nextWaypointFound= false;
    public float stoppingDistance = 2f;
    public float speed = 5f;
    public float rotationSpeed = 5f;

    public Transform raycastPivot;


    bool nextWayPointisRedlight = false;
    bool nextWayPointisCurve = false;
    bool inCurve = false;

    public float dotDifferentialValue = 0.1f;

    bool stop = false;
    bool IsAtRedlight = false;

    public SimpleSpline splineHelper;

    public LayerMask layerMaskWaypoint;

	private void Awake()
	{
        splineHelper = GetComponent<SimpleSpline>();
	}

	// Start is called before the first frame update
	void Start()
    {
        waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        previousWayPoint = transform;
        currentState = CarState.SearchingForWaypoint;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		if (GameManager.Instance.gameStarted)
		{
            CheckIfCarInFront();

            switch (currentState)
			{
                case CarState.SearchingForWaypoint:
                    FindNextWayPoint(0);
                    break;
                case CarState.SearchingForWaypointInCurve:
                    FindNextWayPointCurve();
                    break;
                case CarState.SearchingForWaypointRedLight:
                    FindNextWayPointRedlight();
                    break;
                case CarState.MovingTowardsNextWaypointStraight:
                    MoveTowardsNextWayPoint();
                    break;
                case CarState.MovingTowardsNextWaypointInCurve:
                    MoveTowardsNextWayPointInCurve();
                    break;
                case CarState.Stopped:

                    break;
            }
        

            Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, 0) * transform.forward, Color.yellow);
        }
       
    }

    void MoveTowardsNextWayPoint()
	{
        if(nextWayPoint == null)
		{
            //ICI!
            return;
		}

		if (!stop)
		{
            if (Vector3.Distance(transform.position, nextWayPoint.transform.position) > stoppingDistance)
            {
                ManageRotation();
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(nextWayPoint.transform.position.x, transform.position.y, nextWayPoint.transform.position.z), speed * Time.deltaTime);
            }
            else
            {
                _nextWaypointFound = false;

                if (nextWayPoint.isCurve)
                    SwitchState(CarState.SearchingForWaypointInCurve);
                else if (nextWayPoint.isRedLight)
                    SwitchState(CarState.SearchingForWaypointRedLight);
                else
                    SwitchState(CarState.SearchingForWaypoint);
            }
        }

    }

    void MoveTowardsNextWayPointInCurve()
    {
        if (!stop)
        {
            if (Vector3.Distance(transform.position, nextWayPoint.transform.position) > stoppingDistance)
            {
                //ManageRotation();
                //transform.position = Vector3.MoveTowards(transform.position, new Vector3(nextWayPoint.position.x, transform.position.y, nextWayPoint.position.z), speed * Time.deltaTime);
                float distancefromPrevToNextDelta = Vector3.Distance(nextWayPoint.transform.position, transform.position);

                Vector3 lerpValue = splineHelper.CalculateLerp();
                transform.position = new Vector3(lerpValue.x, transform.position.y, lerpValue.z);
                transform.rotation = splineHelper.CalculateRotationLerp();
            }
            else
            {
                _nextWaypointFound = false;
                splineHelper.ResetInterpolationFactor();
                inCurve = false;

                if(nextWayPointisCurve)
                    SwitchState(CarState.SearchingForWaypointInCurve);
                else if(nextWayPointisRedlight)
                    SwitchState(CarState.SearchingForWaypointRedLight);
                else
                    SwitchState(CarState.SearchingForWaypoint);

            }
        }

    }

    void ManageRotation()
	{
        Waypoint nextWaypointWP = nextWayPoint.GetComponent<Waypoint>();
        Vector3 targetDirection = nextWaypointWP.direction;
        //Vector3 targetDirection = (new Vector3(nextWayPoint.position.x, transform.position.y, nextWayPoint.position.z) - transform.position).normalized ;
        float distancefromPrevToNextDelta = Vector3.Distance(nextWayPoint.transform.position, transform.position);
        delta = distancefromPrevToNextDelta;
        ratio = 1 - distancefromPrevToNextDelta / distanceFromPrevToNext;

        Debug.DrawRay(transform.position, targetDirection * 10f);
        Quaternion toRotation = Quaternion.FromToRotation(transform.forward, nextWaypointWP.direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation * toRotation, 1 - distancefromPrevToNextDelta/distanceFromPrevToNext);

        //transform.rotation = toRotation;

    }


    void CheckIfCarInFront()
	{
        RaycastHit hit;
        if (Physics.Raycast(transform.position,  transform.forward, out hit, 3f))
        {
            Debug.DrawRay(transform.position, transform.forward);
            if (hit.collider.CompareTag("Car")  )
            {
                if(Vector3.Dot(transform.forward, hit.collider.transform.forward) >= .3f)
                {
                    stop = true;
                    SwitchState(CarState.Stopped);
                    Debug.DrawLine(transform.position, hit.transform.position, Color.blue, 2f);
                }

                Debug.Log("CAR IN FRONT, SHOULD BE STOPPING");
            }
			else
			{
                if (!IsAtRedlight)
				{
                    stop = false;
                    SwitchState(previousState);
                }

            }
		}
        else
        {
            if (!IsAtRedlight && stop)
            {
                stop = false;
                SwitchState(previousState);
            }
        }
    }

    bool CheckWaypointDirection(Vector3 waypointDirection)
	{
        return Vector3.Dot(transform.forward, waypointDirection) > dotDifferentialValue;
    }


    void FindNextWayPoint(float startAngle)
	{
        for (int i = -18; i < 18; i++)
        {
            float angle = i * 5 ;
            RaycastHit hit;

            if (Physics.Raycast(transform.position,  Quaternion.Euler(0, startAngle + angle, 0) * transform.forward, out hit, 64f, layerMaskWaypoint))
            {
                if (hit.collider.CompareTag("Waypoint") && hit.collider.GetComponent<Waypoint>() != null)
                {
                    if(CheckWaypointDirection(hit.collider.GetComponent<Waypoint>().direction))
					{
                        Waypoint tempWayPoint = hit.collider.GetComponent<Waypoint>();
                        if (nextWayPoint != null && nextWayPoint != tempWayPoint)
                            previousWayPoint = nextWayPoint.transform;
                        nextWayPoint = tempWayPoint;
                        
                        distanceFromPrevToNext = Vector3.Distance(nextWayPoint.transform.position, previousWayPoint.position); 
                        _nextWaypointFound = true;
                        nextWayPointisRedlight = nextWayPoint.GetComponent<Waypoint>().isRedLight;
                        nextWayPointisCurve = nextWayPoint.GetComponent<Waypoint>().isCurve;
                        Debug.DrawLine(transform.position, hit.transform.position, Color.red,2f);

                        SwitchState(CarState.MovingTowardsNextWaypointStraight);
                    }
                   
                }

            }

            Debug.DrawRay(transform.position, Quaternion.Euler(0, angle, 0) * transform.forward, Color.yellow);

        }
    }

    void FindNextWayPointRedlight()
	{
        previousWayPoint = nextWayPoint.transform;
        Waypoint tempWaypoint = nextWayPoint.GetComponent<Waypoint>();

        //SetSplineHelper(tempWaypoint);
        int redlightChoiceIndex = Random.Range(0, tempWaypoint.redLightChoices.Count);
        nextWayPoint = tempWaypoint.redLightChoices[redlightChoiceIndex];
        SetSplineHelperRedLight(tempWaypoint, redlightChoiceIndex);
        distanceFromPrevToNext = Vector3.Distance(nextWayPoint.transform.position, previousWayPoint.position);

        _nextWaypointFound = true;
        nextWayPointisRedlight = nextWayPoint.GetComponent<Waypoint>().isRedLight;
        nextWayPointisCurve = nextWayPoint.GetComponent<Waypoint>().isCurve;

        inCurve = true;
        SwitchState(CarState.MovingTowardsNextWaypointInCurve);
    }

    void FindNextWayPointCurve()
	{
        
        Waypoint tempWaypoint = nextWayPoint;

        SetSplineHelper(tempWaypoint);
        previousWayPoint = nextWayPoint.transform;
        if(tempWaypoint.nextCurveWaypoint == null)
		{
            Debug.Log("NEXTWAYPOINT IS NULL " + tempWaypoint.gameObject.name);
            return;
		}
        nextWayPoint = tempWaypoint.nextCurveWaypoint;
        distanceFromPrevToNext = Vector3.Distance(nextWayPoint.transform.position, previousWayPoint.position);
        

        _nextWaypointFound = true;
        nextWayPointisRedlight = nextWayPoint.GetComponent<Waypoint>().isRedLight;
        nextWayPointisCurve = nextWayPoint.GetComponent<Waypoint>().isCurve;

        inCurve = true;

        SwitchState(CarState.MovingTowardsNextWaypointInCurve);
    }


    void SetSplineHelper(Waypoint wp)
	{
        splineHelper.firstWayPoint = wp;
        splineHelper.middlePoint = wp.curveHandle;
        splineHelper.lastWayPoint = wp.nextCurveWaypoint;
    }

    void SetSplineHelperRedLight(Waypoint wp, int index)
    {
        splineHelper.firstWayPoint = wp;
        splineHelper.middlePoint = wp.redLightHandle; 
        splineHelper.lastWayPoint = wp.redLightChoices[index];
    }

    void StopCar()
	{


	}

    void SwitchState(CarState newState)
	{
        previousState = currentState;
        currentState = newState;
	}
}
