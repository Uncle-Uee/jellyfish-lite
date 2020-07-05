// Created by Kearan Petersen : https://www.blumalice.wordpress.com | https://www.linkedin.com/in/kearan-petersen/

using TMPro;
using UnityEditor;
using UnityEngine;

namespace SOFlow.Fading
{
    public class TextMeshProFadable : Fadable
    {
        /// <summary>
        ///     The text reference.
        /// </summary>
        public TMP_Text Text;

        /// <inheritdoc />
        protected override Color GetColour()
        {
            return Text.color;
        }

        /// <inheritdoc />
        public override void UpdateColour(Color colour, float percentage)
        {
            Text.color = colour;
        }
    }
}