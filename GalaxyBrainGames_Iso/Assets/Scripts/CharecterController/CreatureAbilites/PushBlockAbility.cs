using GalaxyBrain.Interactables;
using UnityEngine;

namespace GalaxyBrain.Creatures.Abilities
{
    /// <summary>
    /// Ability that allows the player to line up the direction
    /// the block needs to be pushed in and distance.
    /// Plays an animation on mouse click and then pushed the block.
    /// </summary>
    public class PushBlockAbility : ICreatureAbility
    {
        private Interactalbe currentObject;
        private PlayerController controller;
        private PushBlock block;
        private Vector3 direction;
        private bool done = false;
        private bool canceled = false;

        private bool buffer = false;

        private const string PUSH_ANIMATION_NAME = "strongPush";

        public bool OnAbilityCheckCondition(Interactalbe interactable)
        {
            block = interactable.GetComponent<PushBlock>();

            if(block != null && !block.Moving)
            {
                return true;
            }

            return false;
        }

        public AbilityDoneType OnAbilityCheckDone()
        {
            if(done)
            {
                if (canceled) return AbilityDoneType.Canceled;

                if (controller.Animator.GetCurrentAnimatorStateInfo(0).IsName(PUSH_ANIMATION_NAME) && controller.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.75f)
                {
                    controller.Animator.SetBool("Push", false);
                    block?.StartPush();
                    return AbilityDoneType.Done;
                }
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
            controller.Animator.SetBool("Stand", false);
        }

        public void OnAbilityStart(PlayerController controller, Interactalbe interactable, Vector3 interactDirection)
        {
            currentObject = interactable;
            direction = interactDirection;
            this.controller = controller;
            done = false;
            canceled = false;
            buffer = false;

            controller.Animator.SetBool("Stand", true);

            Debug.DrawRay(interactable.transform.position, interactDirection * 15, Color.yellow, 100);
        }

        public void OnAbilityUpdate()
        {
            if (buffer && !done)
            {
                float blockMagnitude = block.UpdateAbility(direction);

                if (block.PathLocked)
                {
                    if (blockMagnitude >= .9)
                    {
                        done = true;
                        controller.Animator.SetBool("Stand", false);
                        controller.Animator.SetBool("Push", true);
                    }
                }

                if (blockMagnitude < 0)
                {
                    done = true;
                    canceled = true;
                    controller.Animator.SetBool("Stand", false);
                }
            }
            buffer = true;
        }
    }
}