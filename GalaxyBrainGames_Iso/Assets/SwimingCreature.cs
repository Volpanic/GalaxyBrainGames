using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwimingCreature : MonoBehaviour
{
    [SerializeField] private CreatureData creatureData;
    [SerializeField] private PlayerController controller;

    private void Awake()
    {
        creatureData.LogCreature(controller);
    }
}
