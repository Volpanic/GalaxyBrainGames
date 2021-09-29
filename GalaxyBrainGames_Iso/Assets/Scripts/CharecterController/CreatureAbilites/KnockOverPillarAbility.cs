using GalaxyBrain.Creatures;
using GalaxyBrain.Interactables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyBrain.Creatures.Abilities
{
    public class KnockOverPillarAbility : ICreatureAbility
    {
        private Pushable pillar;

        public bool OnAbilityCheckCondition(Interactalbe interactable)
        {
            pillar = interactable.GetComponent<Pushable>();

            return pillar != null;
        }

        public AbilityDoneType OnAbilityCheckDone()
        {
            return (pillar != null)? AbilityDoneType.Done : AbilityDoneType.NotDone;
        }

        public void OnAbilityEnd()
        {
            pillar = null;
        }

        public void OnAbilityStart(PlayerController controller, Interactalbe interactable, Vector3 interactDirection)
        {
            pillar?.Push(interactDirection);
        }

        public void OnAbilityUpdate()
        {

        }
    }
}