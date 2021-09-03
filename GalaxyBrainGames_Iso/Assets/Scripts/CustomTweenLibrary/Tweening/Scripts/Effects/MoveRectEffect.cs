using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Volpanic.Easing;

namespace Volpanic.UITweening
{
    public class MoveRectEffect : IUIEffect
    {
        private RectTransform rectTransform;
        private Vector3 targetPosition;
        private TweenData tweenData;
        private YieldInstruction wait;

        public MoveRectEffect(RectTransform rectTransform, Vector3 targetPosition,bool localPosition, YieldInstruction wait,
            TweenData tweenData)
        {
            this.rectTransform = rectTransform;
            this.targetPosition = (localPosition)? targetPosition + rectTransform.position : targetPosition;
            this.wait = wait;
            this.tweenData = tweenData;
        }

        public IEnumerator Execute()
        {
            float timer = 0f;
            Vector3 currentPosition = rectTransform.position;
            Vector3 startPos = currentPosition;

            while (rectTransform.position != targetPosition)
            {
                timer = Mathf.Clamp(timer + Time.deltaTime,0, tweenData.Duration);

                Vector3 position = new Vector3(0,0,0);
                position.x = tweenData.EasingFunction(currentPosition.x, targetPosition.x,timer / tweenData.Duration);
                position.y = tweenData.EasingFunction(currentPosition.y, targetPosition.y,timer / tweenData.Duration);
                position.z = tweenData.EasingFunction(currentPosition.z, targetPosition.z,timer / tweenData.Duration);

                rectTransform.position = position;
                yield return null;
            }

            yield return wait;

            if (tweenData.ReturnAfter)
            {
                timer = 0f;
                currentPosition = rectTransform.position;

                while (rectTransform.position != Vector3.one)
                {
                    timer = Mathf.Clamp(timer + Time.deltaTime, 0, tweenData.Duration);
                    
                    Vector3 position = new Vector3(0, 0, 0);
                    position.x = tweenData.EasingFunction(currentPosition.x, startPos.x, timer / tweenData.Duration);
                    position.y = tweenData.EasingFunction(currentPosition.y, startPos.y, timer / tweenData.Duration);
                    position.z = tweenData.EasingFunction(currentPosition.z, startPos.z, timer / tweenData.Duration);

                    rectTransform.position = position;
                    yield return null;
                }
            }

            tweenData.CompleteEvent?.Invoke();
        }
    }
}