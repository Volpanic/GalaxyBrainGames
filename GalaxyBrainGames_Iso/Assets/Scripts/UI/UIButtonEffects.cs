using GalaxyBrain.Utility.Extnesion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GalaxyBrain
{
    [RequireComponent(typeof(EventTrigger))]
    public class UIButtonEffects : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI buttonText;

        [SerializeField, Range(0f, 1f)] private float sinWaveHeightPercent = 0.5f;

        [SerializeField] private bool rainbow = false;
        [SerializeField] private bool shake = false;

        private EventTrigger buttonEvents;
        private RectTransform rectTransform;
        private bool textChanged = false;

        private TMP_MeshInfo[] originalMeshInfo;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            buttonEvents = GetComponent<EventTrigger>();
        }

        private void OnEnable()
        {
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
            AddToTriggerEvents();
        }
        private void OnDisable()
        {
            TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
        }

        private void OnTextChanged(UnityEngine.Object obj)
        {
            if(obj == buttonText)
            {
                textChanged = true;

                originalMeshInfo = buttonText.textInfo.CopyMeshInfoVertexData();
            }
        }

        private void AddToTriggerEvents()
        {
            EventTrigger.Entry hover = new EventTrigger.Entry();
            hover.eventID = EventTriggerType.PointerEnter;
            hover.callback.AddListener((eventData) => { OnHover(); });
            buttonEvents.triggers.Add(hover);
        }

        public void OnHover()
        {
            //StartCoroutine(ShakeText(.25f));
            if (!shake) StartCoroutine(WaveText(.25f));
            else StartCoroutine(ShakeText(.25f));

            if (rainbow) StartCoroutine(RainbowFlash(.25f));
        }

        public void OnClick()
        {
            StartCoroutine(WaveText(.25f));
        }

        private IEnumerator RainbowFlash(float duration)
        {
            float timer = 0;
            float shakeAmount = (rectTransform.rect.yMax - rectTransform.rect.yMin) * sinWaveHeightPercent;

            while (timer <= duration)
            {
                timer += Time.unscaledDeltaTime;

                int characterCount = buttonText.textInfo.characterCount;

                for (int i = 0; i < characterCount; i++)
                {
                    TMP_CharacterInfo charInfo = buttonText.textInfo.characterInfo[i];

                    if (!charInfo.isVisible)
                        continue;

                    int materialIndex = charInfo.materialReferenceIndex;
                    int vertexIndex = charInfo.vertexIndex;

                    Color32[] sourceColors = originalMeshInfo[materialIndex].colors32;
                    Color32[] destinationColors = originalMeshInfo[materialIndex].colors32;

                    float normalizedPosition = i / (float)characterCount;
                    float randomHue = NumberExtensions.SinWave(0f, 1f, duration, normalizedPosition, timer * 0.25f);
                    Color32 targetColor = Color.HSVToRGB(randomHue,1f,1f);
                    targetColor = Color32.Lerp(targetColor, sourceColors[vertexIndex + 0], timer / duration);

                    destinationColors[vertexIndex + 0] = targetColor;
                    destinationColors[vertexIndex + 1] = targetColor;
                    destinationColors[vertexIndex + 2] = targetColor;
                    destinationColors[vertexIndex + 3] = targetColor;

                    buttonText.textInfo.meshInfo[materialIndex].colors32 = destinationColors;
                }

                // Push changes into meshes
                for (int i = 0; i < buttonText.textInfo.meshInfo.Length; i++)
                {
                    buttonText.textInfo.meshInfo[i].mesh.colors32 = buttonText.textInfo.meshInfo[i].colors32;
                    buttonText.UpdateGeometry(buttonText.textInfo.meshInfo[i].mesh, i);
                }


                //Update at the end of frame, to apply the changes
                yield return new WaitForFixedUpdate();
                buttonText.ForceMeshUpdate();
            }
        }

        private IEnumerator ShakeText(float duration)
        {
            float timer = 0;
            float shakeAmount = (rectTransform.rect.yMax - rectTransform.rect.yMin) * sinWaveHeightPercent;

            while (timer <= duration)
            {
                timer += Time.unscaledDeltaTime;

                int characterCount = buttonText.textInfo.characterCount;

                for (int i = 0; i < characterCount; i++)
                {
                    TMP_CharacterInfo charInfo = buttonText.textInfo.characterInfo[i];

                    if (!charInfo.isVisible)
                        continue;

                    int materialIndex = charInfo.materialReferenceIndex;
                    int vertexIndex = charInfo.vertexIndex;

                    Vector3[] sourceVerticies = originalMeshInfo[materialIndex].vertices;
                    Vector3[] destinationVertices = originalMeshInfo[materialIndex].vertices;

                    Vector2 offset = new Vector2(Random.Range(-shakeAmount, shakeAmount), Random.Range(-shakeAmount, shakeAmount)) * (1f - (timer/duration));

                    destinationVertices[vertexIndex + 0] = sourceVerticies[vertexIndex + 0] + new Vector3(offset.x, offset.y, 0);
                    destinationVertices[vertexIndex + 1] = sourceVerticies[vertexIndex + 1] + new Vector3(offset.x, offset.y,0);
                    destinationVertices[vertexIndex + 2] = sourceVerticies[vertexIndex + 2] + new Vector3(offset.x, offset.y,0);
                    destinationVertices[vertexIndex + 3] = sourceVerticies[vertexIndex + 3] + new Vector3(offset.x, offset.y,0);

                    buttonText.textInfo.meshInfo[materialIndex].vertices = destinationVertices;
                }

                // Push changes into meshes
                for (int i = 0; i < buttonText.textInfo.meshInfo.Length; i++)
                {
                    buttonText.textInfo.meshInfo[i].mesh.vertices = buttonText.textInfo.meshInfo[i].vertices;
                    buttonText.UpdateGeometry(buttonText.textInfo.meshInfo[i].mesh, i);
                }


                //Update at the end of frame, to apply the changes
                yield return new WaitForFixedUpdate();
                buttonText.ForceMeshUpdate();
            }
        }

        private IEnumerator WaveText(float duration)
        {
            float timer = 0;
            float shakeAmount = (rectTransform.rect.yMax - rectTransform.rect.yMin) * sinWaveHeightPercent;

            while (timer <= duration)
            {
                timer += Time.unscaledDeltaTime;

                int characterCount = buttonText.textInfo.characterCount;

                for (int i = 0; i < characterCount; i++)
                {
                    TMP_CharacterInfo charInfo = buttonText.textInfo.characterInfo[i];

                    if (!charInfo.isVisible)
                        continue;

                    int materialIndex = charInfo.materialReferenceIndex;
                    int vertexIndex = charInfo.vertexIndex;

                    Vector3[] sourceVerticies = originalMeshInfo[materialIndex].vertices;
                    Vector3[] destinationVertices = originalMeshInfo[materialIndex].vertices;

                    float normalizedPosition = i / (float)characterCount;
                    Vector2 offset = new Vector2(0, NumberExtensions.SinWave(-shakeAmount*.5f, shakeAmount,duration, normalizedPosition, timer)) * (1f - (timer/duration));

                    destinationVertices[vertexIndex + 0] = sourceVerticies[vertexIndex + 0] + new Vector3(offset.x, offset.y, 0);
                    destinationVertices[vertexIndex + 1] = sourceVerticies[vertexIndex + 1] + new Vector3(offset.x, offset.y, 0);
                    destinationVertices[vertexIndex + 2] = sourceVerticies[vertexIndex + 2] + new Vector3(offset.x, offset.y, 0);
                    destinationVertices[vertexIndex + 3] = sourceVerticies[vertexIndex + 3] + new Vector3(offset.x, offset.y, 0);

                    buttonText.textInfo.meshInfo[materialIndex].vertices = destinationVertices;
                }

                // Push changes into meshes
                for (int i = 0; i < buttonText.textInfo.meshInfo.Length; i++)
                {
                    buttonText.textInfo.meshInfo[i].mesh.vertices = buttonText.textInfo.meshInfo[i].vertices;
                    buttonText.UpdateGeometry(buttonText.textInfo.meshInfo[i].mesh, i);
                }


                //Update at the end of frame, to apply the changes
                yield return new WaitForFixedUpdate();
                buttonText.ForceMeshUpdate();
            }
        }

    }
}
