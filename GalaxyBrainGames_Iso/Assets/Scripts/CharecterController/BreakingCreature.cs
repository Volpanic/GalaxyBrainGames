using GalaxyBrain.Creatures.Abilities;
using GalaxyBrain.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GalaxyBrain.Creatures
{
    public class BreakingCreature : MonoBehaviour
    {
        [SerializeField] private PlayerController controller;
        [SerializeField] private CreatureData creatureData;

        // Start is called before the first frame update
        void Awake()
        {
            creatureData.LogCreature(controller);
            controller.AddAbility(new PushBlockAbility());
            controller.AddAbility(new KnockOverPillarAbility());
            controller.AddAbility(new ClimbOnBackAbility(PlayerController.PlayerTypes.Water));
        }
    }
}