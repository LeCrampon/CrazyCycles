using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveArrow : MonoBehaviour
{
    public void PointToNextObjective()
    {
        if(ObjectiveManager._instance._currentObjective != null)
        {
            transform.LookAt(ObjectiveManager._instance._currentObjective.transform);
        }

    }

    // Update is called once per frame
    void Update()
    {
        PointToNextObjective();
    }
}
