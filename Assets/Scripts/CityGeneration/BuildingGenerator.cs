using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class BuildingGenerator : MonoBehaviour
{
    public const string ASSETSPATH = "BuildingBlocks";
    public const string WINDOWSFOLDER = "/Windows";
    public const string BUILDINGSFOLDER = "/Buildings";
    public const string DOORSFOLDER = "/Doors";
    public const string PANELSFOLDER = "/Panels";
    public const string ROOFSFOLDER = "/Roofs";
    public const string CORNERSPATH = "/Corners";

    public const int PANEL_STEP = 2;

    public Vector3 startPosition = new Vector3(0, 0, 0);
    public Quaternion startRotation = Quaternion.identity;

    public GameObject chosenBuilding;
    public GameObject chosenWindow;
    public GameObject chosenDoor;

    public GameObject[] loadedBuildings;
    public GameObject[] loadedWindows;
    public GameObject[] loadedDoors;
    public GameObject[] loadedPanels;
    public GameObject[] loadedRoofs;
    public GameObject[] loadedCorners;

    public GameObject[] windows;
    public GameObject[] doors;

    public int minX = 5;
    public int maxX = 50;


    public int minY = 3;
    public int maxY = 15;

    public int minZ = 5;
    public int maxZ = 50;

    private List<GameObject> generatedBuildings = new List<GameObject>();

    public Transform[] spawnPoints;

    public RectangleSplit rectangleSplit;

    public GameObject testCube;

    private void LoadResources()
    {
        LoadBuildings();
        LoadWindows();
        LoadDoors();
        LoadPanels();
        LoadRoofs();
        LoadCorners();
    }

    private void LoadBuildings()
    {
        string path = ASSETSPATH + BUILDINGSFOLDER;
        loadedBuildings = Resources.LoadAll(path, typeof(GameObject)).Cast<GameObject>().ToArray();
    }

    private void LoadWindows()
    {
        string path = ASSETSPATH + WINDOWSFOLDER;
        loadedWindows = Resources.LoadAll(path, typeof(GameObject)).Cast<GameObject>().ToArray();
    }

    private void LoadDoors()
    {
        string path = ASSETSPATH + DOORSFOLDER;
        loadedDoors = Resources.LoadAll(path, typeof(GameObject)).Cast<GameObject>().ToArray();
    }

    private void LoadPanels()
    {
        string path = ASSETSPATH + PANELSFOLDER;
        loadedPanels = Resources.LoadAll(path, typeof(GameObject)).Cast<GameObject>().ToArray();
    }

    private void LoadRoofs()
    {
        string path = ASSETSPATH + ROOFSFOLDER;
        loadedRoofs = Resources.LoadAll(path, typeof(GameObject)).Cast<GameObject>().ToArray();
    }

    private void LoadCorners()
    {
        string path = ASSETSPATH + CORNERSPATH;
        loadedCorners = Resources.LoadAll(path, typeof(GameObject)).Cast<GameObject>().ToArray();
    }


    private GameObject InstantiatePanel(Vector3 position, Transform parent, int index)
    {

        GameObject instance = Instantiate(loadedPanels[index], parent) as GameObject;

        instance.transform.localPosition = position;
        instance.transform.localRotation = Quaternion.identity;
        return instance;
    }

    private GameObject InstantiateWindow(Vector3 position, Transform parent, int index)
    {

        GameObject instance = Instantiate(loadedWindows[index], parent) as GameObject;

        instance.transform.localPosition = position;
        instance.transform.localRotation = Quaternion.identity;
        return instance;
    }


    private GameObject InstantiateCorner(Vector3 position, Transform parent, int index)
    {

        GameObject instance = Instantiate(loadedCorners[index], parent) as GameObject;

        instance.transform.localPosition = position;
        instance.transform.localRotation = Quaternion.identity;
        return instance;
    }

    private GameObject InstantiateWall(int wallWidth, int wallHeight, Vector3 position, Transform parent, int index)
    {
        GameObject wall = new GameObject();
        wall.transform.position = position;
        wall.transform.parent = parent;
        wall.transform.localRotation = Quaternion.identity;
        wall.name = "Wall";
        GameObject[,] wallElements = new GameObject[wallWidth, wallHeight];

        for(int i =0; i< wallWidth; i++)
        {
            for(int j = 0; j< wallHeight; j++)
            {
                wallElements[i, j] = InstantiatePanel(new Vector3(startPosition.x + i * PANEL_STEP, startPosition.y+ j * PANEL_STEP, startPosition.z ), wall.transform, index);
            }
        }

        return wall;
    }

    private GameObject InstantiateCorners(int wallWidth, int wallDepth, Vector3 position, Transform parent, int index)
    {
        GameObject corners = new GameObject();

        corners.transform.parent = parent;
        corners.transform.localPosition = Vector3.zero;
        corners.transform.localRotation = Quaternion.identity;
        corners.name = "Corners";

        GameObject corner1 = InstantiateCorner(Vector3.zero, parent, index);
        GameObject corner2 = InstantiateCorner(new Vector3(0, 0 , wallDepth * PANEL_STEP), parent, index);
        GameObject corner3 = InstantiateCorner(new Vector3(wallWidth * PANEL_STEP, 0, wallDepth * PANEL_STEP), parent, index);
        GameObject corner4 = InstantiateCorner(new Vector3(wallWidth * PANEL_STEP, 0, 0), parent, index);
        return corners;
    }


    private GameObject InstantiateWallWindows(int wallWidth, int wallHeight, Vector3 position, Transform parent, int index)
    {
        GameObject wall = new GameObject();
        wall.transform.position = position;
        wall.transform.parent = parent;
        wall.transform.localRotation = Quaternion.identity;
        wall.name = "WallWindows";
        GameObject[,] wallElements = new GameObject[wallWidth, wallHeight];

        for (int i = 0; i < wallWidth; i++)
        {
            for (int j = 0; j < wallHeight; j++)
            {
                if(i%2 != 0)
                {
                    wallElements[i, j] = InstantiateWindow(new Vector3(startPosition.x + i * PANEL_STEP, startPosition.y + j * PANEL_STEP, startPosition.z), wall.transform, index);
                }
                else
                {
                    wallElements[i, j] = InstantiatePanel(new Vector3(startPosition.x + i * PANEL_STEP, startPosition.y + j * PANEL_STEP, startPosition.z), wall.transform, index);
                }
                
            }
        }

        return wall;
    }

    private GameObject InstantiateRoof(int roofWidth, int roofHeight, int roofDepth, Vector3 position, Transform parent)
    {
        int index = Random.Range(0, loadedRoofs.Length);
        GameObject instance = Instantiate(loadedRoofs[index], parent) as GameObject;

        instance.transform.localPosition = new Vector3 (0, roofHeight * PANEL_STEP, 0);
        instance.transform.localRotation = Quaternion.identity;
        instance.transform.localScale = new Vector3(roofWidth, 1, roofDepth);
        return instance;
    }

    //private GameObject InstantiateBuilding()
    //{
    //    int index = Random.Range(0, loadedBuildings.Length);
    //    GameObject instance = Instantiate(loadedBuildings[index]) as GameObject;

    //    instance.transform.localScale = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), Random.Range(minZ, maxZ));
    //    instance.transform.position = new Vector3(startPosition.x, startPosition.y, startPosition.z);
    //    chosenBuilding = instance;
    //    return chosenBuilding;
    //}


    private GameObject InstantiateFloor(int buildingWidth, int buildingHeight, int buildingDepth, Vector3 position, Quaternion rotation,Transform parent, int index)
    {


        GameObject floor = new GameObject();
        floor.name = "Floor";
        floor.transform.position = position;

        GameObject wall1 = InstantiateWall(buildingWidth, buildingHeight, position,floor.transform, index);
        GameObject wall2 = InstantiateWall(buildingWidth, buildingHeight, new Vector3(position.x + PANEL_STEP * buildingWidth, position.y, position.z + PANEL_STEP * buildingDepth), floor.transform, index);
        wall2.transform.Rotate(new Vector3(0, 180, 0));
        GameObject wall3 = InstantiateWall(buildingDepth, buildingHeight, new Vector3(position.x , position.y, position.z + PANEL_STEP * buildingDepth), floor.transform, index);
        wall3.transform.Rotate(new Vector3(0,90,0));
        GameObject wall4 = InstantiateWall(buildingDepth, buildingHeight, new Vector3(position.x + PANEL_STEP * buildingWidth, position.y, position.z), floor.transform, index);
        wall4.transform.Rotate(new Vector3(0, -90, 0));

        InstantiateCorners(buildingWidth, buildingDepth, position, floor.transform, index);
        InstantiateRoof(buildingWidth, buildingHeight, buildingDepth, position, floor.transform);

        floor.transform.rotation = rotation;
        floor.transform.parent = parent;
        return floor;
    }

    private GameObject InstantiateFloorWindows(int buildingWidth, int buildingHeight, int buildingDepth, Vector3 position, Quaternion rotation, Transform parent, int index)
    {


        GameObject floor = new GameObject();
        floor.name = "FloorWindows";
        floor.transform.position = position;

        GameObject wall1 = InstantiateWallWindows(buildingWidth, buildingHeight, position, floor.transform, index);
        GameObject wall2 = InstantiateWallWindows(buildingWidth, buildingHeight, new Vector3(position.x + PANEL_STEP * buildingWidth, position.y, position.z + PANEL_STEP * buildingDepth), floor.transform, index);
        wall2.transform.Rotate(new Vector3(0, 180, 0));
        GameObject wall3 = InstantiateWallWindows(buildingDepth, buildingHeight, new Vector3(position.x, position.y, position.z + PANEL_STEP * buildingDepth), floor.transform, index);
        wall3.transform.Rotate(new Vector3(0, 90, 0));
        GameObject wall4 = InstantiateWallWindows(buildingDepth, buildingHeight, new Vector3(position.x + PANEL_STEP * buildingWidth, position.y, position.z), floor.transform, index);
        wall4.transform.Rotate(new Vector3(0, -90, 0));

        InstantiateRoof(buildingWidth, buildingHeight, buildingDepth, position, floor.transform);
        InstantiateCorners(buildingWidth, buildingDepth, position, floor.transform, index);

        floor.transform.rotation = rotation;
        floor.transform.parent = parent;

        return floor;
    }

    private GameObject InstantiateBuilding(int buildingWidth, int buildingHeight, int buildingDepth, Vector3 position, Quaternion rotation, int indexWall)
    {
        GameObject building = new GameObject();
        building.name = "Building";
        building.transform.position = position;

        for(int i =0; i< buildingHeight; i++)
        {
            if(i%2 == 0)
            {
                GameObject floor = InstantiateFloor(buildingWidth, 1, buildingHeight, new Vector3(position.x, position.y + i * PANEL_STEP, position.z), rotation, building.transform, indexWall);
            }
            else
            {
                GameObject floor = InstantiateFloorWindows(buildingWidth, 1, buildingHeight, new Vector3(position.x, position.y + i * PANEL_STEP, position.z), rotation, building.transform, indexWall);
            }
        }

        building.transform.rotation = rotation;
        building.transform.parent = this.transform;
        return building;
    }




    private void Start()
    {
        LoadResources();
    }

    public GameObject Generate(Vector3 position, Quaternion rotation)
    {
        //GameObject building = InstantiateBuilding(5,10, 15, startPosition, startRotation);
        int index = Random.Range(0, loadedPanels.Length);
        int sizeX = Random.Range(minX, maxX);
        sizeX += 1 - sizeX % 2;
        int sizeY = Random.Range(minY, maxY);
        sizeY += 1 - sizeY % 2;
        int sizeZ = Random.Range(minZ, maxZ);
        sizeZ += 1 - sizeZ % 2;
        GameObject building = InstantiateBuilding(sizeX, sizeY, sizeZ, position, rotation, index); 
        //InstantiateWindows(building);
        //      GameObject wall1 = InstantiateWall(Random.Range(minX, maxX), Random.Range(minY, maxY), position);

        return building;
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    if (generatedBuildings.Count != 0)
        //    {
        //        for(int i = 0; i< generatedBuildings.Count; i++)
        //        {
        //            Destroy(generatedBuildings[i]);
        //        }
        //        generatedBuildings.Clear();
        //    }

        //    foreach (Transform t in spawnPoints)
        //    {
        //        generatedBuildings.Add(Generate(t.position, t.rotation));
        //    }
        //}

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rectangleSplit.DrawRectangles();
            //foreach( Rectangle r in rectangleSplit.rectangles)
            //{
            //    //InstantiateBuilding(r.length, 2, r.width, r.position, Quaternion.identity, Random.Range(0, loadedPanels.Length));
            //    GameObject cube = Instantiate(testCube, transform);
            //    cube.transform.position = r.position;
            //    cube.transform.localScale = new Vector3(r.length, 0, r.width);
            //}
        }
    }
}
