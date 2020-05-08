// Created by Kearan Petersen : https://www.blumalice.wordpress.com | https://www.linkedin.com/in/kearan-petersen/

#if UNITY_EDITOR
using UnityEditor;

namespace SOFlow.Fading
{
    [CustomEditor(typeof(MaterialFadable))]
    public class MaterialFadableEditor : Editor
    {
        /// <summary>
        ///     The MaterialFadable target.
        /// </summary>
        private MaterialFadable _target;

        private SerializedProperty _alphaOnly;
        private SerializedProperty _invertAlpha;
        private SerializedProperty _invertPercentage;
        private SerializedProperty _useRenderer;
        private SerializedProperty _targetRenderer;
        private SerializedProperty _targetMaterial;
        private SerializedProperty _overrideColourProperty;
        private SerializedProperty _colourProperty;

        private void OnEnable()
        {
            _target = (MaterialFadable) target;

            _alphaOnly              = serializedObject.FindProperty(nameof(_target.AlphaOnly));
            _invertAlpha            = serializedObject.FindProperty(nameof(_target.InvertAlpha));
            _invertPercentage       = serializedObject.FindProperty(nameof(_target.InvertPercentage));
            _useRenderer            = serializedObject.FindProperty(nameof(_target.UseRenderer));
            _targetRenderer         = serializedObject.FindProperty(nameof(_target.TargetRenderer));
            _targetMaterial         = serializedObject.FindProperty(nameof(_target.TargetMaterial));
            _overrideColourProperty = serializedObject.FindProperty(nameof(_target.OverrideColourProperty));
            _colourProperty         = serializedObject.FindProperty(nameof(_target.ColourProperty));
        }

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            }

            EditorGUILayout.PropertyField(_alphaOnly);
            EditorGUILayout.PropertyField(_invertAlpha);
            EditorGUILayout.PropertyField(_invertPercentage);
            EditorGUILayout.PropertyField(_useRenderer);

            if (_target.UseRenderer)
            {
                EditorGUILayout.PropertyField(_targetRenderer);
            }
            else
            {
                EditorGUILayout.PropertyField(_targetMaterial);
            }

            EditorGUILayout.PropertyField(_overrideColourProperty);

            if (_target.OverrideColourProperty)
            {
                EditorGUILayout.PropertyField(_colourProperty);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif