using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventTrigger : MonoBehaviour
{

    [SerializeField] private bool onlyOnce = true;

    [SerializeField] private GameEvent onTriggerEnter;
    [SerializeField] private UnityEvent onTriggerEnterEvent;

    private bool activated;

    public void OnTriggerEnter()
    {
        if (onlyOnce && activated) return;
        onTriggerEnter?.Raise();
        onTriggerEnterEvent.Invoke();
        activated = true;
    }
}
