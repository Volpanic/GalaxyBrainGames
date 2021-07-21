using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField, Min(0)] private float delayBetweenMovement = 0.1f;
    [SerializeField] private bool selected = false;
    [SerializeField] private Collider myCollider;

    private float castDownDistance = 0.9f;
    private float timer = 0;
    private Vector3 targetPos;
    private Vector3 startTargetPos;

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
        if (selected && myCollider != null && timer > delayBetweenMovement)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))   MoveToSpace(-transform.right);
            if (Input.GetKeyDown(KeyCode.LeftArrow))    MoveToSpace(transform.right);
            if (Input.GetKeyDown(KeyCode.UpArrow))      MoveToSpace(-transform.forward);
            if (Input.GetKeyDown(KeyCode.DownArrow))    MoveToSpace(transform.forward);
        }
        else
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(startTargetPos, targetPos, timer / delayBetweenMovement);
        }
    }

    public void MoveToSpace(Vector3 offset)
    {
        if(CheckSpace(offset))
        {
            startTargetPos = transform.position;
            targetPos = transform.position += offset;
            timer = 0;
        }
    }

    private bool CheckSpace(Vector3 offset)
    {
        Vector3 floorMid = new Vector3(myCollider.bounds.center.x, myCollider.bounds.max.y, myCollider.bounds.center.z);

        Ray r1 = new Ray(floorMid + new Vector3(-myCollider.bounds.extents.x,0,-myCollider.bounds.extents.z) + offset, Vector3.down);
        Ray r2 = new Ray(floorMid + new Vector3(-myCollider.bounds.extents.x,0, myCollider.bounds.extents.z) + offset, Vector3.down);
        Ray r3 = new Ray(floorMid + new Vector3(myCollider.bounds.extents.x ,0,-myCollider.bounds.extents.z) + offset, Vector3.down);
        Ray r4 = new Ray(floorMid + new Vector3(myCollider.bounds.extents.x ,0, myCollider.bounds.extents.z) + offset, Vector3.down);

        Debug.DrawRay(r1.origin,r1.direction,Color.red,0.25f);
        Debug.DrawRay(r2.origin,r2.direction,Color.red,0.25f);
        Debug.DrawRay(r3.origin,r3.direction,Color.red,0.25f);
        Debug.DrawRay(r4.origin,r4.direction,Color.red,0.25f);

        //Check if floor is below
        bool r1Hit = Physics.Raycast(r1, myCollider.bounds.size.y + castDownDistance);
        bool r2Hit = Physics.Raycast(r2, myCollider.bounds.size.y + castDownDistance);
        bool r3Hit = Physics.Raycast(r3, myCollider.bounds.size.y + castDownDistance);
        bool r4Hit = Physics.Raycast(r4, myCollider.bounds.size.y + castDownDistance);

        //Check if the space moving too is free
        bool spaceFree = !Physics.CheckBox(myCollider.bounds.center + offset,myCollider.bounds.extents * 0.9f,myCollider.transform.localRotation);

        //If all ray-casts hit
        return spaceFree && (r1Hit && r2Hit && r3Hit && r4Hit);
    }
}
