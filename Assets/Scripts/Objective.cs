using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{
    public CapsuleCollider _collider;
    public bool _validated = false;

    private void Awake()
    {
        _collider = GetComponent<CapsuleCollider>();
    }

    private void Start()
    {
        StartCoroutine(Initialisation());
    }

    private void OnEnable()
    {
        _validated = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ValidateObjective();
        }
    }

    public void ValidateObjective() 
    {
        _validated = true;
        //rajouter des points, reinitialiser timer, tout ça
        ObjectiveManager._instance.ValidateCurrentObjective();
        ObjectiveManager._instance.ChooseNextObjective();
    }

    public IEnumerator Initialisation()
    {
        yield return new WaitForSeconds(.2f);
        ObjectiveManager._instance._objectives.Add(this);
        gameObject.SetActive(false);
    }

}
