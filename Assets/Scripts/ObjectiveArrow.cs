using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectiveArrow : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _text;
    [SerializeField]
    private Transform _lookAtContainer;   
    [SerializeField]
    private Transform _textLookAtContainer;

    public void PointToNextObjective()
    {
        if(ObjectiveManager._instance._currentObjective != null)
        {
            _lookAtContainer.LookAt(ObjectiveManager._instance._currentObjective.transform);
        }

    }

    public void DisplayText()
    {
        if (ObjectiveManager._instance._currentObjective != null)
        {
            _text.text = Mathf.RoundToInt(Vector3.Distance(transform.position, ObjectiveManager._instance._currentObjective.transform.position)) + " m";
            _textLookAtContainer.transform.LookAt(Camera.main.transform);
        }

    }

    // Update is called once per frame
    void Update()
    {
        PointToNextObjective();
        DisplayText();
    }
}
