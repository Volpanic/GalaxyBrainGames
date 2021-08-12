using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class PlayerController : MonoBehaviour
{
    [SerializeField,Min(0.1f)] private float movementSpeed = 1;
    [SerializeField] private Controller3D controller;

    public bool Selected = false;

    public bool Grounded
    {
        get { return controller.Grounded; }
    }

    private Camera cam;
    private bool moving = false;
    private Vector3 targetPos = Vector3.zero;
    private Vector3 startPos = Vector3.zero;
    private float moveTimer = 0;

    private void Awake()
    {
        cam = Camera.main;
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
        if (Selected) Movement();
    }

    private void Movement()
    {
        if(Input.GetKeyDown(KeyCode.W)) StartMove(transform.position +  transform.forward);
        if(Input.GetKeyDown(KeyCode.A)) StartMove(transform.position + -transform.right  );
        if(Input.GetKeyDown(KeyCode.D)) StartMove(transform.position +  transform.right  );
        if(Input.GetKeyDown(KeyCode.S)) StartMove(transform.position + -transform.forward);
    }

    private void StartMove(Vector3 target)
    {
        target.y = transform.position.y;
        Vector3 movement = target - transform.position;

        Ray dirRay = new Ray(controller.CCollider.bounds.center, movement.normalized);
        RaycastHit hit;

        //Direction cast from center to check if space is free
        if (!Physics.Raycast(dirRay, 1f, controller.GroundLayer))
        {
            //Down from that point to find ground
            Ray downRay = new Ray(controller.CCollider.bounds.center + movement.normalized, Vector3.down);
            if (Physics.Raycast(downRay, out hit, 1.1f, controller.GroundLayer))
            {
                targetPos = new Vector3(hit.point.x, CorrectYPos(hit.point.y), hit.point.z);
                startPos = transform.position;
                moveTimer = 0f;
                moving = true;
            }
        }
    }

    public float CorrectYPos(float y)
    {
        return y + controller.CCollider.bounds.extents.y;
    }

    public bool AttemptMove(Vector3 targetPos)
    {
        
        //Vector3 movement = targetPos - transform.position;
        //controller.SimpleMove(movement);
        //controller.PauseGravityForFrame = true;

        return true;
    }
}
