using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class PlayerController : MonoBehaviour
{
    [SerializeField,Min(0.1f)] private float movementSpeed = 1;
    [SerializeField] private CharacterController controller;

    [SerializeField] GridPathfinding pathfinding;

    public bool Selected = false;

    public bool Grounded
    {
        get { return controller.isGrounded; }
    }

    public float SCREAM = 0;

    private Camera cam;
    private bool moving = false;
    private Vector3 targetPos = Vector3.zero;
    private Vector3 startPos = Vector3.zero;
    private float moveTimer = 0;
    private float moveMaxTime = 0;
    private List<Vector3> path;

    private void Awake()
    {
        cam = Camera.main;

        transform.position = pathfinding.ToGridPos(transform.position);
    }

    private void FixedUpdate()
    {
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical"));
        direction = transform.TransformDirection(direction);

        if(moving)
        {
            moveTimer += Time.deltaTime * movementSpeed;
            Vector3 movemenmt = Vector3.Lerp(startPos,targetPos,moveTimer) - transform.position;
            controller.SimpleMove(movemenmt);
        }

        //controller.SimpleMove(direction * movementSpeed);
    }

    private void Update()
    {
        if (Selected)
        {
            if(!moving) MovementSelection();
        }
        if (moving) MoveAlongPath();
        else controller.SimpleMove(Vector3.zero);
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
        }
    }

    private void MovementSelection()
    {
        controller.SimpleMove(Vector3.zero);
        if (pathfinding == null) return;

        pathfinding.SetOwner(transform);
        pathfinding.LookForPath();

        if (Input.GetMouseButtonDown(0))
        {
            path = pathfinding.GetPath();
            if(path != null)
            {
                moving = true;
                moveMaxTime = movementSpeed * path.Count;
                moveTimer = 0;
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
        SCREAM = unormalizedTime;
        return Vector3.Lerp(path[min], path[min+1], unormalizedTime % 1f);
    }

    public float CorrectYPos(float y)
    {
        return y + controller.bounds.extents.y;
    }

    public bool AttemptMove(Vector3 targetPos)
    {
        
        //Vector3 movement = targetPos - transform.position;
        //controller.SimpleMove(movement);
        //controller.PauseGravityForFrame = true;

        return true;
    }
}
