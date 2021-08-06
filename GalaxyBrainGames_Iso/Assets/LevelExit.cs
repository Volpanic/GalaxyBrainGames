using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [SerializeField] Fade fadeManger;
    bool hasFaded = false;

    private void Update()
    {
        if (hasFaded && fadeManger.FadeDone)
        {
            SceneManager.LoadScene(0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasFaded)
        {
            fadeManger.FadeOut();
            hasFaded = true;
        }
    }
}
