﻿using GalaxyBrain.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorControl : MonoBehaviour
{
    public GameObject element;
    public bool isOpen = false;
    public CreatureData data;
    public Collider myCollider;

    public void Both()
    {
        if (isOpen)
        {
            element.SetActive(true);
            isOpen = false;
            data.pathfinding?.UpdateNodeCells(myCollider.bounds.min,myCollider.bounds.max);
        }
        else
        {
            element.SetActive(false);
            isOpen = true;
            data.pathfinding?.UpdateNodeCells(myCollider.bounds.min, myCollider.bounds.max);
        }
    }
}
