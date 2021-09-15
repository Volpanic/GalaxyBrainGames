using GalaxyBrain.Creatures;
using UnityEngine;
using UnityEngine.Events;

namespace GalaxyBrain.Interactables
{
    public class Interactalbe : MonoBehaviour
    {
        [HideInInspector] public PlayerController lastInteractedWithCreature = null;
        [HideInInspector] public Collider myCollider = null;

        [SerializeField] private PlayerController.PlayerTypes requiredType;
        [SerializeField] private bool allTypes = false;
        [SerializeField] private bool onlyOnce = true;

        [SerializeField] private GameEvent OnInteractedEvent;
        [SerializeField] private UnityEvent OnInteracted;

        public PlayerController.PlayerTypes RequiredType
        {
            get
            {
                return requiredType;
            }
        }

        private bool activated;
        public void OnInteract(PlayerController creature)
        {
            if (IsRequiredType(creature.PlayerType))
            {
                if (onlyOnce && activated) return;
                creature.AttemptInteract(this);
                lastInteractedWithCreature = creature;
                OnInteracted.Invoke();
                activated = true;
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