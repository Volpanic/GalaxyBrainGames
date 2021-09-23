using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GalaxyBrain.Pathfinding;

namespace GalaxyBrain.Creatures.States
{
    public class PlayerDefaultState : PlayerState
    {
        private GridPathfinding pathfinding;

        public PlayerDefaultState(PlayerController controller, GridPathfinding pathfinding) : base(controller)
        {
            this.pathfinding = pathfinding;
        }

        public override void OnStateUpdate()
        {
            controller.Controller.SimpleMove(Vector3.zero);

            if (controller.Selected && !machine.LockState)
            {
                pathfinding.SetOwner(controller.transform, controller.CanClimb, controller.CanSwim, ExtraNodeConditions);
                MovementSelection();
            }
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
                controller.StartMoveAlongPath(pathfinding.GetPath(),true);
            }
        }
    }
}
