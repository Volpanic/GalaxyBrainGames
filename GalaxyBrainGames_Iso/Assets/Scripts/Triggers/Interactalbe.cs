using UnityEngine;
using UnityEngine.Events;

public class Interactalbe : MonoBehaviour
{
    [HideInInspector] public PlayerController lastInteractedWithCreature = null;

    [SerializeField] private PlayerController.PlayerTypes requiredType;
    [SerializeField] private bool onlyOnce = true;

    [SerializeField] private GameEvent OnInteractedEvent;
    [SerializeField] private UnityEvent OnInteracted;

    private bool activated;

    public void OnInteract(PlayerController creature)
    {
        if (requiredType == creature.PlayerType)
        {
            if (onlyOnce && activated) return;
            creature.AttemptInteract(this);
            lastInteractedWithCreature = creature;
            OnInteracted.Invoke();
            activated = true;
        }
    }
}