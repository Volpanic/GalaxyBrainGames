using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Controller3D : MonoBehaviour
{
    [Header("Linking")]
    [SerializeField] private BoxCollider myCollider;

    [Header("Toggles")]
    [SerializeField] private bool useGravity = true;
    [SerializeField] private bool attachToAnchors;

    [Header("Values")]
    [SerializeField] private float gravityAmount = 9.81f;

    [Header("Masks")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask anchorMask;

    private Vector3 velocity = Vector3.zero;
    private CreatureAnchorPoint anchorToo;

    void FixedUpdate()
    {
        if (!anchorToo)
        {
            if (useGravity && !OnGround()) ApplyGravity();

            Vector2 vel = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            velocity.x = vel.x * 0.2f;
            velocity.z = vel.y * 0.2f;

            CheckCollisions();
        }

        if (attachToAnchors)
        {
            UpdateAnchorPoint();
            if(anchorToo != null && velocity != Vector3.zero)
            {
                anchorToo.DetachCurrent();
                anchorToo = null;
            }
        }
    }

    private bool OnGround()
    {
        return Physics.CheckBox(myCollider.bounds.center + (Vector3.down * 0.025f), myCollider.bounds.extents, transform.rotation, groundMask);
    }

    private void CheckCollisions()
    {
        RaycastHit hit;
        Vector3 center = myCollider.bounds.center;
        Vector3 extents = myCollider.bounds.extents;

        //Vector3 horizontal = new Vector3(velocity.x, 0, 0);
        //Vector3 vertical   = new Vector3(0, velocity.y, 0);
        //Vector3 depth      = new Vector3(0, 0, velocity.z);

        Vector3 positon = transform.position;

        // Horizontal
        if (Physics.BoxCast(center, extents, new Vector3(Mathf.Sign(velocity.x), 0, 0), out hit, transform.rotation, Mathf.Abs(velocity.x), groundMask))
        {
            positon.x = hit.point.x + (Mathf.Sign(hit.normal.x) * extents.x);
            velocity.x = 0;
        }

        // Vertical
        if (Physics.BoxCast(center, extents, new Vector3(0, Mathf.Sign(velocity.y), 0), out hit, transform.rotation, Mathf.Abs(velocity.y), groundMask))
        {
            positon.y = hit.point.y + (Mathf.Sign(hit.normal.y) * extents.y);
            velocity.y = 0;
        }

        // Depth
        if (Physics.BoxCast(center, extents, new Vector3(0, 0, Mathf.Sign(velocity.z)), out hit, transform.rotation, Mathf.Abs(velocity.z), groundMask))
        {
            positon.z = hit.point.z + (Mathf.Sign(hit.normal.z) * extents.z);
            velocity.z = 0;
        }

        transform.position = positon + velocity;
    }

    public void UpdateAnchorPoint()
    {
        if (anchorToo != null) return;

        Collider anchorCollider = CheckForAnchorPoint();

        if (anchorCollider != null)
        {
            CreatureAnchorPoint anchor = anchorCollider.gameObject.GetComponent<CreatureAnchorPoint>();

            if (anchor != null)
            {
                //if (anchor.AttemptAttach(gameObject, new Vector3(0, myCollider.bounds.extents.y, 0)))
                if (anchor.AttemptAttach(gameObject, new Vector3(0, 0, 0)))
                {
                    anchorToo = anchor;
                    velocity = Vector3.zero;
                }
            }

        }
    }

    private Collider CheckForAnchorPoint()
    {
        Collider[] colls = Physics.OverlapBox(myCollider.bounds.center, myCollider.bounds.extents, myCollider.transform.localRotation, anchorMask);

        if (colls != null && colls.Length != 0)
        {
            return colls[0];
        }

        return null;
    }

    private void ApplyGravity()
    {
        velocity += Vector3.down * gravityAmount * Time.fixedDeltaTime;
    }
}
