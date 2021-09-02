using GalaxyBrain.Interactables;
using GalaxyBrain.Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Manages most camera to world raycasts
//Mainly to block movement if something else is happening
public class MouseManger : MonoBehaviour
{
    [Header("Refrences")]
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
    void Update()
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
        if (Physics.Raycast(cameraRay, out selectedObject, float.MaxValue))
        {
            if (interaction != null && interaction.LookForInteractables(selectedObject))
            {
                //Make sure we can still path find on climable areas
                if(!ObjectIsOnLayer(selectedObject.collider.gameObject.layer,climableLayer))
                {
                    pathfinding.ForceUnvialblePath();
                    return;
                }
            }
            
            if (pathfinding != null && pathfinding.LookForPath(selectedObject)) return;
        }
    }

    private bool ObjectIsOnLayer(int layer, LayerMask layerMask)
    {
        return layerMask == (layerMask | (1 << layer));
    }
}
