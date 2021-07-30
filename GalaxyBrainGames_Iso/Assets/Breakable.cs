using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Breakable : MonoBehaviour
{
    public bool AutoDestroy = true;
    public UnityEvent OnBreak;

    public void Break()
    {
        OnBreak.Invoke();
        if(AutoDestroy) Destroy(gameObject);
    }
}
