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

        public bool OnAbilityCheckCondition(Interactalbe interactable)
        {
            pillar = interactable.GetComponent<Pushable>();

            return pillar != null;
        }

        public AbilityDoneType OnAbilityCheckDone()
        {
            if (creature.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
            {
                creature.Animator.SetBool("Push", false);
                pillar?.Push(cardinalDirection);
                return (pillar != null) ? AbilityDoneType.Done : AbilityDoneType.NotDone;
            }
            return AbilityDoneType.NotDone;
        }

        public void OnAbilityEnd()
        {
            creature = null;
            pillar = null;
        }

        public void OnAbilityStart(PlayerController controller, Interactalbe interactable, Vector3 interactDirection)
        {
            creature = controller;
            creature.Animator.SetBool("Push",true);
            cardinalDirection = interactDirection;
        }

        public void OnAbilityUpdate()
        {

        }
    }
}