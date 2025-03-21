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
    private GameObject _boidPrefab;

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
            Objective tempObjective = _objectives[Random.Range(0, _objectives.Count)]; ;
            float distance = Vector3.Distance(GameManager._instance._player.transform.position, tempObjective.transform.position);
            while (distance < 80f || distance > 180f)
            {
                tempObjective = _objectives[Random.Range(0, _objectives.Count)]; 
                distance = Vector3.Distance(GameManager._instance._player.transform.position, tempObjective.transform.position);
            }
            _currentObjective = tempObjective;
            _currentObjective.gameObject.SetActive(true);
        }
    }

    public Objective SelectRandomObjective()
    {
        return _objectives[Random.Range(0, _objectives.Count)];
    }

    public void ValidateCurrentObjective()
    {
        ActivatePopUp(_currentObjective.transform.position);
        InstantiateBoidBicycle();
        _currentObjective.gameObject.SetActive(false);
        GameManager._instance.AddScore(_objectivePoints);
        _currentObjective = null;
    }

    private void ActivatePopUp(Vector3 position)
    {
        Vector3 spawnPosition = new Vector3(position.x, position.y + .5f, position.z);
        _objectivesPopUp.transform.position = spawnPosition;
        _objectivesPopUp.SetActive(true);
    }
    
    public void InstantiateBoidBicycle()
    {
        Instantiate(_boidPrefab, new Vector3(_currentObjective.transform.position.x, 0, _currentObjective.transform.position.z), Quaternion.identity, BoidManager._instance.transform);
    }

   
}
