using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class SceneTriggerData : ScriptableObject
{
    //Data
    private Dictionary<GameObject, CustomTrigger> triggerLookup;

    //Getting / Setting functions
    public void LogTrigger(CustomTrigger trigger)
    {
        if(!triggerLookup.ContainsKey(trigger.gameObject))
            triggerLookup.Add(trigger.gameObject, trigger);
    }

    public void RemoveTrigger(CustomTrigger trigger)
    {
        if (triggerLookup.ContainsKey(trigger.gameObject))
            triggerLookup.Remove(trigger.gameObject);
    }

    public CustomTrigger GetTrigger(GameObject obj)
    {
        return triggerLookup[obj];
    }

    //Clean data on inital startup
    private void OnEnable()
    {
        CleanData();
        SceneManager.activeSceneChanged += SceneChanged;
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= SceneChanged;
    }

    //Resets the stored data when scene is restarted
    private void SceneChanged(Scene arg0, Scene arg1)
    {
        //CleanData();
    }

    private void CleanData()
    {
        triggerLookup = new Dictionary<GameObject, CustomTrigger>();
    }

    public void TriggerDetection(Collider myCollider)
    {
        foreach(KeyValuePair<GameObject, CustomTrigger> trigger in triggerLookup)
        {
            if (myCollider.bounds.Intersects(trigger.Value.TriggerCollider.bounds))
            {
                trigger.Value.OnCustomTriggerEnter();
            }
        }
    }
}
