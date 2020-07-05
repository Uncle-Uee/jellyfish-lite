// Created by Kearan Petersen : https://www.blumalice.wordpress.com | https://www.linkedin.com/in/kearan-petersen/

using JellyFish.Data.Primitive;
using UnityEngine;

namespace SOFlow.Fading
{
    public class FloatDataFadable : Fadable
    {
        /// <summary>
        ///     The float data to fade.
        /// </summary>
        public FloatField FloatData;

        /// <summary>
        ///     The fade range.
        /// </summary>
        public Vector2Field FadeRange;

        /// <inheritdoc />
        protected override Color GetColour()
        {
            return default;
        }

        /// <inheritdoc />
        public override void UpdateColour(Color colour, float percentage)
        {
            FloatData.Value = Mathf.Lerp(FadeRange.Value.x, FadeRange.Value.y, percentage);
        }
    }
}