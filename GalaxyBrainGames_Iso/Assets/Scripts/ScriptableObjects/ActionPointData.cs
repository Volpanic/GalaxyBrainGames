using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class ActionPointData : ScriptableObject
{
    public GameEvent OutOfActionPoints;
    public Action<int> OnActionPointChanged;

    private int currentActionPoint = 15;

    public void ResetActionPoints(int startingPoints)
    {
        currentActionPoint = startingPoints;
    }

    public void SubtractActionPoint(int amount = 1)
    {
        currentActionPoint = Mathf.Max(0, currentActionPoint - amount);
        OnActionPointChanged?.Invoke(currentActionPoint);
        CheckIfOutOfPoints();
    }

    private void CheckIfOutOfPoints()
    {
        if(currentActionPoint <= 0)
        {
            //Raise the event to any event listeners
            OutOfActionPoints?.Raise();
        }
    }
}