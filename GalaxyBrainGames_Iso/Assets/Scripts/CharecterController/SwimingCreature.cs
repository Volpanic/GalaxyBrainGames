using GalaxyBrain.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GalaxyBrain.Creatures
{

    public class SwimingCreature : MonoBehaviour
    {
        [SerializeField] private CreatureData creatureData;
        [SerializeField] private PlayerController controller;
        [SerializeField] private Collider myCollider;
        [SerializeField] private LayerMask waterMask;
        [SerializeField] private float FastSpeed = 0.1f;
        [SerializeField] private float SlowSpeed = 0.2f;
        [SerializeField] private GameObject waterParticles;

        private void Awake()
        {
            creatureData.LogCreature(controller);
        }
    }
}
