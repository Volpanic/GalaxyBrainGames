using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
[SelectionBase]
public class PlayerController : MonoBehaviour
{
    [SerializeField,Min(0.1f)] private float movementSpeed = 1;
    [SerializeField] private Controller3D controller;
    [SerializeField] private NavMeshAgent navAgent;

    public bool Selected = false;

    public bool Grounded
    {
        get { return controller.Grounded; }
    }

    private Camera cam;
    private NavMeshPath path;

    private void Awake()
    {
        cam = Camera.main;
        path = new NavMeshPath();
    }

    private void FixedUpdate()
    {
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical"));
        direction = transform.TransformDirection(direction);

        controller.SimpleMove(direction * movementSpeed);
    }

    private void Update()
    {
        if (Selected && Input.GetMouseButtonDown(0)) ClickInput();
    }

    private void ClickInput()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out hit, float.MaxValue))
        {
            navAgent.SetDestination(hit.point);
        }
    }

    public float CorrectYPos(float y)
    {
        return y + controller.CCollider.bounds.extents.y;
    }

    public bool AttemptMove(Vector3 targetPos)
    {
        navAgent.isStopped = true;
        Vector3 movement = targetPos - transform.position;
        controller.SimpleMove(movement);
        controller.PauseGravityForFrame = true;

        return true;
    }
}
