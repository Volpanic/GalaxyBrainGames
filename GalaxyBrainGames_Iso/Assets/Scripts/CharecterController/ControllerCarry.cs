using GalaxyBrain.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyBrain.Creatures
{
    public class ControllerCarry : MonoBehaviour
    {
        [SerializeField] private CharacterController controller;

        private Dictionary<GameObject, CharacterController> cachedControllers = new Dictionary<GameObject, CharacterController>();
        [SerializeField, ReadOnly] private List<CharacterController> passengers = new List<CharacterController>();

        private void Start()
        {
            if (controller == null) enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            FindPassengers();
            MovePassengers();
        }

        private void MovePassengers()
        {
            if (controller.velocity.magnitude != 0)
            {
                for (int i = 0; i < passengers.Count; i++)
                {
                    passengers[i].Move(controller.velocity * Time.deltaTime);
                }
            }
        }

        private void FindPassengers()
        {
            Collider[] colls = Physics.OverlapBox(controller.bounds.center + (Vector3.up * 0.5f), controller.bounds.extents, transform.rotation);
            passengers.Clear();

            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].gameObject == gameObject) continue;

                CharacterController cc = null;
                if (!cachedControllers.ContainsKey(colls[i].gameObject))
                {
                    cc = colls[i].GetComponent<CharacterController>();
                    cachedControllers.Add(colls[i].gameObject, cc);
                }
                else cc = cachedControllers[colls[i].gameObject];

                if (cc != null) passengers.Add(cc);
            }
        }
    }
}