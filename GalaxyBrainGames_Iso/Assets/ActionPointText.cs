using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Volpanic.Easing;
using Volpanic.UITweening;

namespace GalaxyBrain.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class ActionPointText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI actionPointText;

        [Header("Scale Effect")]
        [SerializeField] private Vector3 lerpScale = Vector3.one;
        [SerializeField] private float lerpDuration = 1f;

        private RectTransform rectTransform;
        private EffectBuilder effectBuilder;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            effectBuilder = new EffectBuilder(this);

            effectBuilder.AddEffect(new ScaleRectEffect(rectTransform,lerpScale,new WaitForSeconds(0),new TweenData(lerpDuration,true,Easingf.InOutBack)));
        }

        private void OnEnable()
        {
            if (actionPointText != null) TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
        }

        private void OnDisable()
        {
            if (actionPointText != null) TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
        }

        private void OnTextChanged(Object obj)
        {
            if(obj == actionPointText)
            {
                effectBuilder.ExecuteEvents();
            }
        }
    }
}
