using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValuesHolder : MonoBehaviour
{
    public static ValuesHolder _instance;

    public BikeController _bikeController;
    public PauseMenu _pauseMenu;

    private void Awake()
    {
        if(_instance != this && _instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }
}
