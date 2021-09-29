using GalaxyBrain.Creatures;
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

        [Header("Creature")]
        [SerializeField] private bool OnlyCreatures = false;
        [SerializeField] private PlayerController.PlayerTypes creatureType;

        private bool activated;

        private void OnTriggerEnter(Collider other)
        {
            if (onlyOnce && activated) return;

            if (!OnlyCreatures)
            {
                onTriggerEnter?.Raise();
                onTriggerEnterEvent.Invoke();
                activated = true;
            }
            else
            {
                PlayerController controller = other.GetComponent<PlayerController>();

                if(controller != null && controller.PlayerType == creatureType)
                {
                    onTriggerEnter?.Raise();
                    onTriggerEnterEvent.Invoke();
                    activated = true;
                }
            }
        }
    }
}