// Created by Kearan Petersen : https://www.blumalice.wordpress.com | https://www.linkedin.com/in/kearan-petersen/

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace JellyFish.ObjectPooling
{
    [CustomEditor(typeof(PoolObjectListReference))]
    public class PoolObjectListReferenceEditor : UnityEditor.Editor
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

            EditorGUILayout.LabelField("Total Pool Objects");

            EditorGUILayout.LabelField(_target.PoolObjectCount.ToString());
            foreach (IPoolObjectRoot poolObject in _target.PoolObjects)
            {
                EditorGUILayout.ObjectField(poolObject.GetObjectInstance(), typeof(Object), false);
            }
        }
    }
}
#endif