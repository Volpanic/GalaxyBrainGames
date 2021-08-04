using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController)),SelectionBase]
public class FreeMoveController : MonoBehaviour
{
    public bool Selected = false;

    [SerializeField] private LayerMask groundMask;
    [SerializeField] private CharacterController control;
    [SerializeField] public float Speed = 1;
    [SerializeField, Range(1f, 10f)] private float acceleration = 3;
    private Camera cam;

    private CharacterController carryCollider;
    private float movementMod = 0;
    private Vector3 headingDirection;

    private Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        headingDirection = transform.forward;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Ray mouseRay = cam.ScreenPointToRay(Input.mousePosition);

        if (Selected && Input.GetMouseButton(0))
        {
            float distance = 0;
            groundPlane.Raycast(mouseRay, out distance);

            Vector3 hitPoint = mouseRay.GetPoint(distance);

            Vector3 myXYPos = new Vector3(hitPoint.x, 0, hitPoint.z);
            Vector3 pos = new Vector3(transform.position.x, 0, transform.position.z);

            Vector3 direction = myXYPos - pos;
            headingDirection = Vector3.Slerp(headingDirection, direction.normalized, Time.deltaTime * Speed);

            movementMod = Mathf.MoveTowards(movementMod, 1, Time.deltaTime * acceleration);
        }
        else
        {
            movementMod = Mathf.MoveTowards(movementMod, 0, Time.deltaTime * acceleration);
        }

        control.SimpleMove(headingDirection.normalized * Speed * movementMod);
        transform.LookAt(transform.position + headingDirection);

        if (carryCollider != null)
        {
            //control.Move(carryCollider.velocity);
            AttemptMove(carryCollider.bounds.center + new Vector3(0,control.bounds.extents.y,0));
            headingDirection = Vector3.Slerp(headingDirection, carryCollider.transform.forward, Time.deltaTime * Speed * 8f);
            transform.LookAt(transform.position + headingDirection);
        }
    }

    public float CorrectYPos(float y)
    {
        return y + control.bounds.extents.y;
    }

    public bool AttemptMove(Vector3 targetPos)
    {
        Vector3 movement = targetPos - transform.position;
        control.Move(movement);

        return control.collisionFlags != CollisionFlags.CollidedSides;
    }

    public bool AttemptAttachToCarry(GameObject obj)
    {
        if (obj != null && obj != gameObject)
        {
            CharacterController col = obj.GetComponent<CharacterController>();

            if (col != null)
            {
                carryCollider = col;
                movementMod = 0;
                transform.position = new Vector3(transform.position.x, col.bounds.center.y + control.bounds.extents.y, transform.position.z);
                Debug.Log(col.gameObject.name);
                return true;
            }
        }
        return false;
    }

    public void DetachCarry()
    {
        carryCollider = null;
    }

    public bool IsBeingCarried()
    {
        return (carryCollider != null);
    }
}
