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
        [SerializeField] private CreatureData creatureData;
        [SerializeField] private TextRiseEffect textRisePrefab;

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
            AddEffectToPathEvent();
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
            AddEffectToPathEvent();
        }

        private void OnDisable()
        {
            RemoveEffectToPathEvent();
        }

        private void AddEffectToPathEvent()
        {
            if (creatureData != null)
            {
                foreach (Creatures.PlayerController creature in creatureData.CreaturesInLevel)
                {
                    creature.OnPathInterval += CreateEffect;
                }
            }
        }

        private void RemoveEffectToPathEvent()
        {
            if (creatureData != null)
            {
                foreach (Creatures.PlayerController creature in creatureData.CreaturesInLevel)
                {
                    creature.OnPathInterval -= CreateEffect;
                }
            }
        }

        private void CreateEffect(Vector3 oldPath, Vector3 newPath)
        {
            TextRiseEffect effect = GetEffectInstance();
            Vector3 newPosition = WorldPosToCanvasPos(newPath);

            effect.SetText("-1",Color.white, newPosition);
            effect.gameObject.SetActive(true);
        }

        private Vector2 WorldPosToCanvasPos(Vector3 worldPos)
        {
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
