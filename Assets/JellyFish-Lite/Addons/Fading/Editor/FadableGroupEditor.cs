// Created by Kearan Petersen : https://www.blumalice.wordpress.com | https://www.linkedin.com/in/kearan-petersen/

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SOFlow.Fading
{
    [CustomEditor(typeof(FadableGroup))]
    public class FadableGroupEditor : Editor
    {
        /// <summary>
        ///     The FadableGroup target.
        /// </summary>
        private FadableGroup _target;

        private SerializedProperty _alphaOnly;
        private SerializedProperty _invertAlpha;
        private SerializedProperty _invertPercentage;
        private SerializedProperty _fadables;


        private void OnEnable()
        {
            _target = (FadableGroup) target;

            _alphaOnly        = serializedObject.FindProperty(nameof(_target.AlphaOnly));
            _invertAlpha      = serializedObject.FindProperty(nameof(_target.InvertAlpha));
            _invertPercentage = serializedObject.FindProperty(nameof(_target.InvertPercentage));
            _fadables         = serializedObject.FindProperty(nameof(_target.Fadables));
        }

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Capture Child Fadables"))
                _target.CaptureChildFadables();

            EditorGUILayout.PropertyField(_alphaOnly);
            EditorGUILayout.PropertyField(_invertAlpha);
            EditorGUILayout.PropertyField(_invertPercentage);
            EditorGUILayout.PropertyField(_fadables);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif