using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Volpanic.Easing;
using Volpanic.UITweening;

namespace GalaxyBrain.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class CreatureSelectionIcon : MonoBehaviour
    {
        [Header("Mouse Over")]
        [SerializeField] private Vector3 mouseOverScale = Vector3.one;
        [SerializeField] private float mouseOverLerpTime = 0.5f;

        [Header("Selected")]
        [SerializeField] private Vector3 selectedScale = Vector3.one;
        [SerializeField] private Vector3 selectedOffset = new Vector3(0,16,0);
        [SerializeField] private float selectedLerpTime = 0.5f;

        private RectTransform rectTransform;

        private EffectBuilder mouseOverEnterEffects;
        private EffectBuilder mouseOverExitEffects;
        private EffectBuilder onSwapOnEffects;
        private EffectBuilder onSwapOffEffects;

        private Vector3 originalSelectedPosition;
        private Vector3 endSelectedPositon;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();

            mouseOverEnterEffects = new EffectBuilder(this);
            mouseOverExitEffects = new EffectBuilder(this);
            onSwapOnEffects = new EffectBuilder(this);
            onSwapOffEffects = new EffectBuilder(this);
        }

        //Called in CreatureSelectionGridUI
        public void SetupEffects()
        {
            originalSelectedPosition = rectTransform.position;
            endSelectedPositon = rectTransform.position + selectedOffset;

            //Mouse Over Enter
            mouseOverEnterEffects.AddEffect(new ScaleRectEffect(rectTransform, mouseOverScale, new WaitForSeconds(0),
                new TweenData(mouseOverLerpTime, false, Easingf.InOutSine)));

            //Mouse Over Exit
            mouseOverExitEffects.AddEffect(new ScaleRectEffect(rectTransform, Vector3.one, new WaitForSeconds(0),
                new TweenData(mouseOverLerpTime, false, Easingf.InOutSine)));

            //When Selected
            onSwapOnEffects.AddEffect(new MoveRectEffect(rectTransform, endSelectedPositon, false, new WaitForSeconds(0),
                new TweenData(selectedLerpTime, false, Easingf.InOutSine)));
            onSwapOnEffects.AddEffect(new ScaleRectEffect(rectTransform, selectedScale, new WaitForSeconds(0),
                new TweenData(selectedLerpTime, false, Easingf.InOutBack)));

            //When Deselected
            onSwapOffEffects.AddEffect(new MoveRectEffect(rectTransform, originalSelectedPosition, false, new WaitForSeconds(0),
                new TweenData(selectedLerpTime, false, Easingf.InOutSine)));
            onSwapOffEffects.AddEffect(new ScaleRectEffect(rectTransform, Vector3.one, new WaitForSeconds(0),
                new TweenData(selectedLerpTime, false, Easingf.InOutBack)));
        }

        public void MouseHoverEnter()
        {
            mouseOverEnterEffects.ExecuteEvents();
        }

        public void MouseHoverExit()
        {
            mouseOverExitEffects.ExecuteEvents();
        }

        public void OnSelectCreature()
        {
            onSwapOnEffects.ExecuteEvents();
        }

        public void OnDeselectCreature()
        {
            onSwapOffEffects.ExecuteEvents();
        }
    }
}
