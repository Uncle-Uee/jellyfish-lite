#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace JellyFish.Data.Events
{
    [CustomEditor(typeof(GameEvent))]
    public class GameEventEditor : UnityEditor.Editor
    {
        #region VARIABLES

        private GameEvent _target;

        #endregion

        #region UNITY METHODS

        private void OnEnable()
        {
            _target = target as GameEvent;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Raise"))
            {
                if (_target == null) return;
                _target.Raise();
            }
        }

        #endregion
    }
}
#endif