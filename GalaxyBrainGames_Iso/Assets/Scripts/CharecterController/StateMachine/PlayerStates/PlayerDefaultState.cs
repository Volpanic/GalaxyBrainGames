using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GalaxyBrain.Pathfinding;
using System;

namespace GalaxyBrain.Creatures.States
{
    public class PlayerDefaultState : PlayerState
    {
        private GridPathfinding pathfinding;
        private LayerMask waterLayer;

        public PlayerDefaultState(PlayerController controller, GridPathfinding pathfinding, LayerMask waterLayer) : base(controller)
        {
            this.pathfinding = pathfinding;
            this.waterLayer = waterLayer;
        }

        public override void OnStateUpdate()
        {
            if (!SubmergedInWater())
            {
                controller.Controller.SimpleMove(Vector3.zero);
            }

            if (controller.Selected && !machine.LockState)
            {
                pathfinding.SetOwner(controller.transform, controller.CanClimb, controller.CanSwim, ExtraNodeConditions);
                MovementSelection();
            }
        }

        private bool SubmergedInWater()
        {
            Vector3 extents = controller.ColliderBounds.extents;
            return Physics.CheckBox(controller.Controller.bounds.center + (Vector3.up * extents.y * 2f), extents, controller.transform.rotation,
                waterLayer, QueryTriggerInteraction.Collide);
        }

        private bool ExtraNodeConditions(Node startNode, Node endNode, Node current, Node neighborNode)
        {
            if (controller.WeighedDown && !neighborNode.IsWater)
            {
                return false;
            }

            return true;
        }

        private void MovementSelection()
        {
            if (pathfinding == null) return;

            if (Input.GetMouseButtonDown(0))
            {
                //Change to pathfinding state
                controller.StartMoveAlongPath(pathfinding.GetPath(), true);
            }
        }
    }
}
