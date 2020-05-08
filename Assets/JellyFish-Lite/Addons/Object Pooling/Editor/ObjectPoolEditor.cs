// Created by Kearan Petersen : https://www.blumalice.wordpress.com | https://www.linkedin.com/in/kearan-petersen/

#if UNITY_EDITOR
using System.Collections;
using System.Reflection;
using UnityEditor;

namespace SOFlow.ObjectPooling
{
    [CustomEditor(typeof(ObjectPoolBase), true)]
    public class ObjectPoolEditor : Editor
    {
        /// <summary>
        ///     The ObjectPool target.
        /// </summary>
        private ObjectPoolBase _target;

        private void OnEnable()
        {
            _target = (ObjectPoolBase) target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            IEnumerable pool = _target.GetPool();

            foreach (object objectSet in pool)
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.LabelField("ID");
                EditorGUILayout.LabelField($"Pool Count - {_target.CurrentPoolSize}");
                EditorGUILayout.EndHorizontal();

                PropertyInfo key   = objectSet.GetType().GetProperty("Key");
                PropertyInfo value = objectSet.GetType().GetProperty("Value");

                PropertyInfo valueCount = value.GetValue(objectSet).GetType().GetProperty("Count");

                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.LabelField(key.GetValue(objectSet).ToString());
                EditorGUILayout.LabelField(valueCount.GetValue(value.GetValue(objectSet)).ToString());
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
#endif