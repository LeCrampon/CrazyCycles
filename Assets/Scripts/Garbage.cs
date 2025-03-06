using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Garbage : MonoBehaviour
{
    [SerializeField]
    private float lifetime;

    private void OnEnable()
    {
        StartCoroutine(DeactivateGarbage());
    }


    private IEnumerator DeactivateGarbage()
    {
        yield return new WaitForSeconds(lifetime);
        ResetGarbage();
    }

    private void ResetGarbage()
    {
        gameObject.SetActive(false);
        transform.rotation = Quaternion.identity;
    }
}
