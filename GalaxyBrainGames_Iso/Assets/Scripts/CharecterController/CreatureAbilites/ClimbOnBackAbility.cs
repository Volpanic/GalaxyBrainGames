using GalaxyBrain.Interactables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyBrain.Creatures.Abilities
{
    public class ClimbOnBackAbility : ICreatureAbility
    {
        private PlayerController interactableController;
        private PlayerController.PlayerTypes targetType;

        public ClimbOnBackAbility(PlayerController.PlayerTypes targetType)
        {
            this.targetType = targetType;
        }

        public bool OnAbilityCheckCondition(Interactalbe interactable)
        {
            interactableController = interactable.gameObject.GetComponent<PlayerController>();

            if (interactableController != null && interactableController.PlayerType == targetType)
            {

                return true;
            }
            return false;
        }

        public bool OnAbilityCheckDone()
        {
            return true;
        }

        public void OnAbilityEnd()
        {
            interactableController = null;
        }

        public void OnAbilityStart(PlayerController controller, Interactalbe interactable, Vector3 interactDirection)
        {
            controller.MoveToTarget(interactableController.ColliderBounds.center +
                new Vector3(0, interactableController.ColliderBounds.extents.y, 0));
        }

        public void OnAbilityUpdate()
        {
        }
    }
}
