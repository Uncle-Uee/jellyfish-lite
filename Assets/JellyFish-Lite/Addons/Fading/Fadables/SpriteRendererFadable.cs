// Created by Kearan Petersen : https://www.blumalice.wordpress.com | https://www.linkedin.com/in/kearan-petersen/

using UnityEngine;

namespace SOFlow.Fading
{
    public class SpriteRendererFadable : Fadable
    {
        /// <summary>
        ///     The sprite renderer reference.
        /// </summary>
        public SpriteRenderer SpriteRenderer;

        /// <inheritdoc />
        protected override Color GetColour()
        {
            return SpriteRenderer.color;
        }

        /// <inheritdoc />
        public override void UpdateColour(Color colour, float percentage)
        {
            SpriteRenderer.color = colour;
        }
    }
}