using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BikeController playerBike = other.transform.parent.parent.GetComponent<BikeController>();
            if (playerBike.isRagdoll)
            {
                playerBike.SavePosition(transform.position);
            }
        }
    }
}
