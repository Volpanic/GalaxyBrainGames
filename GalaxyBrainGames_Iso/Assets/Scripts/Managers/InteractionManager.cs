using GalaxyBrain.Creatures;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    [SerializeField] CreatureData creatureData;

    private Dictionary<GameObject, Interactalbe> interactions = new Dictionary<GameObject, Interactalbe>();
    private Camera cam;

    // Start is called before the first frame update
    void Awake()
    {
        cam = Camera.main;

        //Cache all interactables on creation for performance
        Interactalbe[] interacts = FindObjectsOfType<Interactalbe>();

        for(int i = 0; i < interacts.Length; i++)
        {
            interactions.Add(interacts[i].gameObject, interacts[i]);
        }
    }

    public bool LookForInteractables(RaycastHit selectedObject)
    {
        if (creatureData == null) return false;

        PlayerController selectedCreature = creatureData.GetSelectedCreature();

        if(interactions.ContainsKey(selectedObject.collider.gameObject))
        {
            if(Input.GetMouseButtonDown(0) && 
                interactions[selectedObject.collider.gameObject].CheckIfNeaby(selectedCreature.gameObject, 1.25f))
            {
                interactions[selectedObject.collider.gameObject].OnInteract(selectedCreature);
            }
            return true;
        }
        return false;
    }
}
