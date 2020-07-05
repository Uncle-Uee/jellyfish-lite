// Created by Kearan Petersen : https://www.blumalice.wordpress.com | https://www.linkedin.com/in/kearan-petersen/

using UnityEngine;

namespace SOFlow.Fading
{
    public class FadableUIElement : Fadable
    {
        /// <summary>
        ///     The UI element to fade.
        /// </summary>
        public CanvasRenderer UIElement;

        /// <inheritdoc />
        protected override Color GetColour()
        {
            return UIElement.GetColor();
        }

        /// <inheritdoc />
        public override void UpdateColour(Color colour, float percentage)
        {
            UIElement.SetColor(colour);
        }
    }
}