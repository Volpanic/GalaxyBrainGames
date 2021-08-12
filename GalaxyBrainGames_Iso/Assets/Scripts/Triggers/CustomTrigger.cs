using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomTrigger : MonoBehaviour
{
    public Collider TriggerCollider;

    [SerializeField] private SceneTriggerData triggerData;
    [SerializeField] private bool onlyOnce = true;

    [SerializeField] private GameEvent onTriggerEnter;
    [SerializeField] private UnityEvent onTriggerEnterEvent;

    private bool activated;

    private void OnEnable()
    {
        triggerData?.LogTrigger(this);
    }

    private void OnDisable()
    {
        triggerData?.RemoveTrigger(this);
    }

    public void OnCustomTriggerEnter()
    {
        if (onlyOnce && activated) return;
        onTriggerEnter?.Raise();
        onTriggerEnterEvent.Invoke();
        activated = true;
    }
}
