using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CreatureData : ScriptableObject
{
    public List<PlayerController> CreaturesInLevel = new List<PlayerController>();
    public CreatureManager CreatureManager;

    public Dictionary<string, bool> PlayerInventory;

    private void OnEnable()
    {
        CreaturesInLevel = new List<PlayerController>();
    }

    public int GetCreatureCount()
    {
        if (CreaturesInLevel == null) { return 0; }

        return CreaturesInLevel.Count;
    }

    public PlayerController GetCreature(int index)
    {
        if (CreaturesInLevel == null && CreaturesInLevel.Count <= 0) { return null; }

        return CreaturesInLevel[index];
    }

    public void LogCreature(PlayerController creature)
    {
        if (CreaturesInLevel == null) CreaturesInLevel = new List<PlayerController>();
        CreaturesInLevel.Add(creature);

        CleanCreatureData();
    }

    private void CleanCreatureData()
    {
        for(int i = 0; i < CreaturesInLevel.Count; i++)
        {
            if(CreaturesInLevel[i] == null)
            {
                CreaturesInLevel.RemoveAt(i);
                i--;
            }
        }
    }

    public void LogManager(CreatureManager manger)
    {
        CreatureManager = manger;
    }

    public void AddItemToInventory(string item)
    {
        PlayerInventory.Add(item, true);
    }

    public bool CheckAndComsumeItem(string item)
    {
        if(PlayerInventory.ContainsKey(item))
        {
            PlayerInventory.Remove(item);
            return true;
        }

        return false;
    }
}
