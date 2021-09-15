using GalaxyBrain.Interactables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyBrain.Creatures.Abilities
{
    public class ClimbOnBackAbility : ICreatureAbility
    {
        private PlayerController interactableController;
        private SwimingCreature swimming;
        private PlayerController.PlayerTypes targetType;

        public ClimbOnBackAbility(PlayerController.PlayerTypes targetType)
        {
            this.targetType = targetType;
        }

        public bool OnAbilityCheckCondition(Interactalbe interactable)
        {
            interactableController = interactable.gameObject.GetComponent<PlayerController>();

            if (interactableController != null && interactableController.PlayerType == targetType &&
                !interactableController.WeighedDown)
            {
                swimming = interactable.gameObject.GetComponent<SwimingCreature>();

                //Make it so we can only climb on the swimming creatures back if swimming.
                if(swimming != null && swimming.Swimming)
                {
                    return true;

                }
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
            swimming = null;
        }

        public void OnAbilityStart(PlayerController controller, Interactalbe interactable, Vector3 interactDirection)
        {
            controller.MoveToTarget(interactableController.ColliderBounds.center +
                new Vector3(0, interactableController.ColliderBounds.extents.y*2, 0));
            Debug.DrawLine(interactableController.ColliderBounds.center, interactableController.ColliderBounds.center +
                new Vector3(0, interactableController.ColliderBounds.extents.y*2, 0), Color.red, 25f);
        }

        public void OnAbilityUpdate()
        {

        }
    }
}
