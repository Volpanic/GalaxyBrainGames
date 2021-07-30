using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildCreature : MonoBehaviour
{
    [SerializeField] private PlayerController controller;
    [SerializeField] private LayerMask mask;

    private bool hasKey = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl))
        CheckForKeys();   
    }

    private void CheckForKeys()
    {
        if(hasKey)
        {

        }
    }
}
