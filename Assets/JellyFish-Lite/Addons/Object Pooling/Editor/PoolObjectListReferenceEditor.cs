// Created by Kearan Petersen : https://www.blumalice.wordpress.com | https://www.linkedin.com/in/kearan-petersen/

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SOFlow.ObjectPooling
{
    [CustomEditor(typeof(PoolObjectListReference))]
    public class PoolObjectListReferenceEditor : Editor
    {
        /// <summary>
        /// The PoolObjectListReference target.
        /// </summary>
        public PoolObjectListReference _target;

        public void OnEnable()
        {
            _target = (PoolObjectListReference) target;
        }

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Total Pool Objects");
            EditorGUILayout.LabelField(_target.PoolObjectCount.ToString());
            EditorGUILayout.EndHorizontal();

            foreach (IPoolObjectRoot poolObject in _target.PoolObjects)
            {
                EditorGUILayout.ObjectField(poolObject.GetObjectInstance(), typeof(Object), false);
            }
        }
    }
}
#endif