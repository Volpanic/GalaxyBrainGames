using GalaxyBrain.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyBrain.Interactables
{
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
        private Vector3 targetPos = Vector3.zero;
        private Vector3 oldMovement = Vector3.zero;
        private float pushTimer = 0;
        private bool firstSnap = true;

        public void UpdatePlane()
        {
            plane.SetNormalAndPosition(Vector3.up, transform.position);
        }

        // Start is called before the first frame update
        void Awake()
        {
            UpdatePlane();
            cam = Camera.main;

            if (pushBlockRenderer != null)
            {
                pushBlockRenderer.positionCount = 2;
                pushBlockRenderer.SetPosition(0, new Vector3(0, -controller.bounds.extents.y * 0.95f, 0));
                pushBlockRenderer.SetPosition(1, new Vector3(0, -controller.bounds.extents.y * 0.95f, 0));
            }
        }

        private void Update()
        {
            if (moving)
            {
                UpdateBlockMoving();
            }
            else
            {
                if (Physics.BoxCast(controller.bounds.center, controller.bounds.extents * 0.98f, Vector3.down, Quaternion.identity, 0.1f)) SmoothSnapToGrid();
                controller.SimpleMove(Vector3.zero);
            }
        }

        private void UpdateBlockMoving()
        {
            pushTimer += Time.deltaTime;
            Vector3 target = Vector3.Lerp(startPos, targetPos, pushTimer);

            Vector3 movement = target - oldMovement;
            if (movement != Vector3.zero) controller.Move(movement);

            oldMovement = target;

            //Check if we hit a wall
            if ((pushTimer >= 0.2f && (controller.collisionFlags & CollisionFlags.CollidedSides) != 0) || !Physics.BoxCast(controller.bounds.center, controller.bounds.extents * 0.98f, Vector3.down, Quaternion.identity, 0.1f))
            {
                moving = false;

                //Put snap to tile code here...
                Vector3 targetSnap = creatureData.pathfinding.ToGridPos(transform.position);
                targetSnap.y = transform.position.y;

                controller.enabled = false;
                transform.position = targetSnap;
                controller.enabled = true;
            }

            if (pushTimer >= 1)
            {
                //Disable the controller to allow for manual movement.
                creatureData.pathfinding.UpdateNodeCells(controller.bounds.min - Vector3.one, controller.bounds.max + Vector3.one);
            }
        }

        private void SmoothSnapToGrid()
        {
            Vector3 targetSnap = creatureData.pathfinding.ToGridPos(transform.position);

            controller.enabled = false;
            transform.position = Vector3.MoveTowards(transform.position, targetSnap, Time.deltaTime * 4);
            controller.enabled = true;

            if (!firstSnap)
            {
                creatureData.pathfinding.UpdateNodeCells(controller.bounds.min - Vector3.one, controller.bounds.max + Vector3.one);
                firstSnap = true;
            }
        }

        public bool UpdateAbility(Vector3 interactionCardinal)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            float enter = 0;

            if (plane.Raycast(ray, out enter))
            {
                Vector3 hit = ray.GetPoint(enter);

                Vector3 endPoint = new Vector3(Mathf.Round(hit.x), transform.position.y, Mathf.Round(hit.z)) - transform.position;
                endPoint.x *= interactionCardinal.normalized.x;
                endPoint.z *= interactionCardinal.normalized.z;
                endPoint = Vector3.ClampMagnitude(endPoint, maxPushRange).magnitude * interactionCardinal;

                if (pushBlockRenderer != null)
                {
                    pushBlockRenderer.SetPosition(1, endPoint - new Vector3(0, controller.bounds.extents.y * 0.95f, 0));
                }

                if (Input.GetMouseButtonDown(0))
                {
                    //Cancel out if too short
                    if (endPoint.magnitude <= 0.1f)
                    {
                        return true;
                    }

                    StartPush(endPoint);
                    pushBlockRenderer?.SetPosition(1, pushBlockRenderer.GetPosition(0));
                    return true;
                }
            }
            return false;
        }

        public void StartPush(Vector3 localEndPoint)
        {
            startPos = Vector3.zero;
            targetPos = localEndPoint;
            pushTimer = 0;
            oldMovement = Vector3.zero;
            moving = true;
            firstSnap = false;
        }
    }
}