using GalaxyBrain.Creatures.Abilities;
using GalaxyBrain.Creatures.States;
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
        [SerializeField] private LayerMask waterLayer;

        [SerializeField] private GridPathfinding pathfinding;

        [HideInInspector] public bool Selected = false;
        [HideInInspector] public bool IsClimbing = false;
        [HideInInspector] public bool InteruptNextPathInterval = false;

        //Controls when we can leave water when in it
        [HideInInspector] public bool WeighedDown = false;

        public event Action<Vector3, Vector3> OnPathInterval;
        private Quaternion targetRotation;

        public bool CanClimb { get { return canClimb; } }
        public bool CanSwim { get { return canSwim; } }

        public Quaternion TargetRotation
        {
            get { return targetRotation; }
            set { targetRotation = value; }
        }

        public bool Grounded
        {
            get { return controller.isGrounded; }
        }

        public CharacterController Controller
        {
            get { return controller; }
        }

        public PlayerTypes PlayerType
        {
            get
            {
                return playerType;
            }
        }

        public Bounds ColliderBounds
        {
            get
            {
                return controller.bounds;
            }
        }

        private PlayerStateMachine stateMachine;
        private PlayerDefaultState defaultState;
        private PlayerPathfindState pathfindState;
        private PlayerAbilityState abilityState;

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

            //Create States
            defaultState = new PlayerDefaultState(this, pathfinding,waterLayer);
            pathfindState = new PlayerPathfindState(this, movementSpeed);
            abilityState = new PlayerAbilityState(this);

            stateMachine = new PlayerStateMachine(defaultState);
            stateMachine.AddState(pathfindState);
            stateMachine.AddState(abilityState);
        }

        public void AddAbility(ICreatureAbility ability)
        {
            abilityState.AddAbility(ability);
        }

        private void Update()
        {
            stateMachine.UpdateState();

            transform.rotation = UpdateRotation(transform.rotation, targetRotation);
        }

        public Quaternion UpdateRotation(Quaternion current, Quaternion target)
        {
            //Rotate towards target rotation, rotate speed * 1440 (being 360*4)
            return Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * (rotateSpeed * 1440f));
        }

        public Quaternion GetDirectionOfMovement()
        {
            if (controller.velocity.x == 0 && controller.velocity.z == 0) return targetRotation;
            Vector3 lookAt = controller.velocity + transform.position;
            lookAt.y = transform.position.y;

            return Quaternion.LookRotation(lookAt - transform.position);
        }

        public void AttemptInteract(Interactalbe interact)
        {
            if (stateMachine.InDefaultState)
            {
                if(abilityState.AttemptInteract(interact))
                {
                    //Check again in case the interact changes state
                    if (stateMachine.InDefaultState)
                    {
                        stateMachine.ChangeState(typeof(PlayerAbilityState));
                    }
                }
            }
        }

        public void ShiftPlayer(Vector3 offset)
        {
            if (stateMachine.InDefaultState)
            {
                List<Vector3> targetList = new List<Vector3>();

                targetList.Add(pathfinding.ToGridPos(transform.position));
                targetList.Add(pathfinding.ToGridPos(transform.position) + offset);

                StartMoveAlongPath(targetList, false);
            }
            else
            {
                InteruptNextPathInterval = true;
            }
        }

        public void MoveToTarget(Vector3 target)
        {
            List<Vector3> targetList = new List<Vector3>();

            targetList.Add(pathfinding.ToGridPos(transform.position));
            targetList.Add(pathfinding.ToGridPos(target));

            StartMoveAlongPath(targetList, false);
        }

        public void ConsumeActionPoint(int amount = 1)
        {
            actionPointData?.SubtractActionPoint(amount);
        }

        public void StartMoveAlongPath(List<Vector3> targetList, bool consumeActionPoints)
        {
            if (targetList == null || targetList.Count < 2) return;

            InteruptNextPathInterval = false;
            pathfindState.SetPath(targetList.ToArray());
            pathfindState.ConsumeActionPoints = consumeActionPoints;
            stateMachine.ChangeState(typeof(PlayerPathfindState));
        }

        public void PathInterval(Vector3 previousGridCell, Vector3 nextGridCell)
        {
            OnPathInterval?.Invoke(previousGridCell, nextGridCell);
        }

        public void LockMovement()
        {
            stateMachine.ChangeToDefaultState();
            stateMachine.LockState = true;
        }
    }
}