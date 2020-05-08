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

#if UNITY_EDITOR
        /// <summary>
        ///     Adds a TextMesh Pro Fadable to the scene.
        /// </summary>
        [MenuItem("GameObject/SOFlow/Fading/Fadables/Add TextMesh Pro Fadable", false, 10)]
        public static void AddComponentToScene()
        {
            TMP_Text text = Selection.activeGameObject?.GetComponent<TMP_Text>();

            if(text != null)
            {
                TextMeshProFadable fadable = text.gameObject.AddComponent<TextMeshProFadable>();
                fadable.Text = text;

                return;
            }

            GameObject _gameObject = new GameObject("TextMesh Pro Fadable", typeof(TextMeshProFadable));

            if(Selection.activeTransform != null)
            {
                _gameObject.transform.SetParent(Selection.activeTransform);
            }

            Selection.activeGameObject = _gameObject;
        }
#endif
    }
}