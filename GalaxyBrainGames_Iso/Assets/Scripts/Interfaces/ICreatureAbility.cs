using GalaxyBrain.Creatures;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyBrain.Creatures.Abilities
{
    public interface ICreatureAbility
    {
        public bool OnAbilityCheckCondition(Interactalbe interactable); // Used to check if we should use this ability
        public bool OnAbilityCheckDone(); // Used to check if we should use this ability
        public void OnAbilityStart(PlayerController controller, Interactalbe interactable, Vector3 interactDirection); // Ran when the ability is first activated
        public void OnAbilityUpdate(); // Runs when ability is updating
        public void OnAbilityEnd(); // Runs when ability is finished
    }
}
