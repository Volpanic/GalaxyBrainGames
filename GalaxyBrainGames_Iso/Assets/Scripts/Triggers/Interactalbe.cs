using GalaxyBrain.Creatures;
using UnityEngine;
using UnityEngine.Events;

namespace GalaxyBrain.Interactables
{
    public class Interactalbe : MonoBehaviour
    {
        [HideInInspector] public PlayerController lastInteractedWithCreature = null;
        [HideInInspector] public Collider myCollider = null;

        [Header("Creature Info")]
        [SerializeField] private PlayerController.PlayerTypes requiredType;
        [SerializeField] private bool allTypes = false;
        [SerializeField] private bool onlyOnce = true;
        [SerializeField] private bool consumeActionPoint = false;

        [Header("Creature Animation Info")]
        [SerializeField] private bool setAnimatorBool = false;
        [SerializeField] private string animatorBoolName;
        [SerializeField,Range(0f,1f)] private float normlaizedTimeToActivate;
        [SerializeField] private string hardPlayAnimationName;

        [SerializeField] private GameEvent OnInteractedEvent;
        [SerializeField] private UnityEvent OnInteracted;

        private bool canBeInteractedWith = true;

        public PlayerController.PlayerTypes RequiredType
        {
            get
            {
                return requiredType;
            }
        }

        private bool activated;
        public void AttemptInteract(PlayerController creature)
        {
            if (IsRequiredType(creature.PlayerType))
            {
                if (onlyOnce && activated) return;
                creature.AttemptInteract(this);
                
            }
        }

        public void Interact(PlayerController player)
        {
            if (canBeInteractedWith && player.InDefaultState)
            {
                lastInteractedWithCreature = player;

                if(setAnimatorBool)
                {
                    canBeInteractedWith = false;

                    if (animatorBoolName != string.Empty)
                    {
                        canBeInteractedWith = false;

                        player.AnimationEvent(this, animatorBoolName, normlaizedTimeToActivate, (x) =>
                           {
                               OnInteracted.Invoke();
                               activated = true;
                               canBeInteractedWith = true;
                               if (consumeActionPoint) x.ConsumeActionPoint();
                           });
                    }
                    else
                    {
                        player.HardPlayAnimationEvent(this, hardPlayAnimationName, normlaizedTimeToActivate, (x) =>
                        {
                            OnInteracted.Invoke();
                            activated = true;
                            canBeInteractedWith = true;
                            if (consumeActionPoint) x.ConsumeActionPoint();
                        });
                    }
                }
                else
                {
                    OnInteracted.Invoke();
                    activated = true;
                    if (consumeActionPoint) player.ConsumeActionPoint();
                }
            }
        }

        public bool IsRequiredType(PlayerController.PlayerTypes playerType)
        {
            return allTypes || requiredType == playerType;
        }

        public bool CheckIfNeaby(GameObject obj, float maxDistance)
        {
            Vector3 closestPoint = myCollider.ClosestPoint(obj.transform.position);

            if (Vector3.Distance(closestPoint, obj.transform.position) <= maxDistance)
            {
                return true;
            }
            return false;
        }

        private void Awake()
        {
            if (myCollider == null) myCollider = GetComponent<Collider>();
        }
    }
}