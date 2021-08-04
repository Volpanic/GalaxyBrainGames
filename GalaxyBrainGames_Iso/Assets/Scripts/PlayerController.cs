using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[SelectionBase]
public class PlayerController : MonoBehaviour
{
    [SerializeField,Min(0.1f)] private float movementSpeed = 1;
    [SerializeField] private Controller3D controller;

    private void FixedUpdate()
    {
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical"));
        direction = transform.TransformDirection(direction);

        controller.SimpleMove(direction * movementSpeed);
    }
}
