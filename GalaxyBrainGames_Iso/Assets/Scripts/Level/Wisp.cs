using GalaxyBrain.Utility.Extnesion;
using UnityEngine;

namespace GalaxyBrain
{
    /// <summary>
    /// Slowly float the wisp object up and down
    /// </summary>
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
