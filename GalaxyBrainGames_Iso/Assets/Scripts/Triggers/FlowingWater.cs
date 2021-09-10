using GalaxyBrain.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyBrain.Creatures
{
    public class FlowingWater : MonoBehaviour
    {
        [SerializeField] private CreatureData creatureData;
        [SerializeField] private PlayerController.PlayerTypes targetType;

        private HashSet<PlayerController> swimmers = new HashSet<PlayerController>();

        private void OnTriggerEnter(Collider other)
        {
            if (creatureData == null) return;

            List<PlayerController> targetCreatures = creatureData.CreaturesInLevel.FindAll(x => x.PlayerType == targetType);

            for(int i = 0; i < targetCreatures.Count; i++)
            {
                //Check if we found the creature in out trigger
                if(targetCreatures[i].gameObject == other.gameObject)
                {
                    if(!swimmers.Contains(targetCreatures[i]))
                    {
                        swimmers.Add(targetCreatures[i]);
                    }
                    break;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (creatureData == null) return;

            List<PlayerController> targetCreatures = creatureData.CreaturesInLevel.FindAll(x => x.PlayerType == targetType);

            for (int i = 0; i < targetCreatures.Count; i++)
            {
                //Check if we found the creature in out trigger
                if (targetCreatures[i].gameObject == other.gameObject)
                {
                    if (swimmers.Contains(targetCreatures[i]))
                    {
                        swimmers.Remove(targetCreatures[i]);
                    }
                    break;
                }
            }
        }

        private void Update()
        {
            foreach(PlayerController swimmer in swimmers)
            {
                swimmer.ShiftPlayer(transform.forward);
            }
        }
    }
}
