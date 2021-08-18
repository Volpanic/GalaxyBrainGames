using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class GridPathfinding : MonoBehaviour
{
    [SerializeField] LayerMask groundMask;
    [SerializeField] LineRenderer pathRenderer;

    [SerializeField, Range(0f, 1f)] private float sampleMovement = 0.5f;

    private Vector3 lastArea;
    private bool hasHit = false;
    private Camera cam;

    private List<Vector3> path;
    private Transform owner;
    private Vector3 gridOffset;

    private Dictionary<Vector3, Node> nodeGrid = new Dictionary<Vector3, Node>();

    private void Awake()
    {
        cam = Camera.main;
    }

    public void LookForPath()
    {
        FindMousePoint();
    }

    public void SetOwner(Transform newOwner)
    {
        owner = newOwner;
    }

    public void SetOffset(Vector3 offset)
    {
        gridOffset = offset;
    }

    private void FindMousePoint()
    {
        Ray camRay = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        hasHit = Physics.Raycast(camRay, out hit, 100f, groundMask);

        if (hasHit && hit.normal == Vector3.up)
        {
            if (ToGridPos(hit.point + new Vector3(0, 0.1f, 0)) != lastArea)
            {
                //Convert to grid position
                lastArea = ToGridPos(hit.point + new Vector3(0, 0.1f, 0));

                //Path Finding
                FillGrid(ToGridPos(owner.position), lastArea);
                List<Node> nodePath = FindPath(ToGridPos(owner.position), lastArea);

                //Visualization
                UpdateLineRenderer(nodePath);

            }

            lastArea = ToGridPos(hit.point + Vector3.up);
        }
    }

    private void UpdateLineRenderer(List<Node> nodePath)
    {
        path = new List<Vector3>();
        if (pathRenderer != null && nodePath != null && nodePath.Count >= 2)
        {
            for (int i = 0; i < nodePath.Count; i++)
            {
                pathRenderer.positionCount = nodePath.Count;
                pathRenderer.SetPosition(i, nodePath[i].TemporalPosition);

                path.Add(nodePath[i].Position);
            }
        }
    }

    public List<Vector3> GetPath()
    {
        return path;
    }

    #region Grid

    public Vector3 ToGridPos(Vector3 worldPos)
    {
        // 1 is placeholder in case grid size is changed
        float xpoint = ((worldPos.x + 1 / 2) / 1);
        float ypoint = ((worldPos.y + 1 / 2) / 1);
        float zpoint = ((worldPos.z + 1 / 2) / 1);

        return new Vector3(Mathf.Round(xpoint), Mathf.Round(ypoint), Mathf.Round(zpoint)) + gridOffset;
    }

    // Since the grid is stored in a dictionary instead of a 
    // 2D array, we create the info in between so we can see it.
    private void FillGrid(Vector3 startPos, Vector3 endPos)
    {
        CreateAndStoreNode(startPos);
        CreateAndStoreNode(endPos);

        Vector3 min = new Vector3(Mathf.Min(startPos.x, endPos.x),
            Mathf.Min(startPos.y, endPos.y),
            Mathf.Min(startPos.z, endPos.z));

        Vector3 max = new Vector3(Mathf.Max(startPos.x, endPos.x),
            Mathf.Max(startPos.y, endPos.y),
            Mathf.Max(startPos.z, endPos.z));

        //Make sure final node exist
        Vector3 lastPos = new Vector3(0, 0, 0);

        for (float xx = min.x; xx < max.x; xx++)
        {
            for (float yy = min.y; yy < max.y; yy++)
            {
                for (float zz = min.z; zz < max.z; zz++)
                {
                    lastPos = new Vector3(xx, yy, zz);
                    CreateAndStoreNode(lastPos);
                }
            }
        }
    }

    //Adds the node to the 'grid' if it's not there already.
    public Node CreateAndStoreNode(Vector3 pos)
    {
        if (nodeGrid.ContainsKey(pos)) return nodeGrid[pos];

        //Check for wall
        bool wall = (Physics.OverlapBox(pos, new Vector3(0.45f, 0.45f, 0.45f), Quaternion.identity, groundMask).Length > 0);

        //Check if top of cell is blocked, could be a slope if it isn't
        bool canBeSlope = (Physics.OverlapBox(pos + new Vector3(0,0.45f,0), new Vector3(0.045f, 0.045f, 0.045f), Quaternion.identity, groundMask).Length == 0);

        //Add the node
        Node node = new Node(pos, wall, false, pos - new Vector3(0, 0.45f, 0));

        RaycastHit hit;

        if (canBeSlope)
        {
            bool didHit = Physics.Raycast(new Ray(pos + new Vector3(0, 0.48f, 0), Vector3.down), out hit, 1.5f, groundMask);
            if (didHit && Mathf.Abs(Vector3.Dot(hit.point, Vector3.up)) > 0.2f)
            {
                node = new Node(pos, false, true, hit.point + new Vector3(0, 0.05f, 0));
            }
        }

        nodeGrid[pos] = node;

        return node;
    }


    float GetManhattenDistance(Node nodeA, Node nodeB)
    {
        //Get distance between nodes in grid
        float ix = Math.Abs(nodeA.Position.x - nodeB.Position.x);
        float iy = Math.Abs(nodeA.Position.y - nodeB.Position.y);
        float iz = Math.Abs(nodeA.Position.z - nodeB.Position.z);

        return ix + iy + iz;
    }

    private List<Node> GetNeighborNodes(Node current)
    {
        List<Node> adjacentNode = new List<Node>();

        Vector3 pos = current.Position;

        if (nodeGrid.ContainsKey(ToGridPos(pos + Vector3.right))) adjacentNode.Add(nodeGrid[ToGridPos(pos + Vector3.right)]);
        if (nodeGrid.ContainsKey(ToGridPos(pos + Vector3.left))) adjacentNode.Add(nodeGrid[ToGridPos(pos + Vector3.left)]);

        if (nodeGrid.ContainsKey(ToGridPos(pos + Vector3.forward))) adjacentNode.Add(nodeGrid[ToGridPos(pos + Vector3.forward)]);
        if (nodeGrid.ContainsKey(ToGridPos(pos + Vector3.back))) adjacentNode.Add(nodeGrid[ToGridPos(pos + Vector3.back)]);

        //Up Slope Check
        if (nodeGrid.ContainsKey(ToGridPos(pos + Vector3.right + Vector3.up))) adjacentNode.Add(nodeGrid[ToGridPos(pos + Vector3.right + Vector3.up)]);
        if (nodeGrid.ContainsKey(ToGridPos(pos + Vector3.left + Vector3.up))) adjacentNode.Add(nodeGrid[ToGridPos(pos + Vector3.left + Vector3.up)]);

        if (nodeGrid.ContainsKey(ToGridPos(pos + Vector3.forward + Vector3.up))) adjacentNode.Add(nodeGrid[ToGridPos(pos + Vector3.forward + Vector3.up)]);
        if (nodeGrid.ContainsKey(ToGridPos(pos + Vector3.back + Vector3.up))) adjacentNode.Add(nodeGrid[ToGridPos(pos + Vector3.back + Vector3.up)]);

        //Down Slope Check
        if (nodeGrid.ContainsKey(ToGridPos(pos + Vector3.right + Vector3.down))) adjacentNode.Add(nodeGrid[ToGridPos(pos + Vector3.right + Vector3.down)]);
        if (nodeGrid.ContainsKey(ToGridPos(pos + Vector3.left + Vector3.down))) adjacentNode.Add(nodeGrid[ToGridPos(pos + Vector3.left + Vector3.down)]);

        if (nodeGrid.ContainsKey(ToGridPos(pos + Vector3.forward + Vector3.down))) adjacentNode.Add(nodeGrid[ToGridPos(pos + Vector3.forward + Vector3.down)]);
        if (nodeGrid.ContainsKey(ToGridPos(pos + Vector3.back + Vector3.down))) adjacentNode.Add(nodeGrid[ToGridPos(pos + Vector3.back + Vector3.down)]);

        return adjacentNode;
    }

    #endregion

    #region Pathfinding

    private List<Node> FindPath(Vector3 p1, Vector3 p2)
    {
        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        Node startNode = nodeGrid[p1];
        Node targetNode = nodeGrid[p2];

        openList.Add(startNode);

        if (openList[0] == null)
        {
            Debug.Log("No!!! NOOOO");
            return null;
        }

        while (openList.Count > 0)
        {
            Node current = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].FCost < current.FCost || openList[i].FCost == current.FCost && openList[i].hCost < current.hCost)
                {
                    current = openList[i];
                }
            }

            openList.Remove(current);
            closedList.Add(current);

            if (current == targetNode)
            {
                return GetFinalPath(startNode, targetNode);
            }

            foreach (Node neighborNode in GetNeighborNodes(current))
            {
                if (neighborNode.IsWall || !neighborNode.IsGround || closedList.Contains(neighborNode))
                {
                    continue;
                }

                float moveCost = current.gCost + GetManhattenDistance(current, neighborNode);

                if (moveCost < neighborNode.gCost || !openList.Contains(neighborNode))
                {
                    neighborNode.gCost = moveCost;
                    neighborNode.hCost = GetManhattenDistance(neighborNode, targetNode);
                    neighborNode.Parent = current;

                    if (!openList.Contains(neighborNode))
                    {
                        openList.Add(neighborNode);
                    }
                }
            }
        }

        return null;
    }

    private List<Node> GetFinalPath(Node startNode, Node targetNode)
    {
        List<Node> finalPath = new List<Node>();
        Node currentNode = targetNode;

        while (currentNode != startNode)
        {
            finalPath.Add(currentNode);
            currentNode = currentNode.Parent;
        }

        finalPath.Reverse();

        return finalPath;
    }

    #endregion
}
