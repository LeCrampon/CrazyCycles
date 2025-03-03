using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EndlessPathGeneration : MonoBehaviour
{
    public static EndlessPathGeneration _instance;

    public const string ASSETSPATH = "BuildingBlocks/Roads";
    public List<RoadModule> roadModules;

    public GameObject loadingScreen;
    public GameObject bicycleCharacter;

    public RoadModule currentModule;
    public Vector3 startingPoint = Vector3.zero;

    public int CELL_STEP = 8;

    public List<RoadModule> currentModules = new List<RoadModule>();

	private void Awake()
	{
		
	}

	private void Start()
	{
        roadModules = Resources.LoadAll(ASSETSPATH, typeof(RoadModule)).Cast<RoadModule>().ToList();
    }

	public void LoadNextModules(MapCell m)
	{
        List<RoadModule> possibleModules = m.CheckNeighbours();
        if (possibleModules.Count > 1)
            m.roadModule = Instantiate(possibleModules[Random.Range(0, possibleModules.Count)]);
        else if (possibleModules.Count == 1)
            m.roadModule = Instantiate(possibleModules[0]);

        if (m.roadModule != null && startingPoint != null)
        {
            m.roadModule.transform.position = new Vector3(startingPoint.x - CELL_STEP * m.posX, startingPoint.y, startingPoint.z - CELL_STEP * m.posY);
            m.roadModule.transform.rotation = Quaternion.identity;
        }
    }
}
