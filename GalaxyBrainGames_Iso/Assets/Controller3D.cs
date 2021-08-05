using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Controller3D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField, Min(0)] private float gravity = 2.5f;
    [SerializeField, Min(0)] private float maxGravity = 10f;

    [Header("References")]
    [SerializeField] private CapsuleCollider myCollider;
    [SerializeField] private LayerMask groundLayer;

    [Header("Smoothing")]
    [SerializeField] private bool smooth = false;
    [SerializeField] private float smoothSpeed = 10f;

    public bool PauseGravityForFrame
    {
        get { return preventGravity; }
        set { preventGravity = value; }
    }

    public bool Grounded
    {
        get { return grounded; }
    }

    public CapsuleCollider CCollider
    {
        get { return myCollider; }
    }

    //Movement
    private Vector3 accumulatedVelocity;
    private Vector3 move;
    private RaycastHit groundHit;
    private bool grounded;
    private float currentGravity = 0;
    private Vector3 vel;
    private bool preventGravity = false;

    public void SimpleMove(Vector3 movement)
    {
        accumulatedVelocity += movement;
        if(!preventGravity) ApplyGravity(ref accumulatedVelocity);
        FinalMove();
        GroundCheck();
        CollisionCheck();

        preventGravity = false;
    }

    private void FinalMove()
    {
        //Move using the accumulated velocity.
        Vector3 vel = new Vector3(accumulatedVelocity.x, accumulatedVelocity.y, accumulatedVelocity.z);

        transform.position += vel * Time.fixedDeltaTime;
        accumulatedVelocity = Vector3.zero;
    }

    //UpdatesGravity
    private void ApplyGravity(ref Vector3 velocity)
    {
        if(grounded == false)
        {
            //Subtract gravity until max gravity is reached
            velocity.y = Mathf.Clamp(velocity.y - gravity, -maxGravity, float.MaxValue);
        }
    }
    

    private void GroundCheck()
    {
        Ray ray = new Ray(myCollider.bounds.center,Vector3.down);
        RaycastHit temp;

        //Raycast down significant distance to do basic ground check.
        if(Physics.SphereCast(ray,myCollider.radius,out temp,myCollider.bounds.size.y * 1.2f,groundLayer))
        {
            GroundConfirm(temp);
        }
        else
        {
            grounded = false;
        }
    }

    private Vector3 groundCheckPoint = new Vector3(0, -0.5f, 0);
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

                //Snap player to correct Y
                if(!smooth)
                {
                    transform.position = new Vector3(transform.position.x,groundHit.point.y + myCollider.bounds.extents.y, transform.position.z);
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, groundHit.point.y + myCollider.bounds.extents.y, transform.position.z), smoothSpeed * Time.fixedDeltaTime);
                }

                break;
            }
        }

        if(num <= 1 && tempHit.distance <= myCollider.bounds.size.y * 1.125f)
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

            //Get distance into each collider and move opposite of it.
            if(Physics.ComputePenetration(myCollider,transform.position,transform.rotation,overlaps[i],t.position,t.rotation,out dir, out dist))
            {
                Vector3 penetrationVec = dir * dist;
                Vector3 velocityProjected = Vector3.Project(accumulatedVelocity, -dir);
                transform.position = transform.position + penetrationVec;
                vel -= velocityProjected;
            }
        }
    }
}
