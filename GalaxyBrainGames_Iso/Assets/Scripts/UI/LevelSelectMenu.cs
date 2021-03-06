using GalaxyBrain.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        [SerializeField] private Sprite lockedIcon;

        [Header("Reference")]
        [SerializeField] private GameObject levelIconPrefab;
        [SerializeField] private GridLayoutGroup layout;
        [SerializeField] private LevelInfoBox infoBox;

        private Sprite unlockedIcon;

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
                    Image iconSprite = icon.GetComponent<Image>();
                    TextMeshProUGUI text = icon.GetComponentInChildren<TextMeshProUGUI>();

                    bool locked = i > saveData.Data.MaxLevelCompleted;

                    if(unlockedIcon == null  && !locked)
                    {
                        unlockedIcon = iconSprite.sprite;
                    }

                    //Change name if level is unlocked
                    if (locked)
                    {
                        text.text = "";
                        iconSprite.sprite = lockedIcon;
                    }
                    else
                    {
                        text.text = (i+1).ToString();

                        // Store i in num so it can be used in the delegate
                        // would return the value i is at the end of loop otherwise
                        int num = i;
                        //Set button to go to certain floor
                        button.onClick.RemoveAllListeners();
                        //button.onClick.AddListener(delegate { levelProgression.SetCurrentScene(num - 1); });
                        button.onClick.AddListener(delegate { infoBox?.SetInfo(num+1, levelProgression.ScenesInOrder[num], locked); });
                    }
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)layout.transform);
                layout.enabled = false;
            }
        }
    }
}