using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoubelleCollision : MonoBehaviour
{
    public Rigidbody mainRigidbody;
    public Rigidbody capRigidbody;
    public HingeJoint joint;

	public GameObject garbagePrefab;
	bool used = false;

	public Transform spawnPoint;
	public PoubelleSpawn poubelleSpawn;

	private int poubelleScore = 20;


	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player") && !used)
		{
			used = true;

			mainRigidbody.isKinematic = false;
			capRigidbody.isKinematic = false;
			mainRigidbody.AddForce(other.transform.forward * 50f);
			GameObject go1 = InstantiateGarbage();
			GameObject go2 = InstantiateGarbage();
			GameObject go3 = InstantiateGarbage();
			go1.GetComponent<Rigidbody>().AddForce(other.transform.forward * 50f);
			go2.GetComponent<Rigidbody>().AddForce(other.transform.forward * 50f);
			go3.GetComponent<Rigidbody>().AddForce(other.transform.forward * 50f);

			GameManager.Instance.AddScore(poubelleScore);
			StartCoroutine(DeactivatePoubelle());
		}
	}

	GameObject InstantiateGarbage()
	{
		return GameObject.Instantiate(garbagePrefab, spawnPoint.position, Quaternion.identity);
	}

	public void ResetPoubelle()
	{
		used = false;
		mainRigidbody.isKinematic = true;
		capRigidbody.isKinematic = true;
		transform.rotation = Quaternion.identity;
	}

	public IEnumerator DeactivatePoubelle()
	{
		yield return new WaitForSeconds(10f);
		gameObject.SetActive(false);
		poubelleSpawn.used = false;
	}
}
