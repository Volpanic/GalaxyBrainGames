using GalaxyBrain.Systems;
using UnityEngine;

namespace GalaxyBrain.Creatures
{
    /// <summary>
    /// Controls swimming creature specific abilities and effects.
    /// </summary>
    public class SwimingCreature : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CreatureData creatureData;
        [SerializeField] private PlayerController controller;
        [SerializeField] private ControllerCarry controllerCarry;
        [SerializeField] private Collider myCollider;
        [SerializeField] private Transform worldModel;
        [SerializeField] private LayerMask waterMask;

        private void Awake()
        {
            creatureData.LogCreature(controller);
        }

        private void FixedUpdate()
        {
            controller.WeighedDown = controllerCarry.SteppedOn;
        }
    }
}
