using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureAnchorPoint : MonoBehaviour
{
    private PlayerController attachedCreature;


    public bool AttemptAttach(PlayerController toAttach,Vector3 offsetFromAttachPoint)
    {
        if(attachedCreature == null)
        {
            attachedCreature = toAttach;

            //Attach the gosh darn thing
            attachedCreature.transform.parent = this.transform;
            attachedCreature.transform.localPosition = offsetFromAttachPoint;
            attachedCreature.AnchordToo = this;

            return true;
        }

        return false;
    }

    public void DetachCurrent()
    {
        if (attachedCreature != null)
        {
            attachedCreature.transform.parent = null;
            attachedCreature.AnchordToo = null;
        }

        attachedCreature = null;
    }
}
