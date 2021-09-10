using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GalaxyBrain.Interactables
{
    public class GameEventTrigger : MonoBehaviour
    {
        [SerializeField] private bool onlyOnce = true;

        [SerializeField] private GameEvent onTriggerEnter;
        [SerializeField] private UnityEvent onTriggerEnterEvent;

        private bool activated;

        private void OnTriggerEnter(Collider other)
        {
            if (onlyOnce && activated) return;
            onTriggerEnter?.Raise();
            onTriggerEnterEvent.Invoke();
            activated = true;
        }
    }
}