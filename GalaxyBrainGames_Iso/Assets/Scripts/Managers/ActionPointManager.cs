using GalaxyBrain.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionPointManager : MonoBehaviour
{
    [SerializeField] private ActionPointData pointData;
    [SerializeField, Min(1)] private int actionPointCount = 15;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI actionPointTextCount;

    // Start is called before the first frame update
    void Awake()
    {
        pointData?.ResetActionPoints(actionPointCount);
        UpdateActionUI(actionPointCount);
    }

    private void OnEnable()
    {
        if(pointData != null) pointData.OnActionPointChanged += UpdateActionUI;
    }

    private void OnDisable()
    {
        if(pointData != null) pointData.OnActionPointChanged -= UpdateActionUI;
    }

    private void UpdateActionUI(int currentAmount)
    {
        if (actionPointTextCount == null) return;

        //Update UI
        actionPointTextCount.text = currentAmount.ToString();
    }
}
