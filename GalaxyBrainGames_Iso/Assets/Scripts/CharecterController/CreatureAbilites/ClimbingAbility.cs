using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        return climb != null;
    }

    public void OnAbilityEnd()
    {
        climb = null;
    }

    public void OnAbilityStart(PlayerController controller, Interactalbe interactable, Vector3 interactDirection)
    {
        Debug.Log("Climb");
        controller.IsClimbing = true;
    }

    public void OnAbilityUpdate()
    {
        
    }
}
