using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField, Min(0)] private float delayBetweenMovement = 0.1f;
    [SerializeField] private Collider myCollider;

    [Header("Masks")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask anchorMask;

    public bool Selected = false;
    public CreatureAnchorPoint AnchordToo = null;
    public bool ManualMove = true;

    public bool SelectedAndNotMoving { get { return Selected && myCollider != null && !doMovement && !freeFall; } }

    private CreatureAnchorPoint toAttach = null;
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
        if (SelectedAndNotMoving && !Input.GetKey(KeyCode.LeftControl) && ManualMove)
        {
            if (Input.GetKeyDown(KeyCode.D)) MoveToAdjacentSpace(transform.forward);
            if (Input.GetKeyDown(KeyCode.A)) MoveToAdjacentSpace(-transform.forward);
            if (Input.GetKeyDown(KeyCode.W)) MoveToAdjacentSpace(-transform.right);
            if (Input.GetKeyDown(KeyCode.S)) MoveToAdjacentSpace(transform.right);
        }

        if (doMovement)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(startTargetPos, targetPos, timer / delayBetweenMovement);

            if (transform.position == targetPos)
            {
                doMovement = false;

                if (toAttach)
                {
                    toAttach.AttemptAttach(this, new Vector3(0, myCollider.bounds.extents.y, 0));
                    toAttach = null;
                }
            }
        }

        if (freeFall)
        {
            UpdateFreeFall();
        }

        //If we should be in free fall
        if (ManualMove && !CheckFloorBelow(Vector3.zero, castDownDistance) && AnchordToo == null)
        {
            freeFall = true;
        }
    }

    public void MoveToSpace(Vector3 position)
    {
        Vector3 offset = transform.position - position;
        MoveToAdjacentSpace(offset);
    }

    public void MoveToAdjacentSpace(Vector3 offset)
    {
        float addY = 0;
        if (CheckSpaceFree(offset, out addY))
        {
            startTargetPos = transform.position;
            targetPos = transform.position + offset;
            targetPos.y += addY;
            timer = 0;
            doMovement = true;

            if(AnchordToo) AnchordToo.DetachCurrent();

            CheckAndAttachToAnchorPoint(offset);
        }
    }

    public void CheckAndAttachToAnchorPoint(Vector3 offset, bool autoAttach = false)
    {
        Collider anchor = CheckForAnchorPoint(offset);

        if (anchor != null)
        {
            toAttach = anchor.gameObject.GetComponent<CreatureAnchorPoint>();

            if (toAttach != null)
            {
                targetPos = toAttach.transform.position + new Vector3(0, myCollider.bounds.extents.y, 0);
            }
        }

        if(autoAttach)
        {
            if (toAttach)
            {
                toAttach.AttemptAttach(this, new Vector3(0, myCollider.bounds.extents.y, 0));
                toAttach = null;
            }
        }
    }

    private Collider CheckForAnchorPoint(Vector3 offset)
    {
        Collider[] colls = Physics.OverlapBox(myCollider.bounds.center + offset, myCollider.bounds.extents * 0.9f, myCollider.transform.localRotation, anchorMask);

        if (colls != null && colls.Length != 0)
        {
            return colls[0];
        }

        return null;
    }

    public void DetachAnchorPoint()
    {
        if (AnchordToo) AnchordToo.DetachCurrent();
    }

    private bool CheckFloorBelow(Vector3 offset, float downDist)
    {
        Vector3 floorMid = new Vector3(myCollider.bounds.center.x, myCollider.bounds.max.y, myCollider.bounds.center.z);

        Ray r1 = new Ray(floorMid + new Vector3(-myCollider.bounds.extents.x, 0, -myCollider.bounds.extents.z) + offset, Vector3.down);
        Ray r2 = new Ray(floorMid + new Vector3(-myCollider.bounds.extents.x, 0, myCollider.bounds.extents.z) + offset, Vector3.down);
        Ray r3 = new Ray(floorMid + new Vector3(myCollider.bounds.extents.x, 0, -myCollider.bounds.extents.z) + offset, Vector3.down);
        Ray r4 = new Ray(floorMid + new Vector3(myCollider.bounds.extents.x, 0, myCollider.bounds.extents.z) + offset, Vector3.down);

        //Check if floor is below
        bool r1Hit = Physics.Raycast(r1, myCollider.bounds.size.y + downDist, groundMask);
        bool r2Hit = Physics.Raycast(r2, myCollider.bounds.size.y + downDist, groundMask);
        bool r3Hit = Physics.Raycast(r3, myCollider.bounds.size.y + downDist, groundMask);
        bool r4Hit = Physics.Raycast(r4, myCollider.bounds.size.y + downDist, groundMask);

        //If all ray-casts hit
        return r1Hit && r2Hit && r3Hit && r4Hit;
    }

    public bool CheckSpaceFree(Vector3 offset, out float addY)
    {
        addY = 0;
        if(!Physics.CheckBox(myCollider.bounds.center + offset, myCollider.bounds.extents * 0.9f, myCollider.transform.localRotation, groundMask))
        {
            return true;
        }
        else
        {
            Vector3 yOff = new Vector3(0,myCollider.bounds.extents.y,0);
            //Check if space above is empty
            if (!Physics.CheckBox(myCollider.bounds.center + offset + yOff, myCollider.bounds.extents * 0.9f, myCollider.transform.localRotation, groundMask))
            {
                //Get floor point
                Ray downRay = new Ray(myCollider.bounds.center + offset + yOff,Vector3.down);
                RaycastHit hit;
                Physics.Raycast(downRay, out hit, myCollider.bounds.size.y,groundMask);

                if(hit.collider != null)
                {
                    float targetY = hit.point.y + myCollider.bounds.extents.y;
                    addY = targetY - transform.position.y;
                    return true;
                }
            }
        }

        return false;
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
        bool r1Hit = Physics.BoxCast(myCollider.bounds.center, myCollider.bounds.extents, Vector3.down, out hit, transform.rotation, myCollider.bounds.size.y + downDist, groundMask);

        transform.position = new Vector3(transform.position.x, hit.point.y + myCollider.bounds.extents.y, transform.position.z);
    }

    /// <summary>
    /// Adds the colliders half extents to the Y
    /// </summary>
    /// <param name="y"></param>
    /// <returns></returns>
    public float CorrectYPos(float y)
    {
        return y + myCollider.bounds.extents.y;
    }
}
