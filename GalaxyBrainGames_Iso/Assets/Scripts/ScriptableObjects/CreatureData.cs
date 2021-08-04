using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CreatureData : ScriptableObject
{
    public List<FreeMoveController> CreaturesInLevel = new List<FreeMoveController>();
    public CreatureManager CreatureManager;

    public Dictionary<string, bool> PlayerInventory;

    private void OnEnable()
    {
        Debug.Log("DataEnable");
        CreaturesInLevel = new List<FreeMoveController>();
    }

    public int GetCreatureCount()
    {
        if (CreaturesInLevel == null) { return 0; }

        return CreaturesInLevel.Count;
    }

    public FreeMoveController GetCreature(int index)
    {
        if (CreaturesInLevel == null && CreaturesInLevel.Count <= 0) { return null; }

        return CreaturesInLevel[index];
    }

    public void LogCreature(FreeMoveController creature)
    {
        if (CreaturesInLevel == null) CreaturesInLevel = new List<FreeMoveController>();
        CreaturesInLevel.Add(creature);
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
