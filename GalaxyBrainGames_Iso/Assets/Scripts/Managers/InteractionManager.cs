using GalaxyBrain.Creatures;
using GalaxyBrain.Interactables;
using GalaxyBrain.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyBrain.Managers
{
    public enum InteractionViability
    {
        NonViable,
        InteractableNotInRange,
        Interactable
    }

    public class InteractionManager : MonoBehaviour
    {
        [SerializeField] CreatureData creatureData;

        private Dictionary<GameObject, Interactalbe> interactions = new Dictionary<GameObject, Interactalbe>();

        public InteractionViability LookForInteractables(RaycastHit selectedObject)
        {
            if (creatureData == null) return InteractionViability.NonViable;

            PlayerController selectedCreature = creatureData.GetSelectedCreature();

            //Check if interaction is cached
            if (interactions.ContainsKey(selectedObject.collider.gameObject))
            {
                //Make sure we are the correct creature type.
                if(!interactions[selectedObject.collider.gameObject].IsRequiredType(selectedCreature.PlayerType))
                {
                    return InteractionViability.InteractableNotInRange;
                }

                //Check if were in the creatures default state and in range
                if (selectedCreature.InDefaultState && interactions[selectedObject.collider.gameObject].CheckIfNeaby(selectedCreature.gameObject, 1.05f))
                {
                    if(Input.GetMouseButtonDown(0))
                    {
                        interactions[selectedObject.collider.gameObject].AttemptInteract(selectedCreature);
                    }
                    return InteractionViability.Interactable;
                }
                return InteractionViability.InteractableNotInRange;
            }
            return InteractionViability.NonViable;
        }

        // Start is called before the first frame update
        private void Awake()
        {
            //Cache all interactables on creation for performance
            Interactalbe[] interacts = FindObjectsOfType<Interactalbe>();

            for (int i = 0; i < interacts.Length; i++)
            {
                interactions.Add(interacts[i].gameObject, interacts[i]);
            }
        }
    }
}