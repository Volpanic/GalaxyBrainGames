using GalaxyBrain.Systems;
using GalaxyBrain.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GalaxyBrain.Managers
{
    public class ChangeScene : MonoBehaviour
    {
        [SerializeField] private Fade fade;
        [SerializeField] private LevelProgression levelProg;

        private string targetIndex = "";

        public void Changescene()
        {
            if (fade.FadeDone)
            {
                fade.FadeOut(ChangeSceneAfterFade);
            }
        }

        public void ResetScene()
        {
            if (fade.FadeDone)
            {
                fade.FadeOut(ResetSceneAfterFade);
            }
        }

        private void ChangeSceneAfterFade()
        {
            targetIndex = levelProg.GetNextScene();

            SceneManager.LoadScene(targetIndex);
        }

        private void ResetSceneAfterFade()
        {
            targetIndex = SceneManager.GetActiveScene().name;

            SceneManager.LoadScene(targetIndex);
        }
    }
}