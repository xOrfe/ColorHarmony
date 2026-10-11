using System;
using System.Collections.Generic;
using UnityEngine;

namespace XO.ColorHarmony
{
    [Serializable]
    public class GradientXDataScriptable : ScriptableObject
    {
        public List<GradientXData> Gradients;

        public GradientXDataScriptable()
        {
            Gradients = new List<GradientXData>();
        }
    }

}

