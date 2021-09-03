using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Volpanic.UITweening
{
    public interface IUIEffect
    {
        IEnumerator Execute();
    }
}