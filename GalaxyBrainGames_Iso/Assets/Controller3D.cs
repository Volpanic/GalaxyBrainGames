using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Controller3D : MonoBehaviour
{
    public float movementSpeed;
    public float gravity = 2.5f;
    public CapsuleCollider myCollider;
    public LayerMask groundLayer;

    public bool smooth = false;
    public float smoothSpeed = 10f;

    //Movement
    private Vector3 velocity;
    private Vector3 move;
    private RaycastHit groundHit;
    private bool grounded;
    private float currentGravity = 0;
    private Vector3 vel;

    //

    private void Update()
    {
        Gravity();
        SimpleMove();
        FinalMove();
        GroundCheck();
        CollisionCheck();
    }

    public void SimpleMove()
    {
        move = new Vector3(Input.GetAxisRaw("Horizontal"),0,Input.GetAxisRaw("Vertical"));
        velocity += move;
    }

    private void FinalMove()
    {
        Vector3 vel = new Vector3(velocity.x,velocity.y, velocity.z) * movementSpeed;
        vel = transform.TransformDirection(velocity) * movementSpeed;

        transform.position += vel * Time.deltaTime;
        velocity = Vector3.zero;
    }

    private void Gravity()
    {
        if(grounded == false)
        {
            velocity.y -= gravity;
        }
    }
    

    private void GroundCheck()
    {
        Ray ray = new Ray(myCollider.bounds.center,Vector3.down);
        RaycastHit temp;

        if(Physics.SphereCast(ray,myCollider.radius,out temp,20f,groundLayer))
        {
            GroundConfirm(temp);
        }
        else
        {
            grounded = false;
        }
    }

    private Vector3 groundCheckPoint = new Vector3(0, -0.87f, 0);
    private void GroundConfirm(RaycastHit tempHit)
    {
        Collider[] col = new Collider[3];
        int num = Physics.OverlapSphereNonAlloc(transform.TransformPoint(groundCheckPoint),0.57f,col,groundLayer);

        grounded = false;

        for(int i = 0; i < num; i++)
        {
            if(col[i].transform == tempHit.transform)
            {
                groundHit = tempHit;
                grounded = true;

                if(!smooth)
                {
                    transform.position = new Vector3(transform.position.x,groundHit.point.y + myCollider.bounds.extents.y, transform.position.z);
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, groundHit.point.y + myCollider.bounds.extents.y, transform.position.z), smoothSpeed * Time.deltaTime);
                }

                break;
            }
        }

        if(num <= 1 && tempHit.distance <= myCollider.bounds.size.y * 1.25f)
        {
            if(col[0] != null)
            {
                Ray ray = new Ray(myCollider.bounds.center, Vector3.down);
                RaycastHit hit;

                if(Physics.Raycast(ray,out hit, myCollider.bounds.extents.y * 1.25f,groundLayer))
                {
                    if(hit.transform != col[0].transform)
                    {
                        grounded = false;
                        return;
                    }
                }
            }
        }
    }

    private void CollisionCheck()
    {
        Collider[] overlaps = new Collider[4];
        int num = Physics.OverlapSphereNonAlloc(myCollider.bounds.center,myCollider.radius,overlaps,groundLayer,QueryTriggerInteraction.UseGlobal);

        for(int i = 0; i < num; i++)
        {
            Transform t = overlaps[i].transform;
            Vector3 dir;
            float dist;

            if(Physics.ComputePenetration(myCollider,transform.position,transform.rotation,overlaps[i],t.position,t.rotation,out dir, out dist))
            {
                Vector3 penetrationVec = dir * dist;
                Vector3 velocityProjected = Vector3.Project(velocity,-dir);
                transform.position = transform.position + penetrationVec;
                vel -= velocityProjected;
            }
        }
    }
}
