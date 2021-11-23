using GalaxyBrain.Audio;
using GalaxyBrain.Creatures.Abilities;
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

        // Start is called before the first frame update
        void Awake()
        {
            creatureData.LogCreature(controller);
        }

        private void Start()
        {
            PushBlockAbility pushAbility = new PushBlockAbility();
            KnockOverPillarAbility pillarAbility = new KnockOverPillarAbility();

            pushAbility.AbilityStartSound = roarSound;
            pillarAbility.AbilityStartSound = roarSound;

            controller.AddAbility(pushAbility);
            controller.AddAbility(pillarAbility);
        }
    }
}