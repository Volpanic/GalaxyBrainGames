using UnityEngine;
using GalaxyBrain.Pathfinding;
using System;

namespace GalaxyBrain.Creatures.States
{
    /// <summary>
    /// The players default state, handles finding a path to eventually move 
    /// on in the pathfind state, also prevents the water creature from
    /// leaving water if they're carrying a charecter controller;
    /// </summary>
    public class PlayerDefaultState : PlayerState
    {
        private GridPathfinding pathfinding;
        private LayerMask waterLayer;
        private bool waterSnap = false;

        public PlayerDefaultState(PlayerController controller, GridPathfinding pathfinding, LayerMask waterLayer) : base(controller)
        {
            this.pathfinding = pathfinding;
            this.waterLayer = waterLayer;
        }

        public override void OnStateStart()
        {
            waterSnap = false;
        }

        public override void OnStateUpdate()
        {
            if (!SubmergedInWater())
            {
                controller.Controller.SimpleMove(Vector3.zero);
                waterSnap = false;
            }
            else
            {
                SnapToCorrectWaterPosition();
            }

            if (controller.Selected && !machine.LockState)
            {
                pathfinding.SetOwner(controller.transform, controller.CanClimb, controller.CanSwim, ExtraNodeConditions);
                MovementSelection();
            }
        }

        private void SnapToCorrectWaterPosition()
        {
            if(!waterSnap)
            {
                if (controller.PlayerType == PlayerController.PlayerTypes.Water)
                {
                    Vector3 targetPos = pathfinding.ToGridPos(controller.transform.position + pathfinding.SearchPointOffset);
                    targetPos += Vector3.down * pathfinding.WATER_FLOAT_POINT;

                    controller.Controller.enabled = false;
                    controller.transform.position = targetPos;
                    controller.Controller.enabled = true;
                }

                waterSnap = true;
            }
        }

        private bool SubmergedInWater()
        {
            Vector3 extents = controller.ColliderBounds.extents;
            return Physics.CheckBox(controller.Controller.bounds.center, extents, controller.transform.rotation,
                waterLayer, QueryTriggerInteraction.Collide);
        }

        private bool ExtraNodeConditions(Node startNode, Node endNode, Node current, Node neighborNode)
        {
            if (controller.WeighedDown && neighborNode.IsGround)
            {
                return false;
            }

            return true;
        }

        private void MovementSelection()
        {
            if (pathfinding == null) return;

            if (controller.LeftClicked)
            {
                //Change to pathfinding state
                controller.StartMoveAlongPath(pathfinding.GetPath(), true);
            }
        }
    }
}
