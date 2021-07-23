using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakingCreature : MonoBehaviour
{
    [SerializeField] private PlayerController controller;
    [SerializeField] private LayerMask breakableMask;
    [SerializeField] private Collider myCollider;
    [SerializeField] private CreatureData creatureData;

    // Start is called before the first frame update
    void Awake()
    {
        creatureData.LogCreature(controller);
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.SelectedAndNotMoving)
        {
            //Jumping mode
            if (Input.GetKey(KeyCode.LeftControl))
            {
                UpdateBreakControl();
            }
        }
    }

    private void UpdateBreakControl()
    {
        if (Input.GetKeyDown(KeyCode.D)) BreakBreakable(transform.forward);
        if (Input.GetKeyDown(KeyCode.A)) BreakBreakable(-transform.forward);
        if (Input.GetKeyDown(KeyCode.W)) BreakBreakable(-transform.right);
        if (Input.GetKeyDown(KeyCode.S)) BreakBreakable(transform.right);
    }

    public void BreakBreakable(Vector3 offset)
    {
        //Scan for breakable objects
        Collider[] breakableColliders = Physics.OverlapBox(myCollider.bounds.center, myCollider.bounds.extents, transform.rotation, breakableMask);

        //If we did find one
        if (breakableColliders != null && breakableColliders.Length != 0)
        {
            //DO THE BREAK THING!!!!
            Breakable breakable = breakableColliders[0].gameObject.GetComponent<Breakable>();
            breakable.Break();
        }
    }
}
