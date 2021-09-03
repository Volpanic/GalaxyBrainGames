using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Volpanic.Easing;

namespace Volpanic.UITweening
{
    [System.Serializable]
    public struct TweenData 
    {
        public float Duration;
        public bool ReturnAfter;
        public Func<float, float, float, float> EasingFunction;
        public Action CompleteEvent;

        public TweenData(float duration, bool returnAfter, Func<float, float, float, float> easingFunction = null, Action completeEvent = null)
        {
            Duration = duration;
            ReturnAfter = returnAfter;
            EasingFunction = (easingFunction == null)? Easingf.Linear : easingFunction;
            CompleteEvent = completeEvent;
        }
    }
}