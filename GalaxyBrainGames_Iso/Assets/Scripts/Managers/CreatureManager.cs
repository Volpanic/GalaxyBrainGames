using GalaxyBrain.Creatures;
using GalaxyBrain.Pathfinding;
using GalaxyBrain.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyBrain.Managers
{
    public class CreatureManager : MonoBehaviour
    {
        [SerializeField] private CreatureData creatureData;
        [SerializeField] private GridPathfinding pathfinding;

        public event Action<int,int> OnSelectedChanged;

        private int selectedCreature;

        public PlayerController SelectedCreature
        {
            get
            {
                if (creatureData != null)
                {
                    return creatureData.GetCreature(selectedCreature);
                }
                return null;
            }
        }

        public void SelectCreature(int newSelection)
        {
            if (creatureData.CreaturesInLevel != null && creatureData.CreaturesInLevel.Count != 0)
            {
                int oldCreature = selectedCreature;
                creatureData.CreaturesInLevel[selectedCreature].Selected = false;
                selectedCreature = WrapNumber(newSelection, 0, creatureData.CreaturesInLevel.Count);
                creatureData.CreaturesInLevel[selectedCreature].Selected = true;
                if (oldCreature != selectedCreature) OnSelectedChanged?.Invoke(oldCreature, selectedCreature);
            }
        }

        private void Awake()
        {
            creatureData.LogManager(this);
            creatureData.pathfinding = pathfinding;
        }

        // Start is called before the first frame update
        private void Start()
        {
            if (creatureData.CreaturesInLevel != null && creatureData.CreaturesInLevel.Count != 0)
            {
                for (int i = 1; i < creatureData.CreaturesInLevel.Count; i++)
                {
                    creatureData.CreaturesInLevel[i].Selected = false;
                }
                creatureData.CreaturesInLevel[0].Selected = true;
                OnSelectedChanged?.Invoke(-1,selectedCreature);
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q)) SelectCreature(selectedCreature - 1);
            if (Input.GetKeyDown(KeyCode.E)) SelectCreature(selectedCreature + 1);

            for (int i = 0; i < creatureData.CreaturesInLevel.Count; i++)
            {
                if(Input.GetKeyDown(KeyCode.Alpha0 + (i+1)))
                {
                    SelectCreature(i);
                }
            }
        }

        private int WrapNumber(int current, int min, int max)
        {
            int _mod = (current - min) % (max - min);
            if (_mod < 0) return _mod + max;
            else return _mod + min;
        }
    }
}