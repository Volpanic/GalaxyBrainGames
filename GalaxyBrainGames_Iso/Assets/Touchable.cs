using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Touchable : MonoBehaviour
{
    public UnityEvent OnTouch;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            OnTouch.Invoke();
        }
    }
}
