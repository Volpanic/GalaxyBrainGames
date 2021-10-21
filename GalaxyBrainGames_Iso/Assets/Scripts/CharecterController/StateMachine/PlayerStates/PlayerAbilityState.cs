using GalaxyBrain.Creatures.Abilities;
using GalaxyBrain.Interactables;
using System.Collections.Generic;
using UnityEngine;
using GalaxyBrain.Utility.Extnesion;

namespace GalaxyBrain.Creatures.States
{
    /// <summary>
    /// Handles player abilities (pushing blocks, pulling levers etc)
    /// Checks if they have a ICreatureAbility the meets the criteria
    /// of the interacted with object, the runs ICreatureAbility's events
    /// when necessary.
    /// </summary>
    public class PlayerAbilityState : PlayerState
    {
        private int currentRunningAbility = -1;
        private List<ICreatureAbility> abilites;

        public PlayerAbilityState(PlayerController controller) : base(controller)
        {
            abilites = new List<ICreatureAbility>();
        }

        public void AddAbility(ICreatureAbility ability)
        {
            if (abilites != null) abilites.Add(ability);
        }

        public override void OnStateStart()
        {
        }

        public override void OnStateUpdate()
        {
            //Ability code
            abilites[currentRunningAbility].OnAbilityUpdate();
            AbilityDoneType doneState = abilites[currentRunningAbility].OnAbilityCheckDone();
            if (doneState == AbilityDoneType.Done || doneState == AbilityDoneType.Canceled)
            {
                abilites[currentRunningAbility].OnAbilityEnd();
                if (doneState == AbilityDoneType.Done) controller.ConsumeActionPoint();
                machine.ChangeToDefaultState();
                currentRunningAbility = -1;
            }
        }

        public override void OnStateEnd()
        {
            base.OnStateEnd();
        }

        public bool AttemptInteract(Interactalbe interact)
        {
            interact.Interact(controller);

            for (int i = 0; i < abilites.Count; i++)
            {
                if (abilites[i].OnAbilityCheckCondition(interact))
                {
                    Vector3 pos = interact.transform.position;
                    Vector3 pos2 = controller.transform.position;

                    pos.y = 0;
                    pos2.y = 0;

                    Vector3 interactDirection = (pos - pos2).normalized;

                    controller.TargetRotation = controller.GetRotationOfDirection(interactDirection);

                    abilites[i].OnAbilityStart(controller, interact,interactDirection.MakeCardinal());
                    currentRunningAbility = i;
                    return true;
                }
            }

            return false;
        }

        public void ForceAbility(Interactalbe interactObject,ICreatureAbility abilityToForce)
        {
            for (int i = 0; i < abilites.Count; i++)
            {
                if(abilites[i] == abilityToForce)
                {
                    Vector3 pos = interactObject.transform.position;
                    Vector3 pos2 = controller.transform.position;

                    pos.y = 0;
                    pos2.y = 0;

                    Vector3 interactDirection = (pos - pos2).normalized;

                    currentRunningAbility = i;
                    abilites[i].OnAbilityStart(controller, interactObject,interactDirection.MakeCardinal());
                    machine.ChangeState(typeof(PlayerAbilityState));
                    return;
                }
            }
        }
    }
}
