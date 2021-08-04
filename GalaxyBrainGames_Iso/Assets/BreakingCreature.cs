using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakingCreature : MonoBehaviour
{
    [SerializeField] private FreeMoveController controller;
    [SerializeField] private LayerMask breakableMask;
    [SerializeField] private Collider myCollider;
    [SerializeField] private CreatureData creatureData;
    [SerializeField] private float breakRange = 3f;

    private Camera cam;

    // Start is called before the first frame update
    void Awake()
    {
        creatureData.LogCreature(controller);
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.Selected)
        {
            //Jumping mode
            if (Input.GetMouseButton(0))
            {
                BreakBreakable();
            }
        }
    }

    public void BreakBreakable()
    {
        //Scan for breakable objects
        Ray mouseRay = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(mouseRay, out hit, float.MaxValue, breakableMask))
        {
            //If we did find one
            if (hit.collider != null)
            {
                float dist = Vector3.Distance(transform.position, hit.point);

                if (dist <= breakRange)
                {
                    //DO THE BREAK THING!!!!
                    Breakable breakable = hit.collider.gameObject.GetComponent<Breakable>();
                    breakable.Break();
                }
            }
        }
    }
}
