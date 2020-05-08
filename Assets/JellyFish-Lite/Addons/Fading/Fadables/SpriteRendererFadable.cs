// Created by Kearan Petersen : https://www.blumalice.wordpress.com | https://www.linkedin.com/in/kearan-petersen/

using UnityEditor;
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

#if UNITY_EDITOR
        /// <summary>
        ///     Adds a Sprite Renderer Fadable to the scene.
        /// </summary>
        [MenuItem("GameObject/SOFlow/Fading/Fadables/Add Sprite Renderer Fadable", false, 10)]
        public static void AddComponentToScene()
        {
            SpriteRenderer sprite = Selection.activeGameObject?.GetComponent<SpriteRenderer>();

            if(sprite != null)
            {
                SpriteRendererFadable fadable = sprite.gameObject.AddComponent<SpriteRendererFadable>();
                fadable.SpriteRenderer = sprite;

                return;
            }

            GameObject _gameObject = new GameObject("Sprite Renderer Fadable", typeof(SpriteRendererFadable));

            if(Selection.activeTransform != null)
            {
                _gameObject.transform.SetParent(Selection.activeTransform);
            }

            Selection.activeGameObject = _gameObject;
        }
#endif
    }
}