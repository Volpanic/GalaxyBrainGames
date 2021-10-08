using GalaxyBrain.Utility;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Volpanic.Easing;

namespace GalaxyBrain.Effects
{
    [RequireComponent(typeof(RectTransform))]
    public class TextRiseEffect : MonoBehaviour
    {
        [SerializeField,Min(0.01f)] private float aliveTime;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Vector3 positionOffset;

        private float timer = 0;
        private Color textColor = Color.white;

        private Vector3 startPosition;
        private Vector3 targetPosition;

        private RectTransform rectTransform;

        private void OnEnable()
        {
            timer = 0;
        }

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();

            textColor = text.color;
            textColor.a = 1f;

            startPosition = rectTransform.localPosition;
            targetPosition = startPosition + positionOffset;
        }

        public void Update()
        {
            timer += Time.deltaTime;

            Vector3 targetPos;
            float normalizedTime = timer / aliveTime;

            targetPos.x = Easingf.OutSine(startPosition.x, targetPosition.x, normalizedTime);
            targetPos.y = Easingf.OutSine(startPosition.y, targetPosition.y, normalizedTime);
            targetPos.z = Easingf.OutSine(startPosition.z, targetPosition.z, normalizedTime);

            rectTransform.position = targetPos;
            textColor.a = 1f - (normalizedTime);

            text.color = textColor;

            if(timer >= aliveTime)
            {
                gameObject.SetActive(false);
            }
        }

        public void SetText(string newText, Color color, Vector3 newPos)
        {
            text.text = newText;

            color.a = textColor.a;
            textColor = color;

            text.color = textColor;

            if (rectTransform == null) return;
            rectTransform.position = newPos;
            startPosition = newPos;
            targetPosition = startPosition + positionOffset;
        }
    }
}