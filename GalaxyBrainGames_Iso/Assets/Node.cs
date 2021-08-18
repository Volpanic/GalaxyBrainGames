using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool IsWall;
    public bool IsGround;
    public Vector3 Position;
    public Vector3 TemporalPosition;

    public Node Parent;

    public float gCost;
    public float hCost;

    public float FCost { get { return gCost + hCost; } }

    public Node(Vector3 gridPos,bool isWall, bool isGround)
    {
        Position = gridPos;
        IsWall = isWall;
        IsGround = isGround;
    }

    public Node(Vector3 gridPos, bool isWall, bool isGround, Vector3 temporalPositon)
    {
        Position = gridPos;
        IsWall = isWall;
        IsGround = isGround;
        TemporalPosition = temporalPositon;
    }
}
