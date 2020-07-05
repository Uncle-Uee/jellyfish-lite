// Created by Kearan Petersen : https://www.blumalice.wordpress.com | https://www.linkedin.com/in/kearan-petersen/

#if UNITY_EDITOR
using UnityEditor;

namespace SOFlow.Fading
{
    [CustomEditor(typeof(GenericFader))]
    public class GenericFaderEditor : Editor
    {
        /// <summary>
        ///     The GenericFader target.
        /// </summary>
        private GenericFader _target;

        private SerializedProperty _fadeTargets;
        private SerializedProperty _unfadedColour;
        private SerializedProperty _fadedColour;
        private SerializedProperty _fadeCurve;
        private SerializedProperty _unfadeCurve;
        private SerializedProperty _onlyFade;
        private SerializedProperty _fadeTime;
        private SerializedProperty _unfadeTime;
        private SerializedProperty _waitBetweenFades;
        private SerializedProperty _onFadeStart;
        private SerializedProperty _onFadeWait;
        private SerializedProperty _onFadeComplete;

        private void OnEnable()
        {
            _target = (GenericFader) target;

            _fadeTargets = serializedObject.FindProperty(nameof(_target.FadeTargets));
            _unfadedColour    = serializedObject.FindProperty(nameof(_target.UnfadedColour));
            _fadedColour      = serializedObject.FindProperty(nameof(_target.FadedColour));
            _fadeCurve        = serializedObject.FindProperty(nameof(_target.FadeCurve));
            _unfadeCurve      = serializedObject.FindProperty(nameof(_target.UnfadeCurve));
            _onlyFade         = serializedObject.FindProperty(nameof(_target.OnlyFade));
            _fadeTime         = serializedObject.FindProperty(nameof(_target.FadeTime));
            _unfadeTime       = serializedObject.FindProperty(nameof(_target.UnfadeTime));
            _waitBetweenFades = serializedObject.FindProperty(nameof(_target.WaitBetweenFades));
            _onFadeStart      = serializedObject.FindProperty(nameof(_target.OnFadeStart));
            _onFadeWait       = serializedObject.FindProperty(nameof(_target.OnFadeWait));
            _onFadeComplete   = serializedObject.FindProperty(nameof(_target.OnFadeComplete));
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
            EditorGUILayout.PropertyField(_unfadedColour);
            EditorGUILayout.PropertyField(_fadedColour);
            EditorGUILayout.PropertyField(_fadeCurve);


            if (!_target.OnlyFade)
            {
                EditorGUILayout.PropertyField(_unfadeCurve);
            }

            EditorGUILayout.PropertyField(_fadeTargets);
            EditorGUILayout.PropertyField(_onlyFade);
            EditorGUILayout.PropertyField(_fadeTime);

            if (!_target.OnlyFade)
            {
                EditorGUILayout.PropertyField(_unfadeTime);
                EditorGUILayout.PropertyField(_waitBetweenFades);
            }

            EditorGUILayout.PropertyField(_onFadeStart);

            if (!_target.OnlyFade)
            {
                EditorGUILayout.PropertyField(_onFadeWait);
            }

            EditorGUILayout.PropertyField(_onFadeComplete);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif