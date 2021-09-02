using GalaxyBrain.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private CreatureData creatureData;

    [SerializeField] private float zoomInOrthoSize = 5f;
    [SerializeField] private float zoomOutOrthoSize = 20f;

    [SerializeField] private Vector3 offset;

    private Vector3 originalPosition;


    private void Awake()
    {
        originalPosition = transform.position;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        CameraZoom();

        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.position = Vector3.Lerp(transform.position, originalPosition, Time.deltaTime * 5f);
        }
        else if(creatureData != null)
        {
            if (creatureData.CreatureManager != null && creatureData.CreatureManager.SelectedCreature != null)
            {
                Vector3 target = creatureData.CreatureManager.SelectedCreature.transform.position;
                transform.position = Vector3.Lerp(transform.position, target + offset, Time.deltaTime * 5f);
            }
        }
    }

    private void CameraZoom()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, zoomOutOrthoSize, Time.deltaTime * 3f);
        }
        else
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, zoomInOrthoSize, Time.deltaTime * 5f);
        }
    }
}
