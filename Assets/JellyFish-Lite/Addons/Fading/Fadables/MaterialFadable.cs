// Created by Kearan Petersen : https://www.blumalice.wordpress.com | https://www.linkedin.com/in/kearan-petersen/

using JellyFish.Data.Primitive;
using UnityEngine;

namespace SOFlow.Fading
{
    public class MaterialFadable : Fadable
    {
        /// <summary>
        /// Indicates whether a renderer or material should be used as a reference.
        /// </summary>
        public BoolField UseRenderer = new BoolField(true);

        /// <summary>
        /// The target renderer to fade.
        /// </summary>
        public Renderer TargetRenderer;

        /// <summary>
        /// The target material to fade.
        /// </summary>
        public Material TargetMaterial;

        /// <summary>
        /// Indicates whether the material colour property should be manually specified.
        /// </summary>
        public BoolField OverrideColourProperty = new BoolField(false);

        /// <summary>
        /// The colour property to adjust.
        /// </summary>
        public StrField ColourProperty = new StrField("_Color");

        /// <inheritdoc />
        protected override Color GetColour()
        {
            Material materialReference = UseRenderer ? TargetRenderer.sharedMaterial : TargetMaterial;

            return OverrideColourProperty ? materialReference.GetColor(ColourProperty) : materialReference.color;
        }

        /// <inheritdoc />
        public override void UpdateColour(Color colour, float percentage)
        {
            Material materialReference = UseRenderer ? TargetRenderer.sharedMaterial : TargetMaterial;

            if (OverrideColourProperty)
            {
                materialReference.SetColor(ColourProperty, colour);
            }
            else
            {
                materialReference.color = colour;
            }
        }
    }
}