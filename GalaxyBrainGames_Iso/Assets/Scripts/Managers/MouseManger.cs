using GalaxyBrain.Pathfinding;
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
                    pathfinding.ForceUnvialblePath();
                    return;
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
    }
}