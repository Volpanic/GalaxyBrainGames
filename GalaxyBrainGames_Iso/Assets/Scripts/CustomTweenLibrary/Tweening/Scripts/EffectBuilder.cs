using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Volpanic.UITweening
{
    public class EffectBuilder
    {
        private MonoBehaviour owner { get; }

        private List<IUIEffect> effects = new List<IUIEffect>();

        //Constructor takes in a mono behavior so we can run coroutines
        public EffectBuilder(MonoBehaviour owner)
        {
            this.owner = owner;
        }

        public EffectBuilder AddEffect(IUIEffect effect)
        {
            effects.Add(effect);
            return this;
        }

        public void ExecuteEvents()
        {
            owner.StopAllCoroutines();
            foreach(IUIEffect effect in effects)
            {
                owner.StartCoroutine(effect.Execute());
            }
        }
    }
}