using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoubellesPool : MonoBehaviour
{
    public static PoubellesPool Instance;
    public List<GameObject> pooledPoubelles;
    public GameObject objectToPool;
    public int amountToPool;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        pooledPoubelles = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(objectToPool);
            tmp.SetActive(false);
            pooledPoubelles.Add(tmp);
        }
    }

    public GameObject GetPoubelle()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (!pooledPoubelles[i].activeInHierarchy)
            {
                return pooledPoubelles[i];
            }
        }
        return null;
    }
}
