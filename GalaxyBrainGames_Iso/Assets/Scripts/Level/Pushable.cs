using GalaxyBrain.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Volpanic.Easing;

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
                float lerpPos = Easingf.InExpo(0,1,fallingTimer);
                transform.rotation = Quaternion.Lerp(initalRotation, targetRotation, lerpPos);

                if (fallingTimer >= 1)
                {
                    fallingOver = false;
                    fallingTimer = 1;
                    transform.rotation = targetRotation;
                    creatureData.pathfinding.UpdateNodeCells(myCollider.bounds.min, myCollider.bounds.max);
                }
            }
        }

        public void Push(Vector3 pushDirection)
        {
            if (pushDirection == Vector3.zero) return;

            Debug.DrawRay(transform.position,pushDirection * 4,Color.white,5);

            FaceDirection(pushDirection);
            targetRotation = GetPushRotation(pushDirection);
            fallingOver = true;
            pushedOver = true;
        }

        private void FaceDirection(Vector3 direction)
        {
            Vector3 tartgetEuler = Quaternion.LookRotation(direction,Vector3.up).eulerAngles;
            transform.rotation = Quaternion.Euler(tartgetEuler.x, tartgetEuler.y + 90, tartgetEuler.z);
            initalRotation = transform.rotation;
        }

        private Quaternion GetPushRotation(Vector3 pushDirection)
        {
            return Quaternion.FromToRotation(Vector3.up,pushDirection) * transform.rotation;
        }
    }
}