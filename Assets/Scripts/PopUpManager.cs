using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpManager : MonoBehaviour
{
    public static PopUpManager _instance;

    public List<GameObject> _popupsPool;
    public GameObject popUpToPool;
    public int amountToPool;

    private void Awake()
    {
        if(_instance != this && _instance != null)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        _popupsPool = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(popUpToPool);
            tmp.SetActive(false);
            _popupsPool.Add(tmp);
        }
    }

    public GameObject GetPopUp()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (!_popupsPool[i].activeInHierarchy)
            {
                return _popupsPool[i];
            }
        }
        return null;
    }
}
