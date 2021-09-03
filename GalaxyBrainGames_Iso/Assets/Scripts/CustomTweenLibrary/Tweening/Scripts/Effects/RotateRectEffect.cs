using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Volpanic.UITweening
{
    public class RotateRectEffect : IUIEffect
    {
        private RectTransform rectTransform { get; }
        private TweenData tweenData;
        private float addAngle { get; }


        public RotateRectEffect(RectTransform rectTransform, float addAngle, TweenData tweenData)
        {
            this.rectTransform = rectTransform;
            this.addAngle = addAngle;
            this.tweenData = tweenData;
        }

        public IEnumerator Execute()
        {
            float timer = 0f;
            Quaternion startingRotation = rectTransform.localRotation;
            float currentRotation = startingRotation.eulerAngles.z;
            float targetRotation  = startingRotation.eulerAngles.z + addAngle;

            Vector3 rotateTo = new Vector3(0,0,0);

            while (timer < tweenData.Duration)
            {
                timer = Mathf.Clamp(timer + Time.deltaTime, 0, tweenData.Duration);
                //Vector3 scale = Vector3.Lerp(currentScale, maxSize, timer);

                rotateTo.z = tweenData.EasingFunction(currentRotation, targetRotation, timer / tweenData.Duration);

                rectTransform.rotation = Quaternion.Euler(rotateTo);
                yield return null;
            }

            if (tweenData.ReturnAfter)
            {
                timer = 0f;
                currentRotation = targetRotation;
                targetRotation = startingRotation.eulerAngles.z;

                while (timer < tweenData.Duration)
                {
                    timer = Mathf.Clamp(timer + Time.deltaTime, 0, tweenData.Duration);
                    //Vector3 scale = Vector3.Lerp(currentScale, maxSize, timer);

                    rotateTo.z = tweenData.EasingFunction(currentRotation, targetRotation, timer / tweenData.Duration);

                    rectTransform.rotation = Quaternion.Euler(rotateTo);
                    yield return null;
                }
            }
            tweenData.CompleteEvent?.Invoke();
        }
    }
}
