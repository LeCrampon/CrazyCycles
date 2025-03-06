using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager _instance;
    public List<Objective> _objectives;
    public Objective _currentObjective;
    public int _objectivePoints = 250;

    [SerializeField]
    private GameObject _objectivesPopUp;


    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }

        _objectives = new List<Objective>();
    }


    public void ChooseNextObjective()
    {
        if(_objectives.Count != 0 && _currentObjective == null)
        {
            _currentObjective = _objectives[Random.Range(0, _objectives.Count)];
            _currentObjective.gameObject.SetActive(true);
        }
    }

    public void ValidateCurrentObjective()
    {
        ActivatePopUp(_currentObjective.transform.position);

        _currentObjective.gameObject.SetActive(false);
        GameManager.Instance.AddScore(_objectivePoints);
        _currentObjective = null;
    }

    private void ActivatePopUp(Vector3 position)
    {
        Vector3 spawnPosition = new Vector3(position.x, position.y + .5f, position.z);
        _objectivesPopUp.transform.position = spawnPosition;
        _objectivesPopUp.SetActive(true);
    }
    

   
}
