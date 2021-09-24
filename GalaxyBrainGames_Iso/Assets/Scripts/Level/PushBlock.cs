using GalaxyBrain.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Volpanic.Easing;

namespace GalaxyBrain.Interactables
{
    public class PushBlock : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer pushBlockRenderer;
        [SerializeField] private CharacterController controller;
        [SerializeField] private Collider myCollider;
        [SerializeField] private CreatureData creatureData;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private int maxPushRange = 3;

        private Plane plane;
        private Camera cam;

        private bool moving = false;
        private Vector3 startPos = Vector3.zero;
        private Vector3 targetPos = Vector3.zero;
        private Vector3 oldMovement = Vector3.zero;
        private float pushTimer = 0;
        private float pushMaxTime = 1;
        private bool firstSnap = true;
        private bool firstLand = true;

        public void UpdatePlane()
        {
            plane.SetNormalAndPosition(Vector3.up, transform.position);
        }

        // Start is called before the first frame update
        void Awake()
        {
            UpdatePlane();
            cam = Camera.main;
        }

        private void Update()
        {
            if (moving)
            {
                UpdateBlockMoving();
            }
            else
            {
                if (Physics.BoxCast(controller.bounds.center, controller.bounds.extents * 0.98f, Vector3.down, Quaternion.identity, 0.025f)) 
                {
                    SmoothSnapToGrid();

                    if (!firstLand)
                    {
                        firstLand = true;
                        creatureData.pathfinding.UpdateNodeCells(myCollider.bounds.min - Vector3.one, myCollider.bounds.max + Vector3.one);
                    }
                }
                else
                {
                    firstLand = false;
                }
                controller.SimpleMove(Vector3.zero);
            }
        }

        private void UpdateBlockMoving()
        {
            pushTimer += Time.deltaTime;
            float lerpPos = Easingf.OutSine(0f, 1f, pushTimer / pushMaxTime);
            Vector3 target = Vector3.Lerp(startPos, targetPos, lerpPos);

            Vector3 movement = target - oldMovement;
            if (movement != Vector3.zero) controller.Move(movement);

            oldMovement = target;

            //Check if we hit a wall
            if (pushTimer >= 0.2f && (PlaceMeeting(movement, 0.9f) || !PlaceMeeting(Vector3.down*0.1f, 0.9f)))
            {
                moving = false;

                //Put snap to tile code here...
                Vector3 targetSnap = creatureData.pathfinding.ToGridPos(transform.position);
                targetSnap.y = transform.position.y;

                controller.enabled = false;
                transform.position = targetSnap;
                controller.enabled = true;
            }

            if (pushTimer >= pushMaxTime)
            {
                //Disable the controller to allow for manual movement.
                creatureData.pathfinding.UpdateNodeCells(myCollider.bounds.min - Vector3.one, myCollider.bounds.max + Vector3.one);

                creatureData.pathfinding.UpdateNodeCells(startPos - Vector3.one, startPos + Vector3.one);
                moving = false;
                SmoothSnapToGrid();
            }
        }

        public bool PlaceMeeting(Vector3 offset, float sizeScale)
        {
            Collider[] colls = Physics.OverlapBox(controller.bounds.center + offset, controller.bounds.extents * sizeScale, Quaternion.identity, groundMask);

            for (int i = 0; i < colls.Length; i++)
            {
                //Skip if it's collider is mine or my childs
                if(colls[i].gameObject == gameObject || colls[i].transform.IsChildOf(transform))
                {
                    continue;
                }

                return true;
            }

            return false;
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

                Vector3 endPoint = new Vector3(Mathf.Ceil(hit.x), transform.position.y, Mathf.Ceil(hit.z)) - transform.position;
                endPoint.x *= interactionCardinal.normalized.x;
                endPoint.z *= interactionCardinal.normalized.z;
                endPoint = Vector3.ClampMagnitude(endPoint, maxPushRange).magnitude * interactionCardinal;

                if (pushBlockRenderer != null)
                {
                    UpdateTileIdecator(interactionCardinal.normalized, endPoint.magnitude);
                }

                if (Input.GetMouseButtonDown(0))
                {
                    //Cancel out if too short
                    if (endPoint.magnitude <= 0.1f)
                    {
                        pushBlockRenderer.gameObject.SetActive(false);
                        return true;
                    }

                    StartPush(endPoint);
                    pushBlockRenderer.gameObject.SetActive(false);
                    return true;
                }
            }
            return false;
        }

        private void UpdateTileIdecator(Vector3 normalized, float magnitude)
        {
            if(magnitude <= .9f)
            {
                pushBlockRenderer.transform.localPosition = new Vector3(0, pushBlockRenderer.transform.localPosition.y,0);
                return;
            }

            pushBlockRenderer.gameObject.SetActive(true);
            normalized.y = 0;
            
            Vector3 endDirection = normalized * magnitude;
            endDirection.x = Mathf.Abs(endDirection.x);
            endDirection.z = Mathf.Abs(endDirection.z);

            pushBlockRenderer.size = new Vector2(Mathf.Max(endDirection.x, 1), Mathf.Max(endDirection.z, 1));

            //Set correct position
            Vector3 midPoint = ((normalized * magnitude) * 0.5f) + (normalized * 0.5f);
            pushBlockRenderer.transform.localPosition = new Vector3(midPoint.x,pushBlockRenderer.transform.localPosition.y,midPoint.z);
        }

        public void StartPush(Vector3 localEndPoint)
        {
            startPos = Vector3.zero;
            targetPos = localEndPoint;
            pushTimer = 0;
            oldMovement = Vector3.zero;
            moving = true;
            firstLand = false;
            firstSnap = false;

            pushMaxTime = 1;
        }
    }
}