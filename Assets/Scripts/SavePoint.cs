using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!GameManager._instance._player.isRagdoll && other.CompareTag("Player"))
        {
            GameManager._instance._player.SavePosition(transform.position);
        }
    }
}
