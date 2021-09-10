using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorControl : MonoBehaviour
{
    public GameObject element;
    public bool isOpen = false;
    public void Both()
    {
        if (isOpen)
        {
            element.SetActive(true);
            isOpen = false;
        }
        else
        {
            element.SetActive(false);
            isOpen = true;
        }
    }
}
