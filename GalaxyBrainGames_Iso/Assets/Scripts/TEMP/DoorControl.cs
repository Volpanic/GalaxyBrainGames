using GalaxyBrain.Audio;
using GalaxyBrain.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorControl : MonoBehaviour
{
    public AudioData doorSound;
    public GameObject element;
    public bool isOpen = false;
    public CreatureData data;
    public Collider myCollider;
    public Animator animator;

    public void Both()
    {
        if (isOpen)
        {
            element.SetActive(true);
            animator.Play("AN_door_close");
            if (animator != null)
                animator.SetBool("doorOpen", false);
            isOpen = false;
            data.pathfinding?.UpdateNodeCells(myCollider.bounds.min,myCollider.bounds.max);
        }
        else
        {
            element.SetActive(false);
            animator.Play("AN_door_open");
            if (animator != null)
                animator.SetBool("doorOpen", true);
            isOpen = true;
            data.pathfinding?.UpdateNodeCells(myCollider.bounds.min, myCollider.bounds.max);
        }
        doorSound?.Play();
    }
}