using GalaxyBrain.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GalaxyBrain.Creatures
{

    public class SwimingCreature : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CreatureData creatureData;
        [SerializeField] private PlayerController controller;
        [SerializeField] private ControllerCarry controllerCarry;
        [SerializeField] private Collider myCollider;
        [SerializeField] private Transform worldModel;
        [SerializeField] private LayerMask waterMask;
        [SerializeField] private GameObject waterParticles;

        [Header("Settings")]
        [SerializeField] private float FastSpeed = 0.1f;
        [SerializeField] private float SlowSpeed = 0.2f;
        [SerializeField] private float waterSubmergeDepth = 0.25f;

        private Vector3 worldModelOriginalPos;
        private bool submerge = false;

        private void Awake()
        {
            creatureData.LogCreature(controller);

            worldModelOriginalPos = worldModel.localPosition;
        }

        private void OnEnable()
        {
            controller.OnPathInterval += PathInterval;
        }

        private void OnDisable()
        {
            controller.OnPathInterval -= PathInterval;
        }

        private void PathInterval(Vector3 pos, Vector3 nextPos)
        {
            submerge = false;

            if (Physics.CheckBox(nextPos + (Vector3.down*0.1f),myCollider.bounds.extents ,transform.rotation,waterMask))
            {
                submerge = true;
            }
        }

        private void Update()
        {
            if(submerge)
            {
                worldModel.localPosition = Vector3.MoveTowards(worldModel.localPosition, worldModelOriginalPos + (Vector3.down * waterSubmergeDepth),
                    Time.deltaTime*4);
            }
            else
            {
                worldModel.localPosition = Vector3.MoveTowards(worldModel.localPosition, worldModelOriginalPos,
                    Time.deltaTime*4);
            }

            controller.WeighedDown = controllerCarry.SteppedOn;

        }
    }
}
