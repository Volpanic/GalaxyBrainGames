using GalaxyBrain.Creatures.Abilities;
using GalaxyBrain.Interactables;
using GalaxyBrain.Pathfinding;
using GalaxyBrain.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GalaxyBrain.Creatures
{
    [System.Serializable, SelectionBase]
    public class PlayerController : MonoBehaviour
    {
        public enum PlayerTypes
        {
            Child,
            Water,
            Strong
        }

        [Header("Movement")]
        [SerializeField, Min(0.1f)] private float movementSpeed = 1;
        [SerializeField] private float rotateSpeed = 1;

        [Header("References")]
        [SerializeField] private CharacterController controller;
        [SerializeField] private ActionPointData actionPointData;

        [Header("Abilities")]
        [SerializeField] private PlayerTypes playerType;
        [SerializeField] private bool canClimb;
        [SerializeField] private bool canSwim;

        [SerializeField] private GridPathfinding pathfinding;

        [HideInInspector] public bool Selected = false;
        [HideInInspector] public bool IsClimbing = false;

        public bool Grounded
        {
            get { return controller.isGrounded; }
        }

        public PlayerTypes PlayerType
        {
            get
            {
                return playerType;
            }
        }



        private List<Vector3> path;
        private bool manualMove = false;

        //Moving Along Path
        private bool moving = false;
        private bool canMove = true;
        private float moveTimer = 0;
        private float moveMaxTime = 0;
        private int currentPathIndex = 0;

        private List<ICreatureAbility> abilites = new List<ICreatureAbility>();
        private int currentRunningAbility = -1;

        private Quaternion targetRotation;

        public void AddAbility(ICreatureAbility ability)
        {
            if (abilites != null) abilites.Add(ability);
        }

        private void Awake()
        {
            transform.position = pathfinding.ToGridPos(transform.position);
            targetRotation = transform.rotation;

            if (pathfinding != null && controller != null)
            {
                controller.enabled = false;

                Vector3 gridPos = pathfinding.ToGridPos(transform.position);
                transform.position = new Vector3(gridPos.x, transform.position.y, gridPos.z);

                controller.enabled = true;
            }
        }

        private void Update()
        {

            if (currentRunningAbility < 0)
            {
                if (Selected)
                {
                    pathfinding.SetOwner(transform, moving, canClimb && IsClimbing, canSwim);

                    if (!moving) MovementSelection();
                }

                if (moving && canMove) MoveAlongPath();
                else
                {
                    if (!manualMove)
                    {
                        controller.SimpleMove(Vector3.zero);
                    }
                }
            }
            else
            {
                abilites[currentRunningAbility].OnAbilityUpdate();
                if (abilites[currentRunningAbility].OnAbilityCheckDone())
                {
                    abilites[currentRunningAbility].OnAbilityEnd();
                    currentRunningAbility = -1;
                }
            }

            manualMove = false;
            transform.rotation = UpdateRotation(transform.rotation, targetRotation);
        }

        private Quaternion UpdateRotation(Quaternion current, Quaternion target)
        {
            //Rotate towards target rotation, rotate speed * 1440 (being 360*4)
            return Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * (rotateSpeed * 1440f));
        }

        private Quaternion GetDirectionOfMovement()
        {
            if (controller.velocity.x == 0 && controller.velocity.z == 0) return targetRotation;
            Vector3 lookAt = controller.velocity + transform.position;
            lookAt.y = transform.position.y;

            return Quaternion.LookRotation(lookAt - transform.position);
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
            Vector3 velocity = transitionalPos - transform.position;

            controller.Move(velocity);
            targetRotation = GetDirectionOfMovement();

            if (moveTimer >= moveMaxTime)
            {
                currentPathIndex++;
                actionPointData?.SubtractActionPoint(1);
                moveTimer = 0;

                // Stop if we wan't to move the player manually
                // or we've reached the end of the path
                if (manualMove || currentPathIndex + 1 >= path.Count)
                {
                    StopMoveAlongPath();
                }
            }
        }

        private void StopMoveAlongPath()
        {
            moving = false;
            IsClimbing = false;
            moveTimer = 0;
            currentPathIndex = 0;

            SnapToGridPosition();
        }

        private void SnapToGridPosition()
        {
            Vector3 snapPos = pathfinding.ToGridPos(transform.position);
            snapPos.y = transform.position.y;

            controller.Move(transform.position - snapPos);
        }

        private void MovementSelection()
        {
            controller.SimpleMove(Vector3.zero);
            if (pathfinding == null) return;

            if (Input.GetMouseButtonDown(0))
            {
                StartMoveAlongPath(pathfinding.GetPath());
            }
        }

        private void StartMoveAlongPath(List<Vector3> path)
        {
            //Make sure we have a path with 2 points
            if (path != null && path.Count >= 2)
            {
                moving = true;
                moveMaxTime = movementSpeed;
                moveTimer = 0;
                currentPathIndex = 0;
                this.path = path;
            }
        }

        public float CorrectYPos(float y)
        {
            return y + controller.bounds.extents.y;
        }

        private void OnDrawGizmos()
        {
            if (pathfinding != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(pathfinding.ToGridPos(transform.position), Vector3.one);
            }
        }

        public void AttemptInteract(Interactalbe interact)
        {
            for (int i = 0; i < abilites.Count; i++)
            {
                if (abilites[i].OnAbilityCheckCondition(interact))
                {
                    Vector3 pos = interact.transform.position;
                    Vector3 pos2 = controller.transform.position;

                    pos.y = 0;
                    pos2.y = 0;

                    Vector3 interactDirection = (pos - pos2).normalized;
                    abilites[i].OnAbilityStart(this, interact, MakeCardinal(interactDirection));
                    currentRunningAbility = i;
                    break;
                }
            }
        }

        public void ShiftPlayer(Vector3 offset)
        {
            if (!moving)
            {
                List<Vector3> targetList = new List<Vector3>();

                targetList.Add(pathfinding.ToGridPos(transform.position));
                targetList.Add(pathfinding.ToGridPos(transform.position) + offset);

                StartMoveAlongPath(targetList);
            }
            manualMove = true;
        }

        public void LockMovement()
        {
            canMove = false;
        }

        private Vector3 MakeCardinal(Vector3 direction)
        {
            float absX = Mathf.Abs(direction.x);
            float absY = Mathf.Abs(direction.y);
            float absZ = Mathf.Abs(direction.z);

            if (absX > absY && absX > absZ) return new Vector3(Mathf.Sign(direction.x), 0, 0);
            if (absY > absX && absY > absZ) return new Vector3(0, Mathf.Sign(direction.y), 0);
            if (absZ > absX && absZ > absY) return new Vector3(0, 0, Mathf.Sign(direction.z));

            return Vector3.zero;
        }
    }
}