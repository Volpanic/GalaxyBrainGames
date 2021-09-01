using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable,SelectionBase]
public class PlayerController : MonoBehaviour
{
    public enum PlayerTypes
    {
        Child, 
        Water,
        Strong
    }

    [SerializeField,Min(0.1f)] private float movementSpeed = 1;
    [SerializeField] private CharacterController controller;
    [SerializeField] private ActionPointData actionPointData;

    [Header("Abilities")]
    [SerializeField] public PlayerTypes PlayerType;
    [SerializeField] bool canClimb;
    [SerializeField] bool canSwim;

    [SerializeField] GridPathfinding pathfinding;

    [HideInInspector] public bool Selected = false;


    public bool Grounded
    {
        get { return controller.isGrounded; }
    }

    private Camera cam;
    private bool moving = false;
    [HideInInspector] public bool IsClimbing = false;
    private Vector3 targetPos = Vector3.zero;
    private Vector3 startPos = Vector3.zero;
    private float moveTimer = 0;
    private float moveMaxTime = 0;
    private List<Vector3> path;

    private List<ICreatureAbility> abilites = new List<ICreatureAbility>();
    private int currentRunningAbility = -1;

    public void AddAbility(ICreatureAbility ability)
    {
        if (abilites != null) abilites.Add(ability);
    }

    private void Awake()
    {
        cam = Camera.main;

        transform.position = pathfinding.ToGridPos(transform.position);

        if(pathfinding != null && controller != null)
        {
            controller.enabled = false;

            Vector3 gridPos = pathfinding.ToGridPos(transform.position);
            transform.position = new Vector3(gridPos.x, transform.position.y , gridPos.z);

            controller.enabled = true;

        }
    }

    private void Update()
    {
        if (currentRunningAbility < 0)
        {
            if (Selected)
            {
                if (!moving) MovementSelection();
            }
            if (moving) MoveAlongPath();
            else controller.SimpleMove(Vector3.zero);
        }
        else
        {
            abilites[currentRunningAbility].OnAbilityUpdate();
            if(abilites[currentRunningAbility].OnAbilityCheckDone())
            {
                abilites[currentRunningAbility].OnAbilityEnd();
                currentRunningAbility = -1;
            }
        }
    }

    private void MoveAlongPath()
    {
        moveTimer += Time.deltaTime;
        Vector3 targetPos = SamplePath(path, moveTimer / moveMaxTime);

        Vector3 velocity = targetPos - transform.position;

        controller.Move(velocity);

        if(moveTimer >= moveMaxTime)
        {
            moving = false;
            IsClimbing = false;

            SnapToGridPosition();
        }
    }

    private void SnapToGridPosition()
    {
        Vector3 snapPos = pathfinding.ToGridPos(transform.position);

        controller.Move(transform.position - snapPos);
    }

    private void MovementSelection()
    {
        controller.SimpleMove(Vector3.zero);
        if (pathfinding == null) return;

        pathfinding.SetOwner(transform,canClimb && IsClimbing,canSwim);

        if (Input.GetMouseButtonDown(0))
        {
            path = pathfinding.GetPath();
            if(path != null)
            {
                moving = true;
                moveMaxTime = movementSpeed * path.Count;
                moveTimer = 0;
                actionPointData?.SubtractActionPoint();
            }
        }
    }

    public Vector3 SamplePath(List<Vector3> path ,float normalizedTime)
    {
        if (path == null || path.Count < 1) return transform.position;
        normalizedTime = Mathf.Clamp01(normalizedTime);

        float unormalizedTime = normalizedTime * (path.Count-1);
        int min = Mathf.CeilToInt(unormalizedTime);

        if (unormalizedTime == 0) return path[0];
        if (min == path.Count - 1) return path[path.Count - 1];

        return Vector3.Lerp(path[min], path[min+1], unormalizedTime % 1f);
    }

    public float CorrectYPos(float y)
    {
        return y + controller.bounds.extents.y;
    }

    private void OnDrawGizmos()
    {
        if(pathfinding != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(pathfinding.ToGridPos(transform.position),Vector3.one);
        }
    }

    public void AttemptInteract(Interactalbe interact)
    {
        for(int i = 0; i < abilites.Count; i++)
        {
            if(abilites[i].OnAbilityCheckCondition(interact))
            {
                Vector3 pos = interact.transform.position;
                Vector3 pos2 = controller.transform.position;

                pos.y  = 0;
                pos2.y = 0;

                Vector3 interactDirection = (pos - pos2).normalized;
                abilites[i].OnAbilityStart(this,interact,MakeCardinal(interactDirection));
                currentRunningAbility = i;
                break;
            }
        }
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
