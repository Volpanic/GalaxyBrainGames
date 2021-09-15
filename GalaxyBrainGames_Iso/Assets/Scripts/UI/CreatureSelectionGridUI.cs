using GalaxyBrain.Creatures;
using GalaxyBrain.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GalaxyBrain.UI
{
    public class CreatureSelectionGridUI : MonoBehaviour
    {
        [SerializeField] private HorizontalLayoutGroup layoutGroup;
        [SerializeField] private GameObject creatureUIBlockPrefab;
        [SerializeField] private CreatureData creatureData;

        private CreatureSelectionIcon[] creatureUIIcons;

        private void OnEnable()
        {
            if (creatureData == null || creatureData.CreatureManager == null) return;
            creatureData.CreatureManager.OnSelectedChanged += SelectedChanged;
        }

        private void OnDisable()
        {
            if (creatureData == null || creatureData.CreatureManager == null) return;
            creatureData.CreatureManager.OnSelectedChanged -= SelectedChanged;
        }

        private void SelectedChanged(int oldIndex, int newIndex)
        {
            if (creatureUIIcons == null) return;
            
            if(oldIndex != -1)
            {
                creatureUIIcons[oldIndex].OnDeselectCreature();
            }

            if (newIndex != -1)
            {
                creatureUIIcons[newIndex].OnSelectCreature();
            }
        }

        // Start is called before the first frame update
        private void Start()
        {
            OnEnable();

            //Make sure we have a creature manager
            if (creatureData != null && creatureUIBlockPrefab != null && layoutGroup != null)
            {
                //Get text base from prefab
                List<CreatureSelectionIcon> lastObject = new List<CreatureSelectionIcon>();

                //Edit the prefabs text and instantiate it
                InstansiateCreatureIconPrefabs(lastObject);

                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)layoutGroup.transform);
                creatureUIIcons = lastObject.ToArray();
                layoutGroup.enabled = false;

                //Setup icons effects, Done here so positions have updated after layout rebuild
                PlayerController selected = creatureData.GetSelectedCreature();
                for (int i = 0; i < lastObject.Count; i++)
                {
                    lastObject[i].SetupEffects();
                    //If this is the currently selected creature
                    if (lastObject[i] == selected)
                    {
                        lastObject[lastObject.Count - 1].OnSelectCreature();
                    }
                }
            }
        }

        private void InstansiateCreatureIconPrefabs(List<CreatureSelectionIcon> lastObject)
        {
            TextMeshProUGUI textBase = creatureUIBlockPrefab.GetComponentInChildren<TextMeshProUGUI>();
            Button button = creatureUIBlockPrefab.GetComponentInChildren<Button>();

            for (int i = 0; i < creatureData.GetCreatureCount(); i++)
            {
                PlayerController creature = creatureData.GetCreature(i);
                if (creature != null)
                {
                    textBase.text = creature.gameObject.name;


                    CreatureSelectionIcon icon = Instantiate(creatureUIBlockPrefab, layoutGroup.transform).GetComponent<CreatureSelectionIcon>();

                    Button iconButton = icon.GetComponentInChildren<Button>();

                    //Store loop i in scope variable, or else they will all be the same value
                    int iLoopValue = i;
                    iconButton.onClick.AddListener(() => {
                        creatureData.SetSelectedCreature(iLoopValue);
                    });

                    lastObject.Add(icon);
                }
            }
        }
    }
}