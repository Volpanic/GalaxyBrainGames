using GalaxyBrain.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GalaxyBrain.UI
{
    public class LevelSelectMenu : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private SaveData saveData;
        [SerializeField] private LevelProgression levelProgression;
        [SerializeField] private GameEvent nextLevelEvent;

        [Header("Reference")]
        [SerializeField] private GameObject levelIconPrefab;
        [SerializeField] private GridLayoutGroup layout;

        private void Start()
        {
            if (saveData != null && levelProgression != null)
            {
                if (levelIconPrefab != null)
                {
                    CreateGridCells();
                }
            }
        }

        private void CreateGridCells()
        {
            if (layout != null)
            {
                for (int i = 0; i < levelProgression.ScenesInOrder.Count; i++)
                {
                    GameObject icon = Instantiate(levelIconPrefab, layout.transform);
                    Button button = icon.GetComponent<Button>();
                    Text text = icon.GetComponentInChildren<Text>();

                    //Change name if level is unlocked
                    if (i > saveData.Data.MaxLevelCompleted)
                    {
                        text.text = "Locked";
                    }
                    else
                    {
                        text.text = i.ToString();

                        // Store i in num so it can be used in the delegate
                        // would return the value i is at the end of loop otherwise
                        int num = i;
                        //Set button to go to certain floor
                        button.onClick.RemoveAllListeners();
                        button.onClick.AddListener(delegate { levelProgression.SetCurrentScene(num - 1); });
                        button.onClick.AddListener(delegate { nextLevelEvent.Raise(); });
                    }
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)layout.transform);
                layout.enabled = false;
            }
        }
    }
}