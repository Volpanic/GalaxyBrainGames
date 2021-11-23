using GalaxyBrain.Attributes;
using GalaxyBrain.Audio;
using GalaxyBrain.Creatures;
using GalaxyBrain.Managers;
using GalaxyBrain.Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GalaxyBrain.Systems
{
    [CreateAssetMenu]
    public class CreatureData : ScriptableObject
    {
        [ReadOnly]
        public List<PlayerController> CreaturesInLevel = new List<PlayerController>();

        [ReadOnly]
        public CreatureManager CreatureManager;

        [ReadOnly]
        public GridPathfinding pathfinding;

        [Header("Creature Icons")]
        public Sprite ChildIcon;
        public Sprite StrongIcon;
        public Sprite WaterIcon;

        [Header("Creature Sounds")]
        public AudioData ChildSwapSound;
        public AudioData StrongSwapSound;
        public AudioData WaterSwapSound;

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

        public PlayerController GetSelectedCreature()
        {
            if (CreaturesInLevel == null && CreaturesInLevel.Count <= 0) { return null; }

            return CreatureManager?.SelectedCreature;
        }

        public void SetSelectedCreature(int creatureIndex)
        {
            if (CreaturesInLevel == null && CreaturesInLevel.Count <= 0) { return; }

            CreatureManager.SelectCreature(creatureIndex);
        }

        public void PlayCreatureSwapSound(PlayerController.PlayerTypes type)
        {
            switch (type)
            {
                case PlayerController.PlayerTypes.Child:  ChildSwapSound?.Play();  break;
                case PlayerController.PlayerTypes.Water:  WaterSwapSound?.Play();  break;
                case PlayerController.PlayerTypes.Strong: StrongSwapSound?.Play(); break;
            }
        }

        public void LogCreature(PlayerController creature)
        {
            if (CreaturesInLevel == null) CreaturesInLevel = new List<PlayerController>();
            CreaturesInLevel.Add(creature);

            //Sort Creatures
            CreaturesInLevel = CreaturesInLevel.OrderBy((x) => x.PlayerType).ToList();

            CleanCreatureData();
        }

        private void CleanCreatureData()
        {
            for (int i = 0; i < CreaturesInLevel.Count; i++)
            {
                if (CreaturesInLevel[i] == null)
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
    }
}