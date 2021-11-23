using GalaxyBrain.Audio;
using GalaxyBrain.Systems;
using UnityEngine;
using UnityEngine.Events;
using Volpanic.Easing;

namespace GalaxyBrain.Interactables
{
    /// <summary>
    /// Controls knocking over a pillar.
    /// </summary>
    [SelectionBase]
    public class Pushable : MonoBehaviour
    {
        [SerializeField] private Interactalbe interactalbe;
        [SerializeField] private CreatureData creatureData;
        [SerializeField] private Collider myCollider;
        [SerializeField] private UnityEvent onPushedEvent;

        [Header("Sounds")]
        [SerializeField] private AudioData onPushedSound;
        [SerializeField] private AudioData landedSound;

        private bool fallingOver = false;
        private float fallingTimer = 0;
        private bool hasPlayedLanadedSound = false;

        private Quaternion initalRotation;
        private Quaternion targetRotation;

        private void Awake()
        {
            initalRotation = transform.rotation;
        }

        private void FixedUpdate()
        {
            if (fallingOver)
            {
                fallingTimer += Time.fixedDeltaTime;
                float lerpPos = Easingf.InExpo(0,1,fallingTimer);
                transform.rotation = Quaternion.Lerp(initalRotation, targetRotation, lerpPos);

                if(!hasPlayedLanadedSound && fallingTimer >= 0.9)
                {
                    hasPlayedLanadedSound = true;
                    landedSound?.Play();
                }

                if (fallingTimer >= 1)
                {
                    fallingOver = false;
                    fallingTimer = 1;
                    transform.rotation = targetRotation;
                    creatureData.pathfinding.UpdateNodeCells(myCollider.bounds.min, myCollider.bounds.max);
                    enabled = false;
                }
            }
        }

        public void Push(Vector3 pushDirection)
        {
            if (pushDirection == Vector3.zero) return;

            onPushedSound?.Play();

            FaceDirection(pushDirection);
            targetRotation = GetPushRotation(pushDirection);
            fallingOver = true;
            onPushedEvent?.Invoke();
        }

        private void FaceDirection(Vector3 direction)
        {
            Vector3 tartgetEuler = Quaternion.LookRotation(direction,Vector3.up).eulerAngles;
            transform.rotation = Quaternion.Euler(tartgetEuler.x, tartgetEuler.y + 90, tartgetEuler.z);
            initalRotation = transform.rotation;
        }

        private Quaternion GetPushRotation(Vector3 pushDirection)
        {
            return Quaternion.FromToRotation(Vector3.up,pushDirection) * transform.rotation;
        }
    }
}