using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField, Min(0)] private float delayBetweenMovement = 0.1f;
    [SerializeField] private Collider myCollider;

    public bool Selected = false;
    private float castDownDistance = 0.9f;
    private float timer = 0;
    private Vector3 targetPos;
    private Vector3 startTargetPos;
    private bool doMovement = false;
    private bool freeFall = false;

    // Start is called before the first frame update
    void Start()
    {
        startTargetPos = transform.position;
        targetPos = transform.position;
        timer = delayBetweenMovement;
    }

    // Update is called once per frame
    void Update()
    {
        if (Selected && myCollider != null && !doMovement && !freeFall)
        {
            if (Input.GetKeyDown(KeyCode.D))   MoveToSpace(transform.forward);
            if (Input.GetKeyDown(KeyCode.A))    MoveToSpace(-transform.forward);
            if (Input.GetKeyDown(KeyCode.W))      MoveToSpace(-transform.right);
            if (Input.GetKeyDown(KeyCode.S))    MoveToSpace(transform.right);
        }
        
        if(doMovement)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(startTargetPos, targetPos, timer / delayBetweenMovement);

            if(transform.position == targetPos)
            {
                doMovement = false;
                //If we should be in free fall
                if (!CheckFloorBelow(Vector3.zero, castDownDistance))
                {
                    freeFall = true;
                }
            }
        }

        if(freeFall)
        {
            UpdateFreeFall();
        }
    }

    public void MoveToSpace(Vector3 offset)
    {
        if(CheckSpaceFree(offset))
        {
            startTargetPos = transform.position;
            targetPos = transform.position += offset;
            timer = 0;
            doMovement = true;
        }
        else
        {
            if(CheckSpaceFreeSlope(offset,out Vector3 targetPos))
            {
                startTargetPos = transform.position;
                timer = 0;
                doMovement = true;
            }
        }
    }

    private bool CheckSpaceFreeSlope(Vector3 offset, out Vector3 targetPos)
    {
        RaycastHit hit;
        bool r1Hit = Physics.BoxCast(myCollider.bounds.center + new Vector3(0, myCollider.bounds.extents.y*3,0) + offset, myCollider.bounds.extents, Vector3.down, out hit, transform.rotation, myCollider.bounds.size.y*3 + castDownDistance);

        targetPos = transform.position;
        if (CheckSpaceFree(offset + Vector3.up))
        {
            Debug.DrawRay(myCollider.bounds.center + new Vector3(0, myCollider.bounds.extents.y * 3f, 0) + offset, Vector3.down, Color.cyan, 0.5f);
            targetPos = new Vector3(hit.point.x, hit.point.y + myCollider.bounds.extents.y, hit.point.z);
        }

        return r1Hit;
    }

    private bool CheckFloorBelow(Vector3 offset, float downDist)
    {
        Vector3 floorMid = new Vector3(myCollider.bounds.center.x, myCollider.bounds.max.y, myCollider.bounds.center.z);

        Ray r1 = new Ray(floorMid + new Vector3(-myCollider.bounds.extents.x,0,-myCollider.bounds.extents.z) + offset, Vector3.down);
        Ray r2 = new Ray(floorMid + new Vector3(-myCollider.bounds.extents.x,0, myCollider.bounds.extents.z) + offset, Vector3.down);
        Ray r3 = new Ray(floorMid + new Vector3(myCollider.bounds.extents.x ,0,-myCollider.bounds.extents.z) + offset, Vector3.down);
        Ray r4 = new Ray(floorMid + new Vector3(myCollider.bounds.extents.x ,0, myCollider.bounds.extents.z) + offset, Vector3.down);

        //Check if floor is below
        bool r1Hit = Physics.Raycast(r1, myCollider.bounds.size.y + downDist);
        bool r2Hit = Physics.Raycast(r2, myCollider.bounds.size.y + downDist);
        bool r3Hit = Physics.Raycast(r3, myCollider.bounds.size.y + downDist);
        bool r4Hit = Physics.Raycast(r4, myCollider.bounds.size.y + downDist);

        //If all ray-casts hit
        return r1Hit && r2Hit && r3Hit && r4Hit;
    }

    public bool CheckSpaceFree(Vector3 offset)
    {
        return !Physics.CheckBox(myCollider.bounds.center + offset, myCollider.bounds.extents * 0.9f, myCollider.transform.localRotation);
    }

    private void UpdateFreeFall()
    {
        transform.position += Vector3.down * Time.deltaTime * 5;

        if (CheckFloorBelow(Vector3.zero, 0.1f))
        {
            SnapToGround(0.1f);
            freeFall = false;
        }
    }

    private void SnapToGround(float downDist)
    {
        RaycastHit hit;

        //Check if floor is below
        bool r1Hit = Physics.BoxCast(myCollider.bounds.center, myCollider.bounds.extents,Vector3.down, out hit, transform.rotation,myCollider.bounds.size.y + downDist);

        transform.position = new Vector3(transform.position.x, hit.point.y + myCollider.bounds.extents.y, transform.position.z);
    }
}
