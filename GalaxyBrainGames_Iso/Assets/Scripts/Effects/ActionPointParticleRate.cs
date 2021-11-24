using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Emits more particles as AP is drained
/// </summary>
namespace GalaxyBrain.Effects
{
    public class ActionPointParticleRate : MonoBehaviour
    {
        [SerializeField] private ActionPointManager apManager;
        [SerializeField] private ParticleSystem particleSystem;
        [SerializeField] private AnimationCurve emissionAmountCurve;

        private int initalAPValue;

        private void Start()
        {
            //Set particle emission rate to inital amount
            if (apManager != null)
            {
                initalAPValue = apManager.CurrentActionPoints;
                UpdateParticleEmissionAmount(Vector3.zero, apManager.CurrentActionPoints);
            }
        }

        void OnEnable()
        {
            if (apManager != null)
            {
                apManager.PointData.OnActionPointChanged += UpdateParticleEmissionAmount;
            }
        }

        void OnDisable()
        {
            if (apManager != null)
            {
                apManager.PointData.OnActionPointChanged -= UpdateParticleEmissionAmount;
            }
        }

        private void UpdateParticleEmissionAmount(Vector3 position, int currentActionPoint)
        {
            if (apManager == null) return;

            // Sample to curve
            float normalizedPosition = (currentActionPoint / (float)initalAPValue);
            float sampledAmount = emissionAmountCurve.Evaluate(1 - normalizedPosition);

            // Update particle system
            ParticleSystem.EmissionModule emission = particleSystem.emission;
            emission.rateOverTime = sampledAmount;
        }
    }
}
