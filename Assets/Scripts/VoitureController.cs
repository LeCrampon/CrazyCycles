using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoitureController : MonoBehaviour
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

    [Header("WayPoints")]
    public Waypoint nextWayPoint;
    public Waypoint previousWayPoint;

    [Header("Distance Ratios")]
    public float distanceFromPrevToNext;
    public float delta;
    public float ratio;

    [Header("Moving Values")]
    public float stoppingDistance = 2f;
    public float speed = 5f;
    public float rotationSpeed = 5f;
    //Pour checker la différence de direction entre la voiture et le waypoint
    public float dotDifferentialValue = 0.1f;

    [Header("Booleans")]
    [SerializeField]
    private bool _nextWaypointFound = false;
    public bool nextWayPointisRedlight = false;
    public bool nextWayPointisCurve = false;
    public bool stop = false;
    public bool isAtRedlight = false;

    [Header("Spline Helper")]
    public SimpleSpline splineHelper;
    public LayerMask layerMaskWaypoint;

    [Header("Collision")]
    [SerializeField]
    private LayerMask _carCollisionLayerMask;
    [SerializeField]
    private Transform _rayCastPivot;

    private void Awake()
    {
        splineHelper = GetComponent<SimpleSpline>();
        currentState = CarState.SearchingForWaypoint;
        previousState = CarState.SearchingForWaypoint;


    }

    // Start is called before the first frame update
    void Start()
    {
        previousWayPoint = GameObject.FindGameObjectWithTag("Waypoint").GetComponent<Waypoint>();
    }

    void FixedUpdate()
    {
        if (GameManager._instance.gameStarted || GameManager._instance.inMenu)
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
                    MoveTowardsNextWayPointStraight();
                    CheckIfCarInFront();
                    break;
                case CarState.MovingTowardsNextWaypointInCurve:
                    MoveTowardsNextWayPointInCurve();
                    CheckIfCarInFront();
                    break;
                case CarState.Stopped:
                    CheckIfCarCanStartAgain();
                    break;
            }

            Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, 0) * transform.forward, Color.yellow);
        }

    }


	#region FONCTIONS DE STATES

	//Trouver le prochain point (angle pour décaler si non trouvé)
	private void FindNextWayPoint(float startingAngle)
    {
        //Tirer des rayons sur angle de -90 -> 90
        for (int i = -18; i < 18; i++)
        {
            float angle = i * 5;
            RaycastHit hit;

            if (Physics.Raycast(transform.position, Quaternion.Euler(0, startingAngle + angle, 0) * transform.forward, out hit, 64f, layerMaskWaypoint))
            {
                //Si frappe un waypoint
                if (hit.collider.CompareTag("Waypoint") && hit.collider.GetComponent<Waypoint>() != null)
                {
                    //Si la direction du waypoint ne diffère pas drastiquement de la direction de la voiture
                    if (CheckWaypointDirection(hit.collider.GetComponent<Waypoint>().direction))
                    {
                        //récuperation du waypoint
                        Waypoint tempWayPoint = hit.collider.GetComponent<Waypoint>();

                        //stockage du précédent waypoint
                        if (nextWayPoint != null && nextWayPoint != tempWayPoint)
                            previousWayPoint = nextWayPoint;

                        //Récupération du waypoint validée
                        nextWayPoint = tempWayPoint;

                        //Calcul de distance entre les deux waypoints (précedent et suivant)
                        distanceFromPrevToNext = Vector3.Distance(nextWayPoint.transform.position, previousWayPoint.transform.position);

                        // Waypoint trouvé
                        _nextWaypointFound = true;

                        //Vérification: est-ce que le prochain waypoint est une courbe ou une redlight
                        nextWayPointisRedlight = nextWayPoint.GetComponent<Waypoint>().isRedLight;
                        nextWayPointisCurve = nextWayPoint.GetComponent<Waypoint>().isCurve;


                        Debug.DrawLine(transform.position, hit.transform.position, Color.red, 2f);

                        //Dans tous les cas, on y va, tout droit
                        // On change d'état.
                        SwitchState(CarState.MovingTowardsNextWaypointStraight);
                    }

                }

            }

            Debug.DrawRay(transform.position, Quaternion.Euler(0, angle, 0) * transform.forward, Color.yellow);

        }
    }


    private void FindNextWayPointCurve()
    {
        //Récupération du prochain waypoint
        Waypoint tempWaypoint = nextWayPoint;

        //Ajustement des variables du spline helper, récupérées à partir de l'ancien
        SetSplineHelper(tempWaypoint);

        //Sauvegarde du précédent waypoint
        previousWayPoint = nextWayPoint;

        //Assignation du prochain waypoint: il s'agit du prochain point de courbe du waypoint de type "curve".
        nextWayPoint = tempWaypoint.nextCurveWaypoint;

        //Calcul de la distance entre les deux waypoints
        distanceFromPrevToNext = Vector3.Distance(nextWayPoint.transform.position, previousWayPoint.transform.position);

        _nextWaypointFound = true;

        //Vérification: est-ce que le prochain waypoint est une courbe ou une redlight ?
        nextWayPointisRedlight = nextWayPoint.GetComponent<Waypoint>().isRedLight;
        nextWayPointisCurve = nextWayPoint.GetComponent<Waypoint>().isCurve;

        //On passe au state: bouger dans une courbe
        SwitchState(CarState.MovingTowardsNextWaypointInCurve);
    }

    private void MoveTowardsNextWayPointStraight()
    {
        //Si on a pas atteint le prochain waypoint : ajuster rotation et avancer vers next waypoint
        if (Vector3.Distance(transform.position, nextWayPoint.transform.position) > stoppingDistance)
        {
            ManageRotation();
            transform.position = Vector3.MoveTowards(transform.position, 
                                        new Vector3(nextWayPoint.transform.position.x, transform.position.y, nextWayPoint.transform.position.z), 
                                        speed * Time.fixedDeltaTime);
        }
        else
        {
            //Si on a atteint le prochain waypoint: On recherche le bon state
            _nextWaypointFound = false;

            //Selon si ce waypoint est une courbe ou une redlight
            if (nextWayPoint.isCurve)
                SwitchState(CarState.SearchingForWaypointInCurve);
            else if (nextWayPoint.isRedLight)
                SwitchState(CarState.SearchingForWaypointRedLight);
            else
                SwitchState(CarState.SearchingForWaypoint);
        }
    }

    private void MoveTowardsNextWayPointInCurve()
	{
        // Si on a pas atteint le prochain waypoint
        if (Vector3.Distance(transform.position, nextWayPoint.transform.position) > stoppingDistance)
        {
            //Calcul du delta de distance
            //float distancefromPrevToNextDelta = Vector3.Distance(nextWayPoint.transform.position, transform.position);

            //Utilisation du splineHelper pour calculer le mouvement et la rotation
            Vector3 lerpValue = splineHelper.CalculateLerp();
            transform.position = new Vector3(lerpValue.x, transform.position.y, lerpValue.z);
            transform.rotation = splineHelper.CalculateRotationLerp();
        }
        else
        {
            //Si on a atteint le prochain waypoint
            _nextWaypointFound = false;
            //On reset le calcul de spline
            splineHelper.ResetInterpolationFactor();

            //On change de state selon si le waypoint est une courbe ou une redlight, et on passe à la recherche du prochain waypoint
            if (nextWayPointisCurve)
                SwitchState(CarState.SearchingForWaypointInCurve);
            else if (nextWayPointisRedlight)
                SwitchState(CarState.SearchingForWaypointRedLight);
            else
                SwitchState(CarState.SearchingForWaypoint);

        }
    }

	

	private void FindNextWayPointRedlight()
	{
		if (nextWayPoint.isOn)
		{
            //On sauvegarde le précédent waypoint
            previousWayPoint = nextWayPoint;

            //Asssignation temporaire de waypoint
            Waypoint tempWaypoint = nextWayPoint;

            //On choisit un prochain waypoint au hasard parmi les choix du feu rouge
            int redlightChoiceIndex = UnityEngine.Random.Range(0, tempWaypoint.redLightChoices.Count);
            nextWayPoint = tempWaypoint.redLightChoices[redlightChoiceIndex];

            //On set le spline helper selon les valeurs du waypoint
            SetSplineHelperRedLight(tempWaypoint, redlightChoiceIndex);

            //On calcule la distance entre les deux waypoints
            distanceFromPrevToNext = Vector3.Distance(nextWayPoint.transform.position, previousWayPoint.transform.position);

            _nextWaypointFound = true;

            //On regarde si le prochain waypoint est une curve ou pas.
            nextWayPointisRedlight = nextWayPoint.GetComponent<Waypoint>().isRedLight;
            nextWayPointisCurve = nextWayPoint.GetComponent<Waypoint>().isCurve;

            //Dans tous les cas, on change d'état et on y va en suivant une courbe.
            SwitchState(CarState.MovingTowardsNextWaypointInCurve);
        }
    }

	



	private void CheckIfCarInFront()
	{
        //Vérifie si il y a une voiture devant
        RaycastHit hit;
        if (Physics.Raycast(_rayCastPivot.position, _rayCastPivot.forward, out hit, 3f, _carCollisionLayerMask))
        {
            //Debug.Log("COLLIDING WITH : " + hit.collider.tag);
            if (hit.collider.CompareTag("Car") && Vector3.Dot(transform.forward, hit.collider.transform.forward) >= .3f)
            {
                //Dans ce cas, on change de state
                SwitchState(CarState.Stopped);
                Debug.DrawLine(_rayCastPivot.position, hit.transform.position, Color.blue, 2f);
            }   
        }
    }

    private void CheckIfCarCanStartAgain()
	{
        //Vérifie si il y a une voiture devant
        RaycastHit hit;
        if (Physics.Raycast(_rayCastPivot.position, _rayCastPivot.forward, out hit, 8f, _carCollisionLayerMask))
        {
            if (!hit.collider.CompareTag("Car"))
            {
                //Dans ce cas, on change de state
                SwitchStateFromStop(previousState);
                Debug.DrawLine(_rayCastPivot.position, hit.transform.position, Color.blue, 2f);
            }

            //Debug.Log("I HIT SOMETHING!  SHOULD STOP");
        }
        else
		{
            SwitchStateFromStop(previousState);
            //Debug.Log("THERE IS NO COLLIDER, LET'S GO");
        }
    }


	#endregion

	// Set a new current state, and save the previous state
	void SwitchState(CarState newState)
    {
        if(newState != currentState)
            previousState = currentState;
        currentState = newState;
    }

    //On ne veut pas sauvegarder la valeur stop
    void SwitchStateFromStop(CarState newState)
    {
        currentState = newState;
    }

    bool CheckWaypointDirection(Vector3 waypointDirection)
    {
        return Vector3.Dot(transform.forward, waypointDirection) > dotDifferentialValue;
    }

    //Gestion de la rotation
    void ManageRotation()
    {
        Vector3 targetDirection = nextWayPoint.direction;
        float distancefromPrevToNextDelta = Vector3.Distance(nextWayPoint.transform.position, transform.position);
        delta = distancefromPrevToNextDelta;
        ratio = 1 - distancefromPrevToNextDelta / distanceFromPrevToNext;

        Debug.DrawRay(transform.position, targetDirection * 10f);
        Quaternion toRotation = Quaternion.FromToRotation(transform.forward, nextWayPoint.direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation * toRotation, 1 - distancefromPrevToNextDelta / distanceFromPrevToNext);


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
}
