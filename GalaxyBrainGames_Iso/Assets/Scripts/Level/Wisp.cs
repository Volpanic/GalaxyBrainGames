using GalaxyBrain.Utility.Extnesion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyBrain
{
    public class Wisp : MonoBehaviour
    {
        [SerializeField] private float floatRange = 0.25f;
        [SerializeField] private float floatDuration = 4f;

        private Vector3 startPosition;
        private float timer = 0;

        private void Start()
        {
            startPosition = transform.localPosition;
        }

        private void Update()
        {
            timer += Time.deltaTime;

            float newY = startPosition.y;

            newY = newY.SinWave(startPosition.y- floatRange, startPosition.y+ floatRange, floatDuration, 0,timer);

            transform.localPosition = new Vector3(startPosition.x, newY, startPosition.z);
        }
    }
}
