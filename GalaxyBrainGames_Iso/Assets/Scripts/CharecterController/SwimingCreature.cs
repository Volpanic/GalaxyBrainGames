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
        [SerializeField] private GameObject waterParticles;

        [Header("Settings")]
        [SerializeField] private float waterSubmergeDepth = 0.5f;
        [SerializeField] private float weighedDownDepth = 0.1f;

        private Vector3 worldModelOriginalPos;
        private bool submerge = false;
        
        public bool Swimming
        {
            get { return submerge; }
        }

        private void Awake()
        {
            creatureData.LogCreature(controller);

            worldModelOriginalPos = worldModel.localPosition;
        }

        private void OnEnable()
        {
            controller.OnPathInterval += PathInterval;
        }

        private void OnDisable()
        {
            controller.OnPathInterval -= PathInterval;
        }

        private void PathInterval(Vector3 pos, Vector3 nextPos)
        {
            submerge = false;

            if (Physics.CheckBox(nextPos + (Vector3.down*0.1f),myCollider.bounds.extents ,transform.rotation,waterMask,QueryTriggerInteraction.Collide))
            {
                submerge = true;
            }
        }

        private void FixedUpdate()
        {
            if (submerge)
            {
                Vector3 targetPos = worldModelOriginalPos + (Vector3.up * waterSubmergeDepth);
                if (controller.WeighedDown) targetPos += Vector3.down * weighedDownDepth;
                worldModel.localPosition = Vector3.MoveTowards(worldModel.localPosition, targetPos,
                    Time.fixedDeltaTime * 4);
            }
            else
            {
                worldModel.localPosition = Vector3.MoveTowards(worldModel.localPosition, worldModelOriginalPos,
                    Time.fixedDeltaTime * 4);
            }

            controller.WeighedDown = controllerCarry.SteppedOn;
        }
    }
}
