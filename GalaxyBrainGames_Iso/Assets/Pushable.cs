using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pushable : MonoBehaviour
{
    [SerializeField] private Interactalbe interactalbe;
    [SerializeField] private CreatureData creatureData;
    [SerializeField] private Collider myCollider;

    private bool pushedOver = false;
    private bool fallingOver = false;
    private float fallingTimer = 0;

    private Quaternion initalRotation;
    private Quaternion targetRotation;

    private void Awake()
    {
        initalRotation = transform.rotation;
    }

    private void Update()
    {
        if(fallingOver)
        {
            fallingTimer += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(initalRotation,targetRotation,fallingTimer);

            if(fallingTimer >= 1)
            {
                transform.position += Vector3.down * 0.5f;
                fallingOver = false;
                creatureData.pathfinding.UpdateNodeCells(myCollider.bounds.min,myCollider.bounds.max);
            }
        }
    }

    public void Push()
    {
        if (interactalbe == null || interactalbe.lastInteractedWithCreature == null) return;

        Vector3 pushDirection = MakeCardinal(-(interactalbe.lastInteractedWithCreature.transform.position - transform.position));
        Debug.DrawLine(transform.position, transform.position + (pushDirection * 4f),Color.red,1);
        if (pushDirection == Vector3.zero) return;

        Vector3 target = initalRotation.eulerAngles;
        target += pushDirection * 90;

        targetRotation.SetFromToRotation(Vector3.up,pushDirection);
        fallingOver = true;
        pushedOver  = true;
    }

    private Vector3 MakeCardinal(Vector3 direction)
    {
        float absX = Mathf.Abs(direction.x);
        float absY = Mathf.Abs(direction.y);
        float absZ = Mathf.Abs(direction.z);

        if (absX > absY && absX > absZ) return new Vector3(Mathf.Sign(direction.x), 0, 0);
        if (absY > absX && absY > absZ) return new Vector3(0, Mathf.Sign(direction.y), 0);
        if (absZ > absX && absZ > absY) return new Vector3(0, 0, Mathf.Sign(direction.z));

        return Vector3.zero;
    }
}
