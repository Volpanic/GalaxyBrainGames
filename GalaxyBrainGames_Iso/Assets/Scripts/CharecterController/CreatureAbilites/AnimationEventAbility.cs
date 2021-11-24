using GalaxyBrain.Creatures;
using GalaxyBrain.Creatures.Abilities;
using GalaxyBrain.Interactables;
using System;
using UnityEngine;

namespace GalaxyBrain.Assets.Scripts.CharecterController.CreatureAbilites
{
    /// <summary>
    /// An ability that can be used in place of different things
    /// currently breaking block and pulling levers.
    /// Simply set a animation bool to true than runs and event 
    /// after a certain point in the animation is reached.
    /// </summary>
    class AnimationEventAbility : ICreatureAbility
    {
        private string animationBoolName = "";
        private float normalizedTimeToRunEvent = 0.5f;
        private Action<PlayerController> onEvent;

        private PlayerController controller;
        private bool hardPlay = false;

        public void SetEventInfo(string animationBoolName, float normalizedTimeToRunEvent, Action<PlayerController> onEvent)
        {
            this.animationBoolName = animationBoolName;
            this.normalizedTimeToRunEvent = normalizedTimeToRunEvent;
            this.onEvent = onEvent;
            hardPlay = false;
        }

        public void SetEventInfoHard(string animationName, float normalizedTimeToRunEvent, Action<PlayerController> onEvent)
        {
            this.animationBoolName = animationName;
            this.normalizedTimeToRunEvent = normalizedTimeToRunEvent;
            this.onEvent = onEvent;
            hardPlay = true;
        }

        public bool OnAbilityCheckCondition(Interactalbe interactable)
        {
            return false;
        }

        public AbilityDoneType OnAbilityCheckDone()
        {
            if (controller.Animator)
            {
                if (controller.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= normalizedTimeToRunEvent)
                {
                    if(!hardPlay) controller.Animator.SetBool(animationBoolName, false);
                    onEvent?.Invoke(controller);
                    return AbilityDoneType.Done;
                }
            }

            return AbilityDoneType.NotDone;
        }

        public void OnAbilityEnd()
        {
        }

        public void OnAbilityStart(PlayerController controller, Interactalbe interactable, Vector3 interactDirection)
        {
            if (!hardPlay) controller.Animator.SetBool(animationBoolName, true);
            else controller.Animator.Play(animationBoolName);
            this.controller = controller;
        }

        public void OnAbilityUpdate()
        {
        }
    }
}
