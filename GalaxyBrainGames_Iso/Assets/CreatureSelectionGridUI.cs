using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureSelectionGridUI : MonoBehaviour
{
    [SerializeField] private HorizontalLayoutGroup layoutGroup;
    [SerializeField] private GameObject creatureUIBlockPrefab;
    [SerializeField] private CreatureData creatureData;

    private float initalYPos = 0;
    private GameObject[] creatureUIIcons;

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

    private void SelectedChanged(int index)
    {
        if (creatureUIIcons == null) return;

        Vector3 pos;

        for (int i = 0; i < creatureUIIcons.Length; i++)
        {
            pos = ((RectTransform)creatureUIIcons[i].transform).position;
            if (i == index)
            {
                pos.y = initalYPos;
            }
            else
            {
                pos.y = initalYPos - 32;
            }
            ((RectTransform)creatureUIIcons[i].transform).position = pos;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        OnEnable();

        //Make sure we have a creature manager
        if (creatureData != null && creatureUIBlockPrefab != null && layoutGroup != null)
        {
            //Get text base from prefab
            Text textBase = creatureUIBlockPrefab.GetComponentInChildren<Text>();
            List<GameObject> lastObject = new List<GameObject>();


            //Edit the prefabs text and instantiate it
            for (int i = 0; i < creatureData.GetCreatureCount(); i++)
            {
                PlayerController creature = creatureData.GetCreature(i);
                if (creature != null)
                {
                    textBase.text = creature.gameObject.name;
                    lastObject.Add(Instantiate(creatureUIBlockPrefab, layoutGroup.transform));
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)layoutGroup.transform);

            if (lastObject.Count >= 0 && lastObject[0] != null) initalYPos = ((RectTransform)lastObject[0].transform).position.y;
            creatureUIIcons = lastObject.ToArray();

            layoutGroup.enabled = false;
        }
    }
}
