// Created by Kearan Petersen : https://www.blumalice.wordpress.com | https://www.linkedin.com/in/kearan-petersen/

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace JellyFish.Fading
{
    [CustomEditor(typeof(GenericFader))]
    public class GenericFaderEditor : UnityEditor.Editor
    {
        /// <summary>
        ///     The GenericFader target.
        /// </summary>
        private GenericFader _target;

        private SerializedProperty _unfadedColour;
        private SerializedProperty _fadedColour;
        private SerializedProperty _fadeCurve;

        private void OnEnable()
        {
            _target = (GenericFader) target;
        }

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            DrawFaderInspector();
        }

        /// <summary>
        ///     Draws the fader inspector.
        /// </summary>
        private void DrawFaderInspector()
        {
            _unfadedColour = serializedObject.FindProperty("UnfadedColour");
            EditorGUILayout.PropertyField(_unfadedColour, new GUIContent("Unfaded Colour"));

            _fadedColour = serializedObject.FindProperty("FadedColour");
            EditorGUILayout.PropertyField(_fadedColour, new GUIContent("Faded Colour"));

            _fadeCurve = serializedObject.FindProperty("FadeCurve");
            EditorGUILayout.PropertyField(_fadeCurve, new GUIContent("Fade Curve"));
            // serializedObject.DrawProperty("UnfadedColour");
            // serializedObject.DrawProperty("FadedColour");
            // serializedObject.DrawProperty("FadeCurve");

            // if (!_target.OnlyFade) serializedObject.DrawProperty("UnfadeCurve");

            // serializedObject.DrawProperty("OnlyFade");
            // serializedObject.DrawProperty("FadeTime");

            // if (!_target.OnlyFade)
            // {
            // serializedObject.DrawProperty("UnfadeTime");
            // serializedObject.DrawProperty("WaitBetweenFades");
            // }

            // serializedObject.DrawProperty("OnFadeStart");

            // if (!_target.OnlyFade) serializedObject.DrawProperty("OnFadeWait");

            // serializedObject.DrawProperty("OnFadeComplete");
        }
    }
}
#endif