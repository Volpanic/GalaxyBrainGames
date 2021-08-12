using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildCreature : MonoBehaviour
{
    [SerializeField] private PlayerController controller;
    [SerializeField] private CreatureData creatureData;


    void Awake()
    {
        creatureData.LogCreature(controller);
    }

}
