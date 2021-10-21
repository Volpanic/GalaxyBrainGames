using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyBrain.Pathfinding
{
    public struct PathNodeInfo
    {
        public Node ReferenceNode;

        public bool IsClimbing;
        public bool IsSwimming;
        public bool ConsumePoint;

        public Vector3 Offset;

        #region
        public Vector3 Position
        {
            get { return ReferenceNode.Position + Offset; }
        }
        #endregion

        public PathNodeInfo(Node referenceNode, bool isClimbing = false, bool isSwiming = false, bool consumePoint = false)
        {
            ReferenceNode = referenceNode;

            IsClimbing = isClimbing;
            IsSwimming = isSwiming;
            ConsumePoint = consumePoint;

            Offset = Vector3.zero;
        }

        public PathNodeInfo(Node referenceNode,Vector3 offset, bool isClimbing = false, bool isSwiming = false, bool consumePoint = false)
        {
            ReferenceNode = referenceNode;

            IsClimbing = isClimbing;
            IsSwimming = isSwiming;
            ConsumePoint = consumePoint;

            Offset = offset;
        }
    }
}