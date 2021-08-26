using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushBlock : MonoBehaviour
{
    [SerializeField] private LineRenderer pushBlockRenderer;
    [SerializeField] private CharacterController controller;
    [SerializeField] private CreatureData creatureData;
    [SerializeField] private int maxPushRange = 3;

    private Plane plane;
    private Camera cam;

    private bool moving = false;
    private Vector3 startPos = Vector3.zero;
    private Vector2 targetPos = Vector3.zero;
    private float pushTimer = 0;

    public void UpdatePlane()
    {
        plane.SetNormalAndPosition(Vector3.up,transform.position);
    }

    // Start is called before the first frame update
    void Awake()
    {
        UpdatePlane();
        cam = Camera.main;

        if (pushBlockRenderer != null)
        {
            pushBlockRenderer.positionCount = 2;
            pushBlockRenderer.SetPosition(0, Vector3.zero);
            pushBlockRenderer.SetPosition(1, Vector3.zero);
         }
    }

    private void Update()
    {
        if(moving)
        {
            UpdateBlockMoving();
        }
        else
        {
            controller.SimpleMove(Vector3.zero);
        }
    }

    private void UpdateBlockMoving()
    {
        pushTimer += Time.deltaTime;
        Vector3 target = Vector3.Lerp(startPos,targetPos,pushTimer);

        Vector3 movement = target - transform.position;

        controller.Move(movement);

        //Check if we hit a wall
        if((pushTimer >= 0.2f && (controller.collisionFlags & CollisionFlags.CollidedSides) != 0) || !Physics.BoxCast(controller.bounds.center, controller.bounds.extents, Vector3.down, Quaternion.identity, 0.1f))
        {
            moving = false;
            //Put snap to tile code here...

            creatureData.pathfinding.UpdateNodeCells(controller.bounds.min,controller.bounds.max);
        }

        if(pushTimer >= 1)
        {
            //Disable the controller to allow for manual movement.
            controller.enabled = false;
            transform.position = targetPos;
            controller.enabled = true;
        }
    }

    public bool UpdateAbility(Vector3 interactionCardinal)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        float enter = 0;

        if(plane.Raycast(ray,out enter))
        {
            Vector3 hit = ray.GetPoint(enter);

            Vector3 endPoint = new Vector3(Mathf.Round(hit.x), transform.position.y, Mathf.Round(hit.z)) - transform.position;
            endPoint.x *= interactionCardinal.normalized.x;
            endPoint.z *= interactionCardinal.normalized.z;
            endPoint = Vector3.ClampMagnitude(endPoint,maxPushRange);

            if(pushBlockRenderer != null)
            {
                pushBlockRenderer.SetPosition(1,endPoint);
            }

            if (Input.GetMouseButtonDown(0))
            {
                StartPush(endPoint.magnitude, endPoint.normalized);
                pushBlockRenderer?.SetPosition(1, pushBlockRenderer.GetPosition(0));
                return true;
            }
        }
        return false;
    }

    public void StartPush(float targetDistance,Vector3 direction)
    {
        startPos = transform.position;
        targetPos = startPos + (direction.normalized * targetDistance);
        Debug.Log(direction.normalized * targetDistance);
        pushTimer = 0;
        moving = true;
    }
}
