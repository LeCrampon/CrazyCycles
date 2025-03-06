using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!GameManager.Instance._player.isRagdoll && other.CompareTag("Player"))
        {
            GameManager.Instance._player.SavePosition(transform.position);
        }
    }
}
