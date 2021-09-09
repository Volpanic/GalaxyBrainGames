using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyBrain.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SceneAttribute : PropertyAttribute
    {
        // Custom Attributes need 2 paths, the Property Attribute
        // and the Property Drawer, This file is empty because 
        // the attribute doesn't need to hold any data, but
        // still needs this to exist.
    }
}
