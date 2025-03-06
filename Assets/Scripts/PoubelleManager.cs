using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PoubelleManager : MonoBehaviour
{
    public static PoubelleManager Instance;
    public List<PoubelleSpawn> poubelleSpawns;
    public bool started = false;
    public bool once = false;

    public int _poubellePoints = 20;
    [SerializeField]
    private GameObject _poubellePopUp;

	private void Awake()
	{
        Instance = this;
	}

    public void InstantiateAllPoubelles()
	{
        poubelleSpawns = FindObjectsOfType<PoubelleSpawn>().ToList<PoubelleSpawn>();
        for (int i = 0; i < PoubellesPool.Instance.amountToPool; i++)
        {
            InstantiatePoubelle(SelectRandomSpawn());
        }
        started = true;
    }

    PoubelleSpawn SelectRandomSpawn()
	{
        List<PoubelleSpawn> unusedSpawns = poubelleSpawns.Where(s => s.used == false).ToList();
        IEnumerable unused = poubelleSpawns.Where(s => s.used == false);
        return unusedSpawns[Random.Range(0, unusedSpawns.Count)];
    }

    // Update is called once per frame
    void Update()
    {
		if (started && !once)
		{
            StartCoroutine(InstantiatePoubelles());
        }

    }

    IEnumerator InstantiatePoubelles()
	{
		while (true)
		{
            yield return new WaitForSeconds(10f);
            if (PoubellesPool.Instance.GetPoubelle() != null)
            {

                InstantiatePoubelle(SelectRandomSpawn());

            }
        }

	}

    void InstantiatePoubelle(PoubelleSpawn spawn)
	{
        GameObject poubelle = PoubellesPool.Instance.GetPoubelle();
        poubelle.GetComponent<PoubelleCollision>().ResetPoubelle();
        poubelle.GetComponent<PoubelleCollision>().poubelleSpawn = spawn;
        poubelle.transform.position = spawn.transform.position;
        spawn.used = true;
        poubelle.transform.rotation = Quaternion.identity;
        poubelle.SetActive(true);
    }

    public void ActivatePoubellePopUp(Vector3 position)
    {
        Vector3 spawnPosition = new Vector3(position.x, position.y + 1.5f, position.z);
        _poubellePopUp.transform.position = spawnPosition;
        _poubellePopUp.SetActive(true);
    }
}
