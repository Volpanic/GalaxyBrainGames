using GalaxyBrain.Assets.Scripts.CharecterController.CreatureAbilites;
using GalaxyBrain.Creatures.Abilities;
using GalaxyBrain.Creatures.States;
using GalaxyBrain.Interactables;
using GalaxyBrain.Pathfinding;
using GalaxyBrain.Systems;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyBrain.Creatures
{
    /// <summary>
    /// Controls the grid based movement through the games scene
    /// </summary>
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
        [SerializeField] private Animator animator;

        [SerializeField] private Color selectionIconTint = Color.white;

        [SerializeField] Sprite creatureIcon;

        [HideInInspector] public bool Selected = false;
        [HideInInspector] public bool IsClimbing = false;
        [HideInInspector] public bool InteruptNextPathInterval = false;

        //Controls when we can leave water when in it
        [HideInInspector] public bool WeighedDown = false;

        public event Action<Vector3, Vector3> OnPathInterval;
        private Quaternion targetRotation;
        private bool leftClicked  = false;
        private bool rightClicked = false;

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

        public bool LeftClicked
        {
            get { return leftClicked; }
        }

        public bool RightClicked
        {
            get { return rightClicked; }
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

        public bool InDefaultState
        {
            get { return stateMachine.InDefaultState; }
        }

        public Animator Animator
        {
            get { return animator; }
        }

        public GridPathfinding Pathfinding
        {
            get { return pathfinding; }
        }

        public Color SelectionIconTint
        {
            get { return selectionIconTint; }
        }

        public Sprite CreatureIcon
        {
            get { return creatureIcon; }
        }

        private PlayerStateMachine stateMachine;
        private PlayerDefaultState defaultState;
        private PlayerPathfindState pathfindState;
        private PlayerAbilityState abilityState;

        private AnimationEventAbility animationEventAbility;

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

            animationEventAbility = new AnimationEventAbility();
            AddAbility(animationEventAbility);
        }

        public void AddAbility(ICreatureAbility ability)
        {
            abilityState.AddAbility(ability);
        }

        private void Update()
        {
            if (!leftClicked) leftClicked = Input.GetMouseButtonDown(0);
            if (!rightClicked) rightClicked = Input.GetMouseButtonDown(1);
        }

        private void FixedUpdate()
        {
            // If timescale is 0 don't run any code
            // Most likely game is paused if timescale is 0
            if (Time.timeScale == 0) return;

            stateMachine.UpdateState();

            transform.rotation = UpdateRotation(transform.rotation, targetRotation);

            leftClicked = false;
            rightClicked = false;
        }

        public Quaternion UpdateRotation(Quaternion current, Quaternion target)
        {
            //Rotate towards target rotation, rotate speed * 1440 (being 360*4)
            return Quaternion.RotateTowards(transform.rotation, targetRotation, Time.fixedDeltaTime * (rotateSpeed * 1440f));
        }

        public Quaternion GetDirectionOfMovement()
        {
            if (controller.velocity.x == 0 && controller.velocity.z == 0) return targetRotation;
            Vector3 lookAt = controller.velocity + transform.position;
            lookAt.y = transform.position.y;

            return Quaternion.LookRotation(lookAt - transform.position);
        }

        public Quaternion GetRotationOfDirection(Vector3 direction)
        {
            if (direction.x == 0 && direction.z == 0) return targetRotation;
            Vector3 lookAt = direction + transform.position;
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
                List<PathNodeInfo> targetList = new List<PathNodeInfo>();

                PathNodeInfo startNode = new PathNodeInfo(pathfinding.CreateAndStoreNode(pathfinding.ToGridPos(transform.position + pathfinding.SearchPointOffset)), false, false, false);
                PathNodeInfo endNode = new PathNodeInfo(pathfinding.CreateAndStoreNode(pathfinding.ToGridPos(transform.position + pathfinding.SearchPointOffset) + offset), false, false, false);

                if (startNode.ReferenceNode.IsWater && canSwim)
                {
                    startNode.IsSwimming = true;
                    startNode.Offset += Vector3.down * pathfinding.WATER_FLOAT_POINT;
                }

                if (endNode.ReferenceNode.IsWater && canSwim)
                {
                    endNode.IsSwimming = true;
                    endNode.Offset += Vector3.down * pathfinding.WATER_FLOAT_POINT;
                }

                targetList.Add(startNode);
                targetList.Add(endNode);

                StartMoveAlongPath(targetList, false);
            }
            else
            {
                InteruptNextPathInterval = true;
            }
        }

        public void MoveToTarget(Vector3 target)
        {
            List<PathNodeInfo> targetList = new List<PathNodeInfo>();

            PathNodeInfo startNode = new PathNodeInfo(pathfinding.CreateAndStoreNode(pathfinding.ToGridPos(transform.position)), false, false, false);
            PathNodeInfo endNode = new PathNodeInfo(pathfinding.CreateAndStoreNode(pathfinding.ToGridPos(target)), false, false, false);

            targetList.Add(startNode);
            targetList.Add(endNode);

            StartMoveAlongPath(targetList, false);
        }

        public void ConsumeActionPoint(int amount = 1)
        {
            actionPointData?.SubtractActionPoint(transform.position,amount);
        }

        public void StartMoveAlongPath(List<PathNodeInfo> targetList, bool consumeActionPoints)
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

        public void AnimationEvent(Interactalbe interaction,string boolName, float normalizedTimeForEvent, Action<PlayerController> onEvent)
        {
            animationEventAbility.SetEventInfo(boolName,normalizedTimeForEvent,onEvent);
            abilityState.ForceAbility(interaction,animationEventAbility);

            Vector3 pos = interaction.transform.position;
            Vector3 pos2 = transform.position;

            pos.y = 0;
            pos2.y = 0;

            Vector3 interactDirection = (pos - pos2).normalized;

            TargetRotation = GetRotationOfDirection(interactDirection);
        }

        public void HardPlayAnimationEvent(Interactalbe interaction, string animationName, float normalizedTimeForEvent, Action<PlayerController> onEvent)
        {
            animationEventAbility.SetEventInfoHard(animationName, normalizedTimeForEvent, onEvent);
            abilityState.ForceAbility(interaction, animationEventAbility);

            Vector3 pos = interaction.transform.position;
            Vector3 pos2 = transform.position;

            pos.y = 0;
            pos2.y = 0;

            Vector3 interactDirection = (pos - pos2).normalized;

            TargetRotation = GetRotationOfDirection(interactDirection);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(pathfinding.ToGridPos(transform.position), Vector3.one);
        }
    }
}