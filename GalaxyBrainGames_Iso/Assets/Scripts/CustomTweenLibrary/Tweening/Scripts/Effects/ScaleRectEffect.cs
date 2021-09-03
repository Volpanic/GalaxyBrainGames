using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Volpanic.Easing;

namespace Volpanic.UITweening
{
    public class ScaleRectEffect : IUIEffect
    {
        private RectTransform rectTransform;
        private Vector3 maxSize;
        private TweenData tweenData;
        private YieldInstruction wait;

        public ScaleRectEffect(RectTransform rectTransform, Vector3 maxSize, YieldInstruction wait,
            TweenData tweenData)
        {
            this.rectTransform = rectTransform;
            this.maxSize = maxSize;
            this.wait = wait;
            this.tweenData = tweenData;
        }

        public IEnumerator Execute()
        {
            float timer = 0f;
            Vector3 currentScale = rectTransform.localScale;

            while (rectTransform.localScale != maxSize)
            {
                timer = Mathf.Clamp(timer + Time.deltaTime,0, tweenData.Duration);
                //Vector3 scale = Vector3.Lerp(currentScale, maxSize, timer);

                Vector3 scale = new Vector3(0,0,0);
                scale.x = tweenData.EasingFunction(currentScale.x,maxSize.x,timer / tweenData.Duration);
                scale.y = tweenData.EasingFunction(currentScale.y,maxSize.y,timer / tweenData.Duration);
                scale.z = tweenData.EasingFunction(currentScale.z,maxSize.z,timer / tweenData.Duration);

                rectTransform.localScale = scale;
                yield return null;
            }

            yield return wait;

            if (tweenData.ReturnAfter)
            {
                timer = 0f;
                currentScale = rectTransform.localScale;

                while (rectTransform.localScale != Vector3.one)
                {
                    timer = Mathf.Clamp(timer + Time.deltaTime, 0, tweenData.Duration);
                    //Vector3 scale = Vector3.Lerp(currentScale, Vector3.one, timer);
                    Vector3 scale = new Vector3(0, 0, 0);
                    scale.x = tweenData.EasingFunction(currentScale.x, 1, timer / tweenData.Duration);
                    scale.y = tweenData.EasingFunction(currentScale.y, 1, timer / tweenData.Duration);
                    scale.z = tweenData.EasingFunction(currentScale.z, 1, timer / tweenData.Duration);
                    rectTransform.localScale = scale;

                    yield return null;
                }
            }

            tweenData.CompleteEvent?.Invoke();
        }
    }
}