using GalaxyBrain.Creatures.Abilities;
using GalaxyBrain.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyBrain.Creatures
{
    public class ChildCreature : MonoBehaviour
    {
        [SerializeField] private PlayerController controller;
        [SerializeField] private CreatureData creatureData;

        void Awake()
        {
            creatureData.LogCreature(controller);
            controller.AddAbility(new ClimbingAbility());
            controller.AddAbility(new ClimbOnBackAbility(PlayerController.PlayerTypes.Water));
        }

    }
}