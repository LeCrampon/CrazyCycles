using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.AI.Navigation;

public class MapGenerator : MonoBehaviour
{

    public Vector3 startingPoint = Vector3.zero;

    public const string ASSETSPATH = "BuildingBlocks/Roads";

    public int mapSizeX;
    public int mapSizeY;
    public int CELL_STEP = 8;

    public List<RoadModule> roadModules;

    private MapCell[,] generatedMap;

    bool mapFilled = false;

    [SerializeField]
    private NavMeshSurface navMesh;

    public GameObject loadingScreen;
    public GameObject bicycleCharacter;

    private void Start()
    {
        roadModules = Resources.LoadAll(ASSETSPATH, typeof(RoadModule)).Cast<RoadModule>().ToList();
        GenerateMap();
        StartCoroutine(PopulateMap());
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    StartCoroutine(PopulateMap());
        //}
    }

    public void PopulateModule(MapCell m)
    {
        List<RoadModule> possibleModules = m.CheckNeighbours();
        if(possibleModules.Count > 1)
            m.roadModule = Instantiate(possibleModules[Random.Range(0, possibleModules.Count)]);
        else if(possibleModules.Count == 1)
            m.roadModule = Instantiate(possibleModules[0]);

        if(m.roadModule != null && startingPoint != null)
		{
            m.roadModule.transform.position = new Vector3(startingPoint.x - CELL_STEP * m.posX, startingPoint.y, startingPoint.z - CELL_STEP * m.posY);
            m.roadModule.transform.rotation = Quaternion.identity;
        }
       
    }

    void GenerateMap()
    {
        generatedMap = new MapCell[mapSizeX, mapSizeY];

        for (int i = 0; i< mapSizeX; i++)
        {
            for(int j =0; j< mapSizeY; j++)
            {
                generatedMap[i, j] = new MapCell();
                generatedMap[i, j].posX = i;
                generatedMap[i, j].posY = j;
                generatedMap[i, j].mapSizeX = mapSizeX;
                generatedMap[i, j].mapSizeY = mapSizeY;
                generatedMap[i, j].map = generatedMap;
                generatedMap[i, j].generator = this;
                generatedMap[i, j].allModules = roadModules;
            }
        }



    }
    private IEnumerator PopulateMap()
    {
        //loadingScreen.SetActive(true);
        yield return new WaitForSeconds(.5f);
        for (int i =0; i< mapSizeX; i++)
        {
            for(int j =0; j< mapSizeY; j++)
            {
                yield return new WaitForSeconds(.01f);
                PopulateModule(generatedMap[i, j]);

            }
        }
        //navMesh.BuildNavMesh();
        PoubelleManager.Instance.InstantiateAllPoubelles();
        loadingScreen.SetActive(false);
        bicycleCharacter.GetComponent<Rigidbody>().isKinematic = false;
        GameManager.Instance.gameStarted = true;
        StartCoroutine(RedLightManager.Instance.SwitchRedLights());
        yield return new WaitForSeconds(1f);
        ObjectiveManager._instance.ChooseNextObjective();

    }

    //private void PopulateMap()
    //{
    //    int cpt = 0;
    //    int firstX = Random.Range(0, 0);
    //    int firstY = Random.Range(0, 0);

    //    while (!mapFilled)
    //    {
    //        if (cpt == 0)
    //        {
    //            PopulateModule(generatedMap[firstX, firstY]);
    //            mapFilled = true;
    //        }
    //        CheckIfMapFilled();
    //        if (mapFilled)
    //        {
    //            return;
    //        }


    //        cpt++;
    //    }
    //}

    private void PopulateFirstCell()
    {
        int indexX = Random.Range(0, mapSizeX);
        int indexY = Random.Range(0, mapSizeY);
       
    }

    void CheckIfMapFilled()
    {
        foreach(MapCell m in generatedMap)
        {
            if(m.roadModule == null)
            {
                return;
            }
        }

        mapFilled = true;
    }


   




}
