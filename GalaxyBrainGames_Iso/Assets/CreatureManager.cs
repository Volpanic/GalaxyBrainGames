using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureManager : MonoBehaviour
{
    [SerializeField] private List<PlayerController> CreaturesInLevel;
    private int selectedCreature;

    // Start is called before the first frame update
    void Start()
    {
        if (CreaturesInLevel != null && CreaturesInLevel.Count != 0)
        {
            for (int i = 1; i < CreaturesInLevel.Count; i++)
            {
                CreaturesInLevel[i].Selected = false;
            }
            CreaturesInLevel[0].Selected = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) SelectCreature(selectedCreature - 1);
        if (Input.GetKeyDown(KeyCode.E)) SelectCreature(selectedCreature + 1);
    }

    public void SelectCreature(int newSelection)
    {
        if (CreaturesInLevel != null && CreaturesInLevel.Count != 0)
        {
            CreaturesInLevel[selectedCreature].Selected = false;
            selectedCreature = WrapNumber(newSelection, 0, CreaturesInLevel.Count);
            CreaturesInLevel[selectedCreature].Selected = true;
        }
    }

    private int WrapNumber(int current, int min, int max)
    {
        int _mod = (current - min) % (max - min);
        if (_mod < 0) return _mod + max; 
        else return _mod + min;
    }

    private void OnDrawGizmos()
    {
        if (CreaturesInLevel != null && CreaturesInLevel.Count != 0)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(CreaturesInLevel[selectedCreature].transform.position, 0.2f);
        }
    }
}
