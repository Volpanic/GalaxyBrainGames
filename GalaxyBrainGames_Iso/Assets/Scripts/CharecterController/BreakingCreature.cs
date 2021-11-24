using GalaxyBrain.Audio;
using GalaxyBrain.Creatures.Abilities;
using GalaxyBrain.Creatures.States;
using GalaxyBrain.Systems;
using UnityEngine;

namespace GalaxyBrain.Creatures
{
    /// <summary>
    /// Controls breaking creature specific abilities and effects.
    /// </summary>
    public class BreakingCreature : MonoBehaviour
    {
        [SerializeField] private PlayerController controller;
        [SerializeField] private CreatureData creatureData;
        [SerializeField] private AudioData roarSound;

        private bool hasPlayedRoar = true;

        // Start is called before the first frame update
        void Awake()
        {
            creatureData.LogCreature(controller);
        }

        private void Start()
        {
            controller.AddAbility(new PushBlockAbility());
            controller.AddAbility(new KnockOverPillarAbility());
        }

        private void FixedUpdate()
        {
            if (controller.InDefaultState)
            {
                hasPlayedRoar = false;
            }
            else  if(!hasPlayedRoar && controller.IsInState(typeof(PlayerAbilityState)))
            {
                hasPlayedRoar = true;
                roarSound?.Play();
            }
        }
    }
}