using GalaxyBrain.Creatures;
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

        private bool buffer = false;

        public bool OnAbilityCheckCondition(Interactalbe interactable)
        {
            block = interactable.GetComponent<PushBlock>();

            return (block != null);
        }

        public bool OnAbilityCheckDone()
        {
            return done;
        }

        public void OnAbilityEnd()
        {
            currentObject = null;
            block = null;
            done = false;
            buffer = false;
        }

        public void OnAbilityStart(PlayerController controller, Interactalbe interactable, Vector3 interactDirection)
        {
            currentObject = interactable;
            direction = interactDirection;
            this.controller = controller;
            done = false;
            buffer = false;

            Debug.DrawRay(interactable.transform.position, interactDirection * 15, Color.yellow, 100);
        }

        public void OnAbilityUpdate()
        {
            if (buffer)
            {
                done = block.UpdateAbility(direction);
            }
            buffer = true;
        }
    }
}