using GalaxyBrain.Interactables;
using UnityEngine;

namespace GalaxyBrain.Creatures.Abilities
{
    /// <summary>
    /// Controls knocking over pillars,
    /// plays and animation than knock over the pillar
    /// when the animation is nearly done.
    /// </summary>
    public class KnockOverPillarAbility : ICreatureAbility
    {
        private Pushable pillar;
        private PlayerController creature;

        private Vector3 cardinalDirection;
        private bool standing = false;

        private const string PUSH_ANIMATION_NAME = "strongPush";
        private const string STAND_IDLE_ANIMATION_NAME = "strongStandIdle";

        public bool OnAbilityCheckCondition(Interactalbe interactable)
        {
            pillar = interactable.GetComponent<Pushable>();

            return pillar != null;
        }

        public AbilityDoneType OnAbilityCheckDone()
        {
            AnimatorStateInfo animationState = creature.Animator.GetCurrentAnimatorStateInfo(0);

            if (!standing)
            {
                standing = animationState.IsName(STAND_IDLE_ANIMATION_NAME);
            }
            else
            {
                creature.Animator.SetBool("Stand", false);
                creature.Animator.SetBool("Push", true);

                if (animationState.IsName(PUSH_ANIMATION_NAME) && animationState.normalizedTime >= 0.75f)
                {
                    pillar?.Push(cardinalDirection);
                    return (pillar != null) ? AbilityDoneType.Done : AbilityDoneType.NotDone;
                }
            }
            return AbilityDoneType.NotDone;
        }

        public void OnAbilityEnd()
        {
            creature.Animator.SetBool("Stand", false);
            creature.Animator.SetBool("Push", false);
            creature = null;
            pillar = null;
        }

        public void OnAbilityStart(PlayerController controller, Interactalbe interactable, Vector3 interactDirection)
        {
            creature = controller;
            creature.Animator.SetBool("Stand",true);
            cardinalDirection = interactDirection;
            standing = false;
        }

        public void OnAbilityUpdate()
        {

        }
    }
}