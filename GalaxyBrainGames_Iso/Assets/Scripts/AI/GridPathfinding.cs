using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace GalaxyBrain.Pathfinding
{
    [SelectionBase]
    public class GridPathfinding : MonoBehaviour
    {
        [Header("Masks")]
        [SerializeField] LayerMask groundMask;
        [SerializeField] LayerMask climbableMask;
        [SerializeField] LayerMask slopeMask;
        [SerializeField] LayerMask waterMask;
        [SerializeField] LayerMask dynamicPathBlockingMask;

        [Header("Visualization")]
        [SerializeField] LineRenderer pathRenderer;
        [SerializeField] Gradient validPathGradiant;
        [SerializeField] Gradient nonvalidPathGradiant;

        public event Action OnPathChanged;
        private Func<Node, Node, Node, Node, bool> extraNodeConditons;

        private Vector3 lastArea;

        private List<Vector3> path = new List<Vector3>();
        private Transform owner;
        private Vector3 gridOffset;
        private bool viablePath = false;

        private bool canSwim = false;
        private bool canClimb = false;
        private bool ownerMoving = false;
        bool isClimbing = false;

        public bool LookPathPath = false;

        private Dictionary<Vector3, Node> nodeGrid = new Dictionary<Vector3, Node>();

        public bool LookForPath(RaycastHit hit)
        {
            if (!LookPathPath || owner == null || ownerMoving)
            {
                path.Clear();
                return false;
            }

            if (ToGridPos(hit.point + new Vector3(0, 0.5f, 0)) != lastArea)
            {
                //Convert to grid position
                lastArea = ToGridPos(hit.point + new Vector3(0, 0.1f, 0));
                Vector3 ownerPos = ToGridPos(owner.position + new Vector3(0,0.5f,0));

                //Path Finding
                FillGrid(ownerPos, lastArea);
                List<Node> nodePath = FindPath(ownerPos, lastArea);

                //Visualization
                UpdatePath(nodePath);
            }

            lastArea = ToGridPos(hit.point + Vector3.up);
            OnPathChanged?.Invoke();
            LookPathPath = false;

            return true;
        }

        //Makes it so the path is non viable, meaning it cant be used for the mean time
        public void ForceUnvialblePath()
        {
            viablePath = false;
        }

        public void SetOwner(Transform newOwner, bool climb = false, bool swim = false, Func<Node, Node, Node, Node, bool> nodeCondtions = null)
        {
            if (owner != newOwner)
            {
                //We've changed the owner, so scrap the current path
                if (path != null) path.Clear();
                viablePath = false;
                UpdateLineRenderer();
            }

            owner = newOwner;
            canClimb = climb;
            canSwim = swim;
            LookPathPath = true;
            extraNodeConditons = nodeCondtions;
        }

        //Path info
        public List<Vector3> GetPath()
        {
            return (viablePath) ? path : null;
        }

        public Vector3 GetPathEndPoint()
        {
            if (!viablePath || path.Count <= 0) return owner.position;
            else return path[path.Count - 1];
        }

        public int GetPathCount()
        {
            if (!viablePath || path.Count <= 0) return 0;
            else return path.Count;
        }

        private void UpdatePath(List<Node> nodePath)
        {
            path = new List<Vector3>();

            if (nodePath != null && nodePath.Count > 0)
            {
                path.Add(owner.position);

                for (int i = 0; i < nodePath.Count; i++)
                {
                    //pathRenderer.SetPosition(i + 1, nodePath[i].TemporalPosition);

                    Node node = nodePath[i];


                    if (node.IsSlope)
                    {
                        Vector3 oldPos = path[path.Count - 1];
                        Vector3 dir = (node.Position - oldPos) * 0.5f;

                        dir.y = 0;
                        dir = dir.normalized;

                        path.Add(new Vector3(node.Position.x, oldPos.y, node.Position.z) - (dir * 0.5f));

                        //path.Add(node.Position);

                        if (i + 1 < nodePath.Count)
                        {
                            Node nextNode = nodePath[i + 1];
                            path.Add(new Vector3(node.Position.x, nextNode.Position.y, node.Position.z) + (dir * 0.5f));
                        }
                    }
                    else
                    {
                        path.Add(node.Position);
                    }
                }
            }

            UpdateLineRenderer();
        }

        private void UpdateLineRenderer()
        {
            if (pathRenderer != null)
            {
                if (path == null || path.Count < 2)
                {
                    pathRenderer.positionCount = 0;
                    return;
                }

                pathRenderer.positionCount = path.Count;
                for (int i = 0; i < path.Count; i++)
                {
                    pathRenderer.SetPosition(i, path[i] + new Vector3(0, -0.45f, 0));
                }

                if (viablePath) pathRenderer.colorGradient = validPathGradiant;
                else pathRenderer.colorGradient = nonvalidPathGradiant;
            }
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

        //Adds the node to the 'grid' if it's not there already.
        public Node CreateAndStoreNode(Vector3 pos)
        {
            if (nodeGrid.ContainsKey(pos)) return nodeGrid[pos];

            nodeGrid[pos] = CheckNodeConditions(new Node(pos, false, false));

            return nodeGrid[pos];
        }

        public void UpdateNodeCells(Vector3 minArea, Vector3 maxArea)
        {
            Vector3 minGrid = ToGridPos(minArea - Vector3.one);
            Vector3 maxGrid = ToGridPos(maxArea + Vector3.one);

            for (float xx = minGrid.x; xx <= maxGrid.x; xx++)
            {
                for (float yy = minGrid.y; yy <= maxGrid.y; yy++)
                {
                    for (float zz = minGrid.z; zz <= maxGrid.z; zz++)
                    {
                        UpdateGridCell(new Vector3(xx, yy, zz));
                    }
                }
            }
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

            min -= Vector3.one;
            max += Vector3.one;

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

        private Node CheckNodeConditions(Node node)
        {
            //Check for wall
            Collider[] wall = Physics.OverlapBox(node.Position + new Vector3(0, 0.25f, 0), new Vector3(0.33f, 0.11f, 0.33f), Quaternion.identity, groundMask, QueryTriggerInteraction.Collide);

            bool climbable = (Physics.OverlapBox(node.Position, new Vector3(0.5f, 0.75f, 0.5f), Quaternion.identity, climbableMask, QueryTriggerInteraction.Collide).Length > 0);
            bool belowClimbable = (Physics.OverlapBox(node.Position + Vector3.down, new Vector3(0.5f, 0.75f, 0.5f), Quaternion.identity, climbableMask, QueryTriggerInteraction.Collide).Length > 0);

            bool sloped = (Physics.OverlapBox(node.Position, new Vector3(0.45f, 0.45f, 0.45f), Quaternion.identity, slopeMask).Length > 0);
            bool water = (Physics.OverlapBox(node.Position, new Vector3(0.45f, 0.6f, 0.45f), Quaternion.identity, waterMask, QueryTriggerInteraction.Collide).Length > 0);

            //Add the node
            node.IsWall = wall.Length > 0;
            node.IsGround = false;
            node.IsWater = false;
            node.IsSlope = false;
            node.TemporalPosition = node.Position - new Vector3(0, 0.45f, 0);
            node.IsClimbable = climbable || belowClimbable;

            if (climbable)
            {
                CreateAndStoreNode(node.Position + Vector3.up);
            }

            //Check if ground
            if (!sloped && wall.Length == 0)
            {
                RaycastHit hit;
                if (Physics.Raycast(new Ray(node.Position, Vector3.down), out hit, 0.8f, groundMask))
                {
                    //Make sure it's not water
                    if (!(waterMask == (waterMask | (1 << hit.collider.gameObject.layer))))
                    {
                        node.IsGround = true;
                        node.TemporalPosition = node.Position - new Vector3(0, 0.45f, 0);
                    }
                }
            }

            if (!node.IsWall && !node.IsGround)
            {
                node.IsWater = water;
            }

            //Check if slope
            if (sloped)
            {
                RaycastHit hit;
                node.IsSlope = true;
                node.IsWall = false;
                if (Physics.Raycast(new Ray(node.Position + new Vector3(0, 0.5f, 0), Vector3.down), out hit, 1f, slopeMask))
                {
                    node.TemporalPosition = hit.point + new Vector3(0, 0.05f, 0);

                    Vector3 slopeDir = new Vector3(0, 0, 0);
                    if (Mathf.Abs(hit.normal.x) >= 0.5f) slopeDir.x = Mathf.Sign(hit.normal.x);
                    if (Mathf.Abs(hit.normal.z) >= 0.5f) slopeDir.z = Mathf.Sign(hit.normal.z);

                    node.slopeNormal = slopeDir;
                }
            }

            return node;
        }

        private void UpdateGridCell(Vector3 positon)
        {
            if (nodeGrid.ContainsKey(positon))
            {
                nodeGrid[positon] = CheckNodeConditions(nodeGrid[positon]);
            }
            else
            {
                CreateAndStoreNode(positon);
            }
        }

        private float GetManhattenDistance(Node nodeA, Node nodeB)
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

            //Regular get neighbor nodes.
            //Cardinals
            if (nodeGrid.ContainsKey(pos + Vector3.right)) adjacentNode.Add(nodeGrid[pos + Vector3.right]);
            if (nodeGrid.ContainsKey(pos + Vector3.left)) adjacentNode.Add(nodeGrid[pos + Vector3.left]);
            if (nodeGrid.ContainsKey(pos + Vector3.forward)) adjacentNode.Add(nodeGrid[pos + Vector3.forward]);
            if (nodeGrid.ContainsKey(pos + Vector3.back)) adjacentNode.Add(nodeGrid[pos + Vector3.back]);

            //Check for down slope
            if (nodeGrid.ContainsKey(pos + Vector3.down + Vector3.right)) adjacentNode.Add(nodeGrid[pos + Vector3.down + Vector3.right]);
            if (nodeGrid.ContainsKey(pos + Vector3.down + Vector3.left)) adjacentNode.Add(nodeGrid[pos + Vector3.down + Vector3.left]);
            if (nodeGrid.ContainsKey(pos + Vector3.down + Vector3.forward)) adjacentNode.Add(nodeGrid[pos + Vector3.down + Vector3.forward]);
            if (nodeGrid.ContainsKey(pos + Vector3.down + Vector3.back)) adjacentNode.Add(nodeGrid[pos + Vector3.down + Vector3.back]);

            //Check for up the slope
            if (current.IsSlope)
            {
                //Make sure we can only go up the slope on it's axis
                if (current.slopeNormal.x != 0)
                {
                    if (nodeGrid.ContainsKey(pos + Vector3.right + Vector3.up)) adjacentNode.Add(nodeGrid[pos + Vector3.right + Vector3.up]);
                    if (nodeGrid.ContainsKey(pos + Vector3.left + Vector3.up)) adjacentNode.Add(nodeGrid[pos + Vector3.left + Vector3.up]);

                    if (nodeGrid.ContainsKey(pos + Vector3.right + Vector3.down)) adjacentNode.Add(nodeGrid[pos + Vector3.right + Vector3.down]);
                    if (nodeGrid.ContainsKey(pos + Vector3.left + Vector3.down)) adjacentNode.Add(nodeGrid[pos + Vector3.left + Vector3.down]);
                }

                if (current.slopeNormal.z != 0)
                {
                    if (nodeGrid.ContainsKey(pos + Vector3.forward + Vector3.up)) adjacentNode.Add(nodeGrid[pos + Vector3.forward + Vector3.up]);
                    if (nodeGrid.ContainsKey(pos + Vector3.back + Vector3.up)) adjacentNode.Add(nodeGrid[pos + Vector3.back + Vector3.up]);

                    if (nodeGrid.ContainsKey(pos + Vector3.forward + Vector3.down)) adjacentNode.Add(nodeGrid[pos + Vector3.forward + Vector3.down]);
                    if (nodeGrid.ContainsKey(pos + Vector3.back + Vector3.down)) adjacentNode.Add(nodeGrid[pos + Vector3.back + Vector3.down]);
                }
            }

            if (canClimb)
            {
                if (nodeGrid.ContainsKey(pos)) adjacentNode.Add(nodeGrid[pos]);
                if (nodeGrid.ContainsKey(pos + Vector3.up)) adjacentNode.Add(nodeGrid[pos + Vector3.up]);
                if (nodeGrid.ContainsKey(pos + Vector3.down)) adjacentNode.Add(nodeGrid[pos + Vector3.down]);
            }

            return adjacentNode;
        }

        #endregion
        #region Pathfinding

        private List<Node> FindPath(Vector3 p1, Vector3 p2)
        {
            viablePath = false;

            List<Node> openList = new List<Node>();
            HashSet<Node> closedList = new HashSet<Node>();

            Node startNode = nodeGrid[p1];
            Node targetNode = nodeGrid[p2];

            isClimbing = false;

            //Debug.DrawRay(targetNode.Position + (Vector3.down * 0.5f), Vector3.up * 3, Color.yellow, 0.1f);

            openList.Add(startNode);

            if (openList[0] == null)
            {
                // Debug.Log("No!!! NOOOO");
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

                //Check if we are now Climbing
                if (canClimb && openList.Count >= 1)
                {
                    Node oldCurrent = openList[openList.Count - 1];

                    if (oldCurrent.Position.x == current.Position.x &&
                        oldCurrent.Position.z == current.Position.z &&
                        oldCurrent.Position.y != current.Position.y)
                    {
                        //We must be moving vertically
                        isClimbing = true;
                        UnityEngine.Debug.DrawRay(current.Position, Vector3.up, Color.red, 0.1f);
                    }
                    else
                    {
                        isClimbing = false;
                        UnityEngine.Debug.DrawRay(current.Position, Vector3.up, Color.blue, 0.1f);
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
                    //Check node conditions to make sure we can traverse though them.
                    if (closedList.Contains(neighborNode) || !CheckIfNodeIsViable(startNode, targetNode, current, neighborNode))
                    {
                        continue;
                    }

                    float moveCost = current.gCost + GetManhattenDistance(current, neighborNode);
                    if (neighborNode.IsWater && !canSwim) moveCost += 10;

                    //If were climbing or just not on the ground prefer ground
                    if (!neighborNode.IsGround && !neighborNode.IsSlope) moveCost += 5;

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

        private bool CheckIfNodeIsViable(Node startNode, Node endNode, Node current, Node neighborNode)
        {
            UpdateGridCell(neighborNode.Position);

            //Extra conditions
            if (extraNodeConditons != null)
            {
                if (!extraNodeConditons.Invoke(startNode, endNode, current, neighborNode))
                {
                    return false;
                }
            }

            //Skip node states
            if (neighborNode.IsWall || (!neighborNode.IsGround && !neighborNode.IsSlope && !neighborNode.IsClimbable && !neighborNode.IsWater))
            {
                return false;
            }

            //Check for dynamic blocks, players etc
            Collider[] dynamicBlock = Physics.OverlapBox(neighborNode.Position, Vector3.one * 0.25f, Quaternion.identity, dynamicPathBlockingMask);
            for (int i = 0; i < dynamicBlock.Length; i++)
            {
                if (dynamicBlock[i].gameObject.transform != owner)
                {
                    return false;
                }
            }

            //Make sure if it's water we can swim
            if (!canSwim && neighborNode.IsWater)
            {
                Collider[] dynamicGround = Physics.OverlapBox(neighborNode.Position + Vector3.down, new Vector3(.25f,.6f,.25f), Quaternion.identity, dynamicPathBlockingMask);
                if (dynamicGround.Length <= 0)
                {
                    return false;
                }
            }

            //Don't sample climbable blocks if they aren't ground level
            if (neighborNode.IsClimbable && !neighborNode.IsGround)
            {
                if (!canClimb) return false;
            }

            //Make sure we go on the slope the correct way
            if (neighborNode.IsSlope)
            {
                Vector3 dir = (neighborNode.Position - current.Position);
                dir.y = 0;
                dir = dir.normalized;
                if (dir != neighborNode.slopeNormal && dir != -neighborNode.slopeNormal) return false;
            }
            else
            {
                //Stepping down one tile
                if (neighborNode.Position.y < current.Position.y)
                {
                    if (!neighborNode.IsGround)
                    {
                        if (canClimb && neighborNode.IsClimbable) return true;
                        else return false;
                    }
                    else
                    {
                        //Climb down if can
                        if (isClimbing)
                        {
                            //Make sure we move vertically to the ground
                            if (current.Position.x != neighborNode.Position.x
                                || current.Position.z != neighborNode.Position.z)
                            {
                                //return false;
                            }
                        }
                    }
                }
            }

            //Make sure we get off the slope the correct way
            if (current.IsSlope)
            {
                Vector3 dir = (neighborNode.Position - current.Position);
                dir.y = 0;
                dir = dir.normalized;
                if (dir != current.slopeNormal && dir != -current.slopeNormal) return false;
            }

            if (isClimbing) //Climbing
            {
                //Only go from climbing to ground if it's the target node
                CreateAndStoreNode(neighborNode.Position + Vector3.down);

                //Check if were are moving on the x / Z
                if (neighborNode.Position.x != current.Position.x ||
                    neighborNode.Position.z != current.Position.z)
                {
                    if (!neighborNode.IsGround)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private List<Node> GetFinalPath(Node startNode, Node targetNode)
        {
            List<Node> finalPath = new List<Node>();
            Node currentNode = targetNode;

            bool viable = true;

            while (currentNode != startNode)
            {
                finalPath.Add(currentNode);
                currentNode = currentNode.Parent;
                if (currentNode.IsWater && !canSwim && currentNode != startNode) viable = false;
            }

            finalPath.Reverse();
            viablePath = viable;

            return finalPath;
        }

        private void OnDrawGizmos()
        {
            foreach (var node in nodeGrid)
            {
                if (node.Value.IsClimbable)
                {
                    Gizmos.color = Color.blue * 0.2f;
                    //Gizmos.DrawCube(node.Value.Position, Vector3.one);
                }
            }
        }

        #endregion
    }
}