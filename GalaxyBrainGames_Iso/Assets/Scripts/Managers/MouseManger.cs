using GalaxyBrain.Interactables;
using GalaxyBrain.Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyBrain.Managers
{
    //Manages most camera to world raycasts
    //Mainly to block movement if something else is happening
    public class MouseManger : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GridPathfinding pathfinding;
        [SerializeField] private InteractionManager interaction;
        [SerializeField] private LayerMask defaultLayer;
        [SerializeField] private LayerMask climableLayer;

        private Camera cam;

        private void Awake()
        {
            cam = Camera.main;
        }

        // Update is called once per frame
        private void Update()
        {
            MouseControl();
        }

        private void MouseControl()
        {
            if (cam == null)
            {
                cam = Camera.main;
                return;
            }
            Ray cameraRay = cam.ScreenPointToRay(Input.mousePosition);

            RaycastHit selectedObject;
            if (Physics.Raycast(cameraRay, out selectedObject, float.MaxValue, defaultLayer, QueryTriggerInteraction.Collide))
            {
                //We didn't hit anything, so don't bother
                if (selectedObject.collider == null) return;

                //Check if we hit a interactable first
                if (interaction != null && interaction.LookForInteractables(selectedObject))
                {
                    //Make sure we can still path find on climbable areas
                    if (!ObjectIsOnLayer(selectedObject.collider.gameObject.layer, climableLayer))
                    {
                        pathfinding.ForceUnvialblePath();
                        return;
                    }
                }

                //Nothing is obstructing, so path find
                if (pathfinding != null && pathfinding.LookForPath(selectedObject)) return;
            }
            else
            {
                //Disable pathfinding path, because we're not on it.
                pathfinding?.ClearPath();
            }
        }

        private bool ObjectIsOnLayer(int layer, LayerMask layerMask)
        {
            return layerMask == (layerMask | (1 << layer));
        }
    }
}