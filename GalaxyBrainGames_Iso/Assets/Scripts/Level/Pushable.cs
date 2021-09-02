using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyBrain.Interactables
{
    [SelectionBase]
    public class Pushable : MonoBehaviour
    {
        [SerializeField] private Interactalbe interactalbe;
        [SerializeField] private CreatureData creatureData;
        [SerializeField] private Collider myCollider;

        private bool pushedOver = false;
        private bool fallingOver = false;
        private float fallingTimer = 0;

        private Quaternion initalRotation;
        private Quaternion targetRotation;

        private void Awake()
        {
            initalRotation = transform.rotation;
        }

        private void Update()
        {
            if (fallingOver)
            {
                fallingTimer += Time.deltaTime;
                transform.rotation = Quaternion.Lerp(initalRotation, targetRotation, fallingTimer);

                if (fallingTimer >= 1)
                {
                    transform.position += Vector3.down * 0.6f;
                    fallingOver = false;
                    fallingTimer = 1;
                    creatureData.pathfinding.UpdateNodeCells(myCollider.bounds.min, myCollider.bounds.max);
                }
            }
        }

        public void Push(Vector3 pushDirection)
        {
            if (pushDirection == Vector3.zero) return;

            Vector3 target = initalRotation.eulerAngles;
            target += pushDirection * 90;

            targetRotation.SetFromToRotation(Vector3.up, pushDirection);
            fallingOver = true;
            pushedOver = true;
        }
    }
}