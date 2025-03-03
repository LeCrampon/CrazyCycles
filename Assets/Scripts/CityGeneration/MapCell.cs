using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapCell 
{
    public RoadModule roadModule;
    public MapCell[,] map;
    public int posX;
    public int posY;
    public int mapSizeX;
    public int mapSizeY; 
    public List<RoadModule> allModules;
    public MapGenerator generator;

    public List<RoadModule> CheckNeighbours()
    {

        List<RoadModule> possibleModules = new List<RoadModule>(allModules);


        //Check Top
        if (posX + 1 >= mapSizeX)
        {
            possibleModules.RemoveAll(x => x.bottom == SideState.Road);
        }
        else if (posX + 1 < mapSizeX)
        {
            if(map[posX +1, posY] != null && map[posX + 1, posY].roadModule != null)
            {
                possibleModules = CheckPossibleModules(map[posX +1, posY].roadModule, Side.Bottom, possibleModules);

            }
            else
            {
            }
        }
        //CheckBottom
        if(posX - 1 < 0)
        {
            possibleModules.RemoveAll(x => x.top == SideState.Road);
        }
        else if (posX - 1 >= 0)
        {

            if (map[posX-1, posY] != null && map[posX - 1, posY].roadModule != null)
            {
                possibleModules = CheckPossibleModules(map[posX -1, posY].roadModule, Side.Top, possibleModules);
            }
            else { }
            
        }


        //CheckRight

        if (posY - 1 < 0)
        {
            possibleModules.RemoveAll(x => x.left == SideState.Road);
        }
        else if (posY - 1 >= 0)
        {
            if (map[posX, posY-1] != null && map[posX, posY - 1].roadModule != null)
            {
                possibleModules = CheckPossibleModules(map[posX, posY -1].roadModule, Side.Left, possibleModules);
            }
            else
            {

            }
        }

        //CheckRight
        if (posY + 1 >= mapSizeY)
        {
            possibleModules.RemoveAll(x => x.right == SideState.Road);
        }
        else if (posY + 1 < mapSizeY)
        {
            if (map[posX, posY +1] != null && map[posX , posY +1].roadModule != null)
            {
                possibleModules = CheckPossibleModules(map[posX, posY +1].roadModule, Side.Right, possibleModules);
            }
            else
            {
            }
        }


        return possibleModules;
    }



    public List<RoadModule> CheckPossibleModules(RoadModule checkedModule, Side side, List<RoadModule> possibleModules)
    {

        switch (side)
        {
            case Side.Top:
                possibleModules.RemoveAll(x => x.top != checkedModule.bottom);
                break;
            case Side.Bottom:
                possibleModules.RemoveAll(x => x.bottom != checkedModule.top);
                break;
            case Side.Left:
                possibleModules.RemoveAll(x => x.left != checkedModule.right);
                break;
            case Side.Right:
                possibleModules.RemoveAll(x => x.right != checkedModule.left);
                break;
        }


        return possibleModules;
    }
}
