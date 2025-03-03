using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SideState
{
    Grass = 0,
    Road = 1
}

public enum Side
{
    Top =0,
    Bottom =1,
    Left = 2,
    Right = 3

}

public class RoadModule : MonoBehaviour
{ 
    public SideState top;
    public SideState left;
    public SideState right;
    public SideState bottom;
}
