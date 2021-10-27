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
        private HashSet<GameObject> blockers = new HashSet<GameObject>();

        private FlowingWater forwardWater;

        public bool Blocked
        {
            get { return blockers.Count > 0; }
        }

        private void Start()
        {
            Collider[] forwardObjects = Physics.OverlapBox(transform.position + transform.forward,Vector3.one,Quaternion.identity,~0,QueryTriggerInteraction.Collide);

            for(int i = 0; i < forwardObjects.Length; i++)
            {
                FlowingWater forward = forwardObjects[i].GetComponent<FlowingWater>();

                if(forward != null && forward != this)
                {
                    forwardWater = forward;
                    break;
                }
            }
        }

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
                    return;
                }
            }

            //No creature, so must be a blocker?
            if(!blockers.Contains(other.gameObject))
            {
                blockers.Add(other.gameObject);
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
                    return;
                }
            }

            //No creature, so must be a blocker?
            if (blockers.Contains(other.gameObject))
            {
                blockers.Remove(other.gameObject);
            }
        }

        private void Update()
        {
            if (Blocked) return;
            if(forwardWater != null && forwardWater.Blocked)
            {
                return;
            }

            foreach (PlayerController swimmer in swimmers)
            {
                swimmer.ShiftPlayer(transform.forward);
            }
        }
    }
}
