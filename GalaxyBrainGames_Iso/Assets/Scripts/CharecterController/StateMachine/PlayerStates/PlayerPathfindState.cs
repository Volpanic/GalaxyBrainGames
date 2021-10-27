using GalaxyBrain.Pathfinding;
using UnityEngine;

namespace GalaxyBrain.Creatures.States
{
    /// <summary>
    /// Moves along the a desired path, then returns
    /// to the default state when compelted.
    /// </summary>
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

        private PathNodeInfo[] path;
        private float moveTimer = 0;
        private int currentPathIndex = 0;
        private float moveMaxTime = 0;
        private bool consumeActionPoints = false;
        //private ActionPointData actionPointData;

        private const float TURN_FORGIVENESS = 0.98f;
        private const float CLIMB_SPEED_MODIFIER = 2f;

        public bool ConsumeActionPoints
        {
            get { return consumeActionPoints; }
            set { consumeActionPoints = value; }
        }

        public override void OnStateStart()
        {
            moveTimer = 0;
            currentPathIndex = 0;
            controller.Animator.SetBool("Walk", true);
        }

        public override void OnStateEnd()
        {
            controller.Animator.SetBool("Walk",false);

            if (controller.PlayerType == PlayerController.PlayerTypes.Child)
                controller.Animator.SetBool("Climb",false);
        }

        public override void OnStateUpdate()
        {
            MoveAlongPath();
        }

        public void SetPath(PathNodeInfo[] path)
        {
            this.path = path;
        }

        private void MoveAlongPath()
        {
            //Don't move horizontally if we're rotating to face a new direction
            if (Quaternion.Dot(controller.TargetRotation, controller.transform.rotation) < TURN_FORGIVENESS) return;

            //Make sure index is in array
            if (currentPathIndex + 1 >= path.Length)
            {
                controller.PathInterval(path[currentPathIndex - 1].Position, path[currentPathIndex].Position);
                StopMoveAlongPath();
                return;
            }

            //Move the player along the path
            moveTimer += Time.deltaTime;

            // Get current point and the next point on path
            Vector3 oldPos = path[currentPathIndex].Position;
            Vector3 targetPos = path[currentPathIndex + 1].Position;

            //ClimbAnimation
            if(controller.PlayerType == PlayerController.PlayerTypes.Child)
                controller.Animator.SetBool("Climb", path[currentPathIndex].IsClimbing);

            //Lerp between the two points
            float moveTime = moveMaxTime;
            if (path[currentPathIndex].IsClimbing) moveTime *= CLIMB_SPEED_MODIFIER;
            Vector3 transitionalPos = Vector3.Lerp(oldPos, targetPos, moveTimer / moveTime);

            //Find how much we need to move to get to that point
            Vector3 velocity = transitionalPos - controller.transform.position;

            controller.Controller.Move(velocity);
            controller.TargetRotation = controller.GetDirectionOfMovement();

            if (moveTimer >= moveTime)
            {
                if (consumeActionPoints && path[currentPathIndex + 1].ConsumePoint) controller.ConsumeActionPoint(1);
                currentPathIndex++;

                moveTimer = 0;

                // Stop if we want to move the player manually
                // or we've reached the end of the path
                if (currentPathIndex + 1 >= path.Length)
                {
                    controller.PathInterval(path[currentPathIndex - 1].Position, path[currentPathIndex].Position);
                    StopMoveAlongPath();
                }
                else
                {
                    controller.PathInterval(path[currentPathIndex - 1].Position, path[currentPathIndex].Position);

                    Debug.DrawRay(path[currentPathIndex].Position,Vector3.up*4,Color.red,0.1f);
                    if (!QuickNodeCheck(path[currentPathIndex + 1].Position, path[currentPathIndex].Position))
                    {
                        StopMoveAlongPath();
                    }
                }

                if(controller.InteruptNextPathInterval)
                {
                    machine.ChangeToDefaultState();
                    controller.InteruptNextPathInterval = false;
                }
            }
        }

        private bool QuickNodeCheck(Vector3 targetPos,Vector3 currentPos)
        {
            return controller.Pathfinding.CheckNode(targetPos, currentPos);
        }

        private void StopMoveAlongPath()
        {
            moveTimer = 0;
            currentPathIndex = 0;
            Machine.ChangeToDefaultState();
        }
    }
}
