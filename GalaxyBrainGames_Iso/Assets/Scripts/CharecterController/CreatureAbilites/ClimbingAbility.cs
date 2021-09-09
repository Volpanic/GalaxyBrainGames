using GalaxyBrain.Creatures;
using GalaxyBrain.Interactables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyBrain.Creatures.Abilities
{
    public class ClimbingAbility : ICreatureAbility
    {
        private Climbable climb;

        public bool OnAbilityCheckCondition(Interactalbe interactable)
        {
            climb = interactable.GetComponent<Climbable>();

            return climb != null;
        }

        public bool OnAbilityCheckDone()
        {
            return climb == null;
        }

        public void OnAbilityEnd()
        {
            climb = null;
        }

        public void OnAbilityStart(PlayerController controller, Interactalbe interactable, Vector3 interactDirection)
        {
            if (!controller.IsClimbing)
            {
                controller.IsClimbing = true;
            }
            climb = null;
        }

        public void OnAbilityUpdate()
        {

        }
    }
}