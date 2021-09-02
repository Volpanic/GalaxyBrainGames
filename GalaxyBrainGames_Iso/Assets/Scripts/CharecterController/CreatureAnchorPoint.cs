using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyBrain.Creatures
{
    public class CreatureAnchorPoint : MonoBehaviour
    {
        private GameObject attachedCreature;


        public bool AttemptAttach(GameObject toAttach, Vector3 offsetFromAttachPoint)
        {
            if (attachedCreature == null)
            {
                attachedCreature = toAttach;

                //Attach the gosh darn thing
                attachedCreature.transform.parent = this.transform;
                attachedCreature.transform.localPosition = offsetFromAttachPoint;

                return true;
            }

            return false;
        }

        public void DetachCurrent()
        {
            if (attachedCreature != null)
            {
                attachedCreature.transform.parent = null;
            }

            attachedCreature = null;
        }
    }
}