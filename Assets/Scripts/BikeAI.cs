using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BikeAI : MonoBehaviour
{
    bool _isCaptured = false;
	
    private NavMeshAgent navAgent;

    [SerializeField]
    private Transform target;

	[SerializeField]
	private Transform _player;

	[SerializeField]
	private BikeAnimation _bikeAnimation;
	private void Awake()
	{
        navAgent = GetComponent<NavMeshAgent>();
	}
	// Start is called before the first frame update
	void Start()
    {

		//StartCoroutine(ChangeTargetCoroutine());

    }

	public IEnumerator ChangeTargetCoroutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(1f);
			target.transform.position = _player.position;
			navAgent.SetDestination(target.position);
		}
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.E))
		{
			_isCaptured = true;
		}
		if (_isCaptured)
		{
			navAgent.SetDestination(_player.position);
			//navAgent.speed = _player.GetComponent<BikeController>().CurrentSpeed;
			_bikeAnimation.RotateWheelBones(navAgent.velocity.magnitude / 3);
			_bikeAnimation.RotateFrontBone(Vector3.Angle(navAgent.velocity, transform.forward));

			//navAgent.speed = Mathf.Lerp(0, 100, Vector3.Distance(transform.position, navAgent.destination));
		}
	}

	private void SetAngularVelocity()
	{

	}
}
