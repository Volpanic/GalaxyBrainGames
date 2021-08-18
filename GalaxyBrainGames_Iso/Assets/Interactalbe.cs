using UnityEngine;
using UnityEngine.Events;

public class Interactalbe : MonoBehaviour
{

    [SerializeField] private bool onlyOnce = true;

    [SerializeField] private GameEvent OnInteractedEvent;
    [SerializeField] private UnityEvent OnInteracted;

    private bool activated;

    public void OnInteract(PlayerController creature)
    {
        if (onlyOnce && activated) return;
        OnInteractedEvent?.Raise();
        OnInteracted.Invoke();
        activated = true;
    }
}
