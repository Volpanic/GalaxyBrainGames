using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildCreature : MonoBehaviour
{
    [SerializeField] private FreeMoveController controller;
    [SerializeField] private LayerMask keyMask;
    [SerializeField] private CreatureData creatureData;


    private bool hasKey = false;

    void Awake()
    {
        creatureData.LogCreature(controller);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (!hasKey)
            {
                CheckForKeys();
            }
            else
            {
                CheckForLockedDoors();
            }
        }
    }

    private void CheckForLockedDoors()
    {
        Collider[] doors = Physics.OverlapSphere(transform.position, 3, keyMask);
        if (doors != null && doors.Length > 0)
        {
            hasKey = true;

            for (int i = 0; i < doors.Length; i++)
            {
                LockedDoor g = doors[i].GetComponent<LockedDoor>();

                if (g != null)
                {
                    Destroy(doors[i].gameObject);
                }
            }
        }
    }

    private void CheckForKeys()
    {
        Collider[] keys = Physics.OverlapSphere(transform.position, 3, keyMask);
        if(keys != null && keys.Length > 0)
        {
            hasKey = true;

            for(int i = 0; i < keys.Length; i++)
            {
                Destroy(keys[i].gameObject);
            }
        }
    }
}
