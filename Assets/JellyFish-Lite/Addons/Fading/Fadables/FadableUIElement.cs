// Created by Kearan Petersen : https://www.blumalice.wordpress.com | https://www.linkedin.com/in/kearan-petersen/

using UnityEditor;
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

#if UNITY_EDITOR
        /// <summary>
        ///     Adds a Fadable UI Element to the scene.
        /// </summary>
        [MenuItem("GameObject/SOFlow/Fading/Fadables/Add Fadable UI Element", false, 10)]
        public static void AddComponentToScene()
        {
            CanvasRenderer canvasRenderer = Selection.activeGameObject?.GetComponent<CanvasRenderer>();
            
            if(canvasRenderer != null)
            {
                FadableUIElement fadable = canvasRenderer.gameObject.AddComponent<FadableUIElement>();
                fadable.UIElement = canvasRenderer;

                return;
            }

            GameObject _gameObject = new GameObject("Fadable UI Element", typeof(FadableUIElement));

            if(Selection.activeTransform != null)
            {
                _gameObject.transform.SetParent(Selection.activeTransform);
            }

            Selection.activeGameObject = _gameObject;
        }
#endif
    }
}