using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlateExit : MonoBehaviour
{
    [SerializeField] private bool onlyOnce = true;

    [SerializeField] private GameEvent onTriggerExit;
    [SerializeField] private UnityEvent onTriggerExitEvent;

    private bool activated;

    public void OnTriggerExit()
    {
        if (onlyOnce && activated) return;
        onTriggerExit?.Raise();
        onTriggerExitEvent.Invoke();
        activated = true;
    }
}
