using GalaxyBrain.Creatures;
using GalaxyBrain.Interactables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyBrain.Creatures.Abilities
{
    public class PushBlockAbility : ICreatureAbility
    {
        private Interactalbe currentObject;
        private PlayerController controller;
        private PushBlock block;
        private Vector3 direction;
        private bool done = false;
        private bool canceled = false;

        private bool buffer = false;

        public bool OnAbilityCheckCondition(Interactalbe interactable)
        {
            block = interactable.GetComponent<PushBlock>();

            return (block != null);
        }

        public AbilityDoneType OnAbilityCheckDone()
        {
            if(done)
            {
                if (controller.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
                {
                    controller.Animator.SetBool("Push", false);
                    block?.StartPush();
                    return AbilityDoneType.Done;
                }

                if (canceled) return AbilityDoneType.Canceled;
            }

            return AbilityDoneType.NotDone;
        }

        public void OnAbilityEnd()
        {
            currentObject = null;
            block = null;
            done = false;
            buffer = false;
            controller.Animator.SetBool("Push", false);
        }

        public void OnAbilityStart(PlayerController controller, Interactalbe interactable, Vector3 interactDirection)
        {
            currentObject = interactable;
            direction = interactDirection;
            this.controller = controller;
            done = false;
            canceled = false;
            buffer = false;

            Debug.DrawRay(interactable.transform.position, interactDirection * 15, Color.yellow, 100);
        }

        public void OnAbilityUpdate()
        {
            if (buffer && !done)
            {
                float blockMagnitude = block.UpdateAbility(direction);

                if (block.PathLocked)
                {
                    if (blockMagnitude >= 1)
                    {
                        done = true;
                        controller.Animator.SetBool("Push", true);
                    }

                    if (blockMagnitude < 0)
                    {
                        done = true;
                        canceled = true;
                    }
                }
            }
            buffer = true;
        }
    }
}