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
        [SerializeField] private Vector3 velocity;

        private Vector3 oldPos;
        private Dictionary<GameObject, CharacterController> cachedControllers = new Dictionary<GameObject, CharacterController>();
        [SerializeField, ReadOnly] private List<CharacterController> passengers = new List<CharacterController>();

        public bool SteppedOn
        {
            get { return passengers.Count > 0; }
        }

        public bool IsBeingCarried(CharacterController controllerToCheck)
        {
            for(int i = 0; i < passengers.Count; i++)
            {
                if (passengers[i] == controllerToCheck) return true;
            }
            return false;
        }

        private void Start()
        {
            if (controller == null) enabled = false;
            oldPos = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            velocity = controller.velocity;
            FindPassengers();
            MovePassengers();

            oldPos = transform.position;
        }

        private void MovePassengers()
        {
            Vector3 posDifference = transform.position - oldPos;
            if (Mathf.Abs(posDifference.x) <= 0.01f && Mathf.Abs(posDifference.z) <= 0.01f) return;

            if (controller.velocity.magnitude != 0)
            {
                for (int i = 0; i < passengers.Count; i++)
                {
                    Vector3 movement = (controller.bounds.center + new Vector3(0, controller.bounds.extents.y, 0)) -
                        (passengers[i].bounds.center - new Vector3(0, passengers[i].bounds.extents.y, 0));
                    passengers[i].Move(movement);
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