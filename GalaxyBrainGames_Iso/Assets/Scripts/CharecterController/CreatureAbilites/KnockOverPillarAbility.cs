using GalaxyBrain.Creatures;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockOverPillarAbility : ICreatureAbility
{
    private Pushable pillar;

    public bool OnAbilityCheckCondition(Interactalbe interactable)
    {
        pillar = interactable.GetComponent<Pushable>();

        return pillar != null;
    }

    public bool OnAbilityCheckDone()
    {
        return pillar != null;
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
