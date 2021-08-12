using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SwimingCreature : MonoBehaviour
{
    [SerializeField] private CreatureData creatureData;
    [SerializeField] private PlayerController controller;
    //[SerializeField] private NavMeshAgent agent;
    [SerializeField] private Collider myCollider;
    [SerializeField] private LayerMask waterMask;
    [SerializeField] private float FastSpeed = 0.1f;
    [SerializeField] private float SlowSpeed = 0.2f;
    [SerializeField] private GameObject waterParticles;

    private void Awake()
    {
        creatureData.LogCreature(controller);
    }

    //private void Update()
    //{
    //    if(Physics.CheckBox(myCollider.bounds.center + (Vector3.down * 0.2f), myCollider.bounds.extents,transform.rotation,waterMask))
    //    {
    //        agent.speed = FastSpeed;
    //        if (waterParticles != null) waterParticles.SetActive(true);
    //    }
    //    else
    //    {
    //        agent.speed = SlowSpeed;
    //        if(waterParticles != null) waterParticles.SetActive(false);
    //    }
    //}
}
