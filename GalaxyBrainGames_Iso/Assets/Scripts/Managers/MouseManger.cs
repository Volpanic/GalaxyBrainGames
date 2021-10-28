using GalaxyBrain.Pathfinding;
using System;
using UnityEngine;

namespace GalaxyBrain.Managers
{
    /// <summary>
    /// Manages raycasts from the camera, mainly to make sure
    /// certain things take priority.
    /// </summary>
    public class MouseManger : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GridPathfinding pathfinding;
        [SerializeField] private InteractionManager interaction;
        [SerializeField] private LayerMask defaultLayer;
        [SerializeField] private LayerMask climableLayer;

        [Header("Cursor sprites")]
        [SerializeField] private Texture2D defaultColoured;
        [SerializeField] private Texture2D defaultGrey;
        [SerializeField] private Texture2D interactableColoured;
        [SerializeField] private Texture2D interactableGrey;

        private Camera cam;

        private void Awake()
        {
            Cursor.SetCursor(defaultGrey, Vector2.zero, CursorMode.Auto);
            cam = Camera.main;
        }

        // Update is called once per frame
        private void FixedUpdate()
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

            bool uninteractableInteractable = false;

            RaycastHit selectedObject;
            if (Physics.Raycast(cameraRay, out selectedObject, float.MaxValue, defaultLayer, QueryTriggerInteraction.Collide))
            {
                //We didn't hit anything, so don't bother
                if (selectedObject.collider == null) return;

                //Check if we hit a interactable first
                if (interaction != null)
                {
                    InteractionViability canInteract = interaction.LookForInteractables(selectedObject);

                    if(canInteract != InteractionViability.NonViable)
                    { 
                        if(canInteract == InteractionViability.InteractableNotInRange)
                        {
                            uninteractableInteractable = true;
                        }
                        else // Can interact with
                        {
                            Cursor.SetCursor(interactableColoured, Vector2.zero, CursorMode.Auto);
                            pathfinding?.ClearPath();
                            return;
                        }
                    }
                }

                //Nothing is obstructing, so path find
                if (pathfinding != null && pathfinding.LookForPath(selectedObject))
                {
                    Cursor.SetCursor(defaultColoured, Vector2.zero, CursorMode.Auto);
                    return;
                }
                else
                {
                    if(uninteractableInteractable)
                    {
                        Cursor.SetCursor(interactableGrey, Vector2.zero, CursorMode.Auto);
                        return;
                    }
                }
            }
            else
            {
                //Disable pathfinding path, because we're not on it.
                pathfinding?.ClearPath();
            }

            Cursor.SetCursor(defaultGrey, Vector2.zero, CursorMode.Auto);
        }
    }
}