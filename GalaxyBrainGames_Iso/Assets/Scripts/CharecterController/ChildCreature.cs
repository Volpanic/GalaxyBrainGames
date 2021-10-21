using GalaxyBrain.Systems;
using UnityEngine;

namespace GalaxyBrain.Creatures
{
    /// <summary>
    /// Controls child creature specific abilities and effects.
    /// </summary>
    public class ChildCreature : MonoBehaviour
    {
        [SerializeField] private PlayerController controller;
        [SerializeField] private CreatureData creatureData;

        void Awake()
        {
            creatureData.LogCreature(controller);
        }
    }
}