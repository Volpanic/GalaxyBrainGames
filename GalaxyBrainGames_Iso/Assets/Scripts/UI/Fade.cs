using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    //Fade info
    [SerializeField] private CanvasGroup FadeInGroup;
    [SerializeField] private bool fadeIn = true;

    public bool FadeDone
    {
        get
        {
            if (fadeIn) return FadeInGroup.alpha == 0;
            else return FadeInGroup.alpha == 1;
        }
    }

    private void Start()
    {
        FadeInGroup.alpha = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeIn)
        {
            FadeInGroup.alpha = Mathf.MoveTowards(FadeInGroup.alpha, 0, Time.deltaTime);
        }
        else
        {
            FadeInGroup.alpha = Mathf.MoveTowards(FadeInGroup.alpha, 1, Time.deltaTime);
        }

    }

    public void FadeIn()
    {
        FadeInGroup.alpha = 1;
        fadeIn = true;
    }

    public void FadeOut()
    {
        FadeInGroup.alpha = 0;
        fadeIn = false;
    }
}
