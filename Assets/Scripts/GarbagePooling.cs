using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbagePooling : MonoBehaviour
{
    public static GarbagePooling _instance;
    private List<GameObject> _pooledGarbage;

    [SerializeField]
    private int _amountToPool;

    [SerializeField]
    private GameObject _garbagePrefab;

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

    }
    private void Start()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        GameObject garbage;
        _pooledGarbage = new List<GameObject>(_amountToPool);
        for(int i=0; i < _amountToPool; i++)
        {
             garbage = Instantiate(_garbagePrefab);
            garbage.SetActive(false);
            garbage.transform.parent = transform;
            _pooledGarbage.Add(garbage);

        }
    }

    public GameObject GetGarbage()
    {
        for (int i = 0; i < _amountToPool; i++)
        {
            if (!_pooledGarbage[i].activeInHierarchy)
            {
                return _pooledGarbage[i];
            }
        }
        return null;
    }
}
