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
    private bool initalized = false;

    private void OnEnable()
    {
        if(initalized) // Makes sure this can run only if start has been called
        triggerData?.LogTrigger(this);
    }

    private void OnDisable()
    {
        if(initalized)
        triggerData?.RemoveTrigger(this);
    }

    private void Start()
    {
        //Called here first so trigger data has time to clear the previous scenes data
        triggerData?.LogTrigger(this);
        initalized = true;
    }

    public void OnCustomTriggerEnter()
    {
        if (onlyOnce && activated) return;
        onTriggerEnter?.Raise();
        onTriggerEnterEvent.Invoke();
        activated = true;
    }
}
