using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CreatureData : ScriptableObject
{
    public List<PlayerController> CreaturesInLevel = new List<PlayerController>();
    public CreatureManager CreatureManager;

    private void OnEnable()
    {
        Debug.Log("DataEnable");
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
    }

    public void LogManager(CreatureManager manger)
    {
        CreatureManager = manger;
    }
}
