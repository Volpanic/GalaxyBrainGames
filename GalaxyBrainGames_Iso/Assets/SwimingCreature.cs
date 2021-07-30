using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwimingCreature : MonoBehaviour
{
    [SerializeField] private CreatureData creatureData;
    [SerializeField] private PlayerController controller;
    [SerializeField] private Collider myCollider;
    [SerializeField] private LayerMask waterMask;
    [SerializeField] private float FastSpeed = 0.1f;
    [SerializeField] private float SlowSpeed = 0.2f;

    private void Awake()
    {
        creatureData.LogCreature(controller);
    }

    private void Update()
    {
        if(Physics.CheckBox(myCollider.bounds.center + (Vector3.down * 0.2f), myCollider.bounds.extents,transform.rotation,waterMask))
        {
            controller.delayBetweenMovement = FastSpeed;
        }
        else
        {
            controller.delayBetweenMovement = SlowSpeed;
        }
    }
}
