using UnityEngine;
using GalaxyBrain.Pathfinding;

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
            return Physics.CheckBox(controller.Controller.bounds.center, extents, controller.transform.rotation,
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

            if (controller.LeftClicked)
            {
                //Change to pathfinding state
                controller.StartMoveAlongPath(pathfinding.GetPath(), true);
            }
        }
    }
}
