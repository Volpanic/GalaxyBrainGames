using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GalaxyBrain.Systems
{
    [CreateAssetMenu]
    public class ActionPointData : ScriptableObject
    {
        public GameEvent OutOfActionPoints;
        public Action<Vector3, int> OnActionPointChanged;

        private int currentActionPoint = 15;

        public void ResetActionPoints(int startingPoints)
        {
            currentActionPoint = startingPoints;
        }

        public void SubtractActionPoint(Vector3 consumePosition, int amount = 1)
        {
            if (amount > 0)
            {
                currentActionPoint -= amount;
                CheckIfOutOfPoints();
                currentActionPoint = Mathf.Max(0, currentActionPoint);
                OnActionPointChanged?.Invoke(consumePosition, currentActionPoint);
            }
        }

        private void CheckIfOutOfPoints()
        {
            if (currentActionPoint < 0)
            {
                //Raise the event to any event listeners
                OutOfActionPoints?.Raise();
            }
        }
    }
}