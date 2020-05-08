// Created by Kearan Petersen : https://www.blumalice.wordpress.com | https://www.linkedin.com/in/kearan-petersen/

#if UNITY_EDITOR
using UnityEditor;

namespace JellyFish.Fading
{
    [CustomEditor(typeof(MaterialFadable))]
    public class MaterialFadableEditor : UnityEditor.Editor
    {
        /// <summary>
        ///     The MaterialFadable target.
        /// </summary>
        private MaterialFadable _target;

        private void OnEnable()
        {
            _target = (MaterialFadable) target;
        }

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            // using (new EditorGUI.DisabledScope(true))
            // {
            //     EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            // }
            //
            // serializedObject.DrawProperty(nameof(_target.AlphaOnly));
            // serializedObject.DrawProperty(nameof(_target.InvertAlpha));
            // serializedObject.DrawProperty(nameof(_target.InvertPercentage));
            // serializedObject.DrawProperty(nameof(_target.UseRenderer));
            //
            // if (_target.UseRenderer)
            // {
            //     serializedObject.DrawProperty(nameof(_target.TargetRenderer));
            // }
            // else
            // {
            //     serializedObject.DrawProperty(nameof(_target.TargetMaterial));
            // }
            //
            // serializedObject.DrawProperty(nameof(_target.OverrideColourProperty));
            //
            // if (_target.OverrideColourProperty)
            // {
            //     serializedObject.DrawProperty(nameof(_target.ColourProperty));
            // }
        }
    }
}
#endif