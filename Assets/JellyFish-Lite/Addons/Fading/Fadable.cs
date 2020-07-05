// Created by Kearan Petersen : https://www.blumalice.wordpress.com | https://www.linkedin.com/in/kearan-petersen/


using UnityEngine;
#if UNITY_EDITOR

#endif

namespace SOFlow.Fading
{
    public abstract class Fadable : MonoBehaviour
    {
        /// <summary>
        ///     Indicates whether only the alpha value of the provided colour should be used.
        /// </summary>
        public bool AlphaOnly;

        /// <summary>
        ///     Indicates whether the alpha should be inverted.
        /// </summary>
        public bool InvertAlpha;

        /// <summary>
        ///     Indicates whether the provided fade percentage should be inverted.
        /// </summary>
        public bool InvertPercentage;

        /// <summary>
        ///     Returns the colour component of this fadable.
        /// </summary>
        /// <returns></returns>
        protected abstract Color GetColour();

        /// <summary>
        ///     Callback received from the fader component.
        /// </summary>
        /// <param name="colour"></param>
        /// <param name="percentage"></param>
        public void OnUpdateColour(Color colour, float percentage)
        {
            if (AlphaOnly)
            {
                Color currentColour = GetColour();

                colour.r = currentColour.r;
                colour.g = currentColour.g;
                colour.b = currentColour.b;
            }

            if (InvertAlpha) colour.a = 1f - colour.a;

            UpdateColour(colour, InvertPercentage ? 1f - percentage : percentage);
        }

        /// <summary>
        ///     Updates the colour for this fadable object.
        /// </summary>
        /// <param name="colour"></param>
        /// <param name="percentage"></param>
        public abstract void UpdateColour(Color colour, float percentage);
    }
}