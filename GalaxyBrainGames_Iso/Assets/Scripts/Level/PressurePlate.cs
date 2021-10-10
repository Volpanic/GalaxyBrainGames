using GalaxyBrain.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GalaxyBrain.Interactables
{
    public class PressurePlate : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField] private GameEvent onPlatePressed;
        [SerializeField] private GameEvent onPlateReleased;

        [SerializeField] private UnityEvent onPlatePressedEvent;
        [SerializeField] private UnityEvent onPlateReleasedEvent;

        [Header("Sounds")]
        [SerializeField] private AudioData onPressedSound;
        [SerializeField] private AudioData onReleasedSound;

        private Collider myCollider;
        private bool pressedDown = false;
        private int startCollisionCount = 0;

        //Is a layermask with every layer enabled
        private const int ALL_LAYERS = ~0;

        private void Awake()
        {
            myCollider = GetComponent<Collider>();

            // Most likely 1, we keep track of this because
            // the pressure plate is most likely in the ground slightly
            // so this lets us ignore it in the collision check.
            startCollisionCount = GetCollidersTouching();
        }

        private int GetCollidersTouching()
        {
            return Physics.OverlapBox(myCollider.bounds.center, myCollider.bounds.extents, Quaternion.identity, ALL_LAYERS, QueryTriggerInteraction.Ignore).Length;
        }

        private void FixedUpdate()
        {
            if(GetCollidersTouching() > startCollisionCount)
            {
                if(!pressedDown)
                {
                    onPressedSound?.Play();
                    onPlatePressed?.Raise();
                    onPlatePressedEvent.Invoke();
                    pressedDown = true;
                }
            }
            else
            {
                if (pressedDown)
                {
                    onReleasedSound?.Play();
                    onPlateReleased?.Raise();
                    onPlateReleasedEvent.Invoke();
                    pressedDown = false;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            
        }

        private void OnTriggerExit(Collider other)
        {

        }
    }
}
