using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Volpanic.Easing
{
    // A stuct with static functions in chosen to emulate Unity's Mathf class
    public struct Easingf
    {
        // Regular Lerp
        public static float Linear(float start, float end, float time)
        {
            return start * (1 - time) + end * time;
        }

        // A interpolation function that has smoothing in and out
        public static float InOutSine(float start, float end, float time)
        {
            float mod = 1f -(Mathf.Cos(time * Mathf.PI) / 2f);
            return start + ((end-start) * mod);
        }

        // A interpolation function that has smoothing in and out
        public static float InSine(float start, float end, float time)
        {
            float mod = -(Mathf.Cos(Mathf.PI * time) - 1f) / 2f;
            return start + ((end - start) * mod);
        }

        public static float InExpo(float start, float end, float time)
        {
            float mod = time == 0 ? 0 : Mathf.Pow(2, 10f * time - 10f);
            mod = Mathf.Clamp01(mod);
            return start + ((end - start) * mod);
        }

        //Bounds for lerps that go out of the 0-1 range
        private const float c1 = 1.70158f;
        private const float c2 = c1 * 1.525f;

        //under shoots and over shoots it's targets before going to correct values
        public static float InOutBack(float start, float end, float time)
        {
            float mod = (time < 0.5f) ?
                (Mathf.Pow(2f * time, 2f) * ((c2 + 1f) * 2f * time - c2)) / 2f :
                (Mathf.Pow(2f * time - 2f, 2f) * ((c2 + 1f) * (time * 2f - 2f)+c2)+2f) / 2f;
            return start + ((end - start) * mod);
        }
    }
}