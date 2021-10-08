using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyBrain.Creatures.States
{
    public class PlayerPathfindState : PlayerState
    {
        public PlayerPathfindState(PlayerController controller, float moveSpeed) : base(controller)
        {
            moveMaxTime = moveSpeed;
        }

        /// <summary>
        /// Called when a point in the path has been reached,
        /// Most likely in the center of a grid cell
        /// v3 = PreviousGridCellPos, v3 = new nextGridCellpos
        /// </summary>

        private Vector3[] path;
        private float moveTimer = 0;
        private int currentPathIndex = 0;
        private float moveMaxTime = 0;
        private bool consumeActionPoints = false;
        //private ActionPointData actionPointData;

        public bool ConsumeActionPoints
        {
            get { return consumeActionPoints; }
            set { consumeActionPoints = value; }
        }

        public override void OnStateStart()
        {
            moveTimer = 0;
            currentPathIndex = 0;
        }

        public override void OnStateUpdate()
        {
            MoveAlongPath();
        }

        public void SetPath(Vector3[] path)
        {
            this.path = path;
        }

        private void MoveAlongPath()
        {
            //Move the player along the path
            moveTimer += Time.deltaTime;

            // Get current point and the next point on path
            Vector3 oldPos = path[currentPathIndex];
            Vector3 targetPos = path[currentPathIndex + 1];

            //Lerp between the two points
            Vector3 transitionalPos = Vector3.Lerp(oldPos, targetPos, moveTimer / moveMaxTime);

            //Find how much we need to move to get to that point
            Vector3 velocity = transitionalPos - controller.transform.position;

            controller.Controller.Move(velocity);
            controller.TargetRotation = controller.GetDirectionOfMovement();

            if (moveTimer >= moveMaxTime)
            {
                currentPathIndex++;
                if (consumeActionPoints) controller.ConsumeActionPoint(1);
                moveTimer = 0;

                // Stop if we wan't to move the player manually
                // or we've reached the end of the path
                if (currentPathIndex + 1 >= path.Length)
                {
                    controller.PathInterval(path[currentPathIndex - 1], path[currentPathIndex]);
                    StopMoveAlongPath();
                }
                else
                {
                    controller.PathInterval(path[currentPathIndex - 1], path[currentPathIndex]);
                }

                if(controller.InteruptNextPathInterval)
                {
                    machine.ChangeToDefaultState();
                    controller.InteruptNextPathInterval = false;
                }
            }
        }

        private void StopMoveAlongPath()
        {
            moveTimer = 0;
            currentPathIndex = 0;
            Machine.ChangeToDefaultState();
        }
    }
}
