using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyBrain.Pathfinding
{
    public class Node
    {
        public bool IsWall;
        public bool IsGround;
        public bool IsSlope = false;
        public bool IsClimbable = false;
        public bool IsWater = false;

        public Vector3 Position;
        public Vector3 TemporalPosition;
        public Vector3 slopeNormal = Vector3.zero;

        public Node Parent;

        // Climbing Direction
        // Corresponds to the Vector3.directions
        // Could probably be cleaner with some bitwise operations on an int
        // But should be less memory this way?
        public bool canClimbLeft    = false; // Vector3.left
        public bool canClimbRight   = false; // Vector3.right
        public bool canClimbForward = false; // Vector3.forward
        public bool canClimbBack    = false; // Vector3.back
        
        public float gCost;
        public float hCost;

        public float FCost { get { return gCost + hCost; } }

        public Node(Vector3 gridPos, bool isWall, bool isGround)
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

        public bool CanClimbDirection(Vector3 direction)
        {
            if (direction.y != 0) return false;

            //Has to be if statements, Can't switch statement with Vector3
            if(direction == Vector3.right)
            {
                if (canClimbRight) return true;
                return false;
            }

            if (direction == Vector3.left)
            {
                if (canClimbLeft) return true;
                return false;
            }

            if (direction == Vector3.forward)
            {
                if (canClimbForward) return true;
                return false;
            }

            if (direction == Vector3.back)
            {
                if (canClimbBack) return true;
                return false;
            }

            return false;
        }

        public void SetClimbDirection(Vector3 direction, bool value)
        {
            if (direction.y != 0) return;

            //Has to be if statements, Can't switch statement with Vector3
            if (direction == Vector3.right)
            {
                canClimbRight = value;
                return;
            }

            if (direction == Vector3.left)
            {
                canClimbLeft = value;
                return;
            }

            if (direction == Vector3.forward)
            {
                canClimbForward = value;
                return;
            }

            if (direction == Vector3.back)
            {
                canClimbBack = value;
                return;
            }
        }
    }
}
