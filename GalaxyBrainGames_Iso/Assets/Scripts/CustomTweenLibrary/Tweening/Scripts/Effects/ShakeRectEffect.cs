using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Volpanic.Easing;

namespace Volpanic.UITweening
{
    public class ShakeRectEffect : IUIEffect
    {
        private RectTransform rectTransform { get; }
        private float maxRotation { get; }

        private TweenData tweenData;

        public ShakeRectEffect(RectTransform rectTransform, float maxRotation,TweenData tweenData)
        {
            this.rectTransform = rectTransform;
            this.maxRotation = maxRotation;
            this.tweenData = tweenData;
        }

        public IEnumerator Execute()
        {
            Quaternion rotateTo = Quaternion.Euler(0, 0, maxRotation);

            float currentRotation = rectTransform.rotation.z;
            float nextRotation = -maxRotation;
            float timer = 0;

            while(Mathf.Abs(nextRotation) > 0.15f)
            {
                timer = Mathf.Clamp(timer + Time.deltaTime,0,tweenData.Duration);
                float newRotation = tweenData.EasingFunction(currentRotation, nextRotation, timer/tweenData.Duration);
                rotateTo.eulerAngles = new Vector3(0, 0, newRotation);
                rectTransform.rotation = rotateTo;

                if(timer >= tweenData.Duration)
                {
                    currentRotation = newRotation;
                    nextRotation = -(nextRotation * 0.8f);
                    timer = 0;
                }

                yield return null;
            }

            rotateTo.eulerAngles = Vector3.zero;
            rectTransform.rotation = rotateTo;
        }
    }
}