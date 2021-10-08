using GalaxyBrain.Audio;
using GalaxyBrain.Systems;
using GalaxyBrain.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyBrain.Effects
{
    [RequireComponent(typeof(RectTransform))]
    public class ActionPointConsumedEffect : MonoBehaviour
    {
        [SerializeField] private RectTransform canvas;
        [SerializeField] private ActionPointData actionPointData;
        [SerializeField] private TextRiseEffect textRisePrefab;
        [SerializeField] private AudioData effectSound;

        private List<TextRiseEffect> effectPool;
        private int effectIndex = 0;

        private Camera cam;

        private void Awake()
        {
            CreateEffectPool(10);
            cam = Camera.main; // Cache main camera on awake
        }

        private void Start()
        {
            AddEffectToAPEvent();
        }

        private void CreateEffectPool(int size)
        {
            effectPool = new List<TextRiseEffect>();

            for (int i = 0; i < size; i++)
            {
                effectPool.Add(Instantiate(textRisePrefab,transform));
                effectPool[i].gameObject.SetActive(false);
                effectPool[i].SetText("-1", Color.white,Vector3.zero);
            }
        }

        private void OnEnable()
        {
            AddEffectToAPEvent();
        }

        private void OnDisable()
        {
            RemoveEffectToAPEvent();
        }

        private void AddEffectToAPEvent()
        {
            actionPointData.OnActionPointChanged += CreateEffect;
        }

        private void RemoveEffectToAPEvent()
        {
            actionPointData.OnActionPointChanged -= CreateEffect;
        }

        private void CreateEffect(Vector3 position, int amount)
        {
            TextRiseEffect effect = GetEffectInstance();
            if (effect == null) return;

            Vector3 newPosition = WorldPosToCanvasPos(position);

            effect.SetText("-1",Color.white, newPosition);
            effect.gameObject.SetActive(true);

            effectSound?.Play();
        }

        private Vector2 WorldPosToCanvasPos(Vector3 worldPos)
        {
            if (cam == null) return Vector2.zero;
            return cam.WorldToScreenPoint(worldPos);
        }

        private TextRiseEffect GetEffectInstance()
        {
            TextRiseEffect effect = effectPool[effectIndex++];

            WrapEffectIndex();

            return effect;
        }

        private void WrapEffectIndex()
        {
            if (effectIndex >= effectPool.Count) effectIndex = 0;
        }
    }
}
