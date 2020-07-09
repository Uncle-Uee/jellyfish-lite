// Created by Kearan Petersen : https://www.blumalice.wordpress.com | https://www.linkedin.com/in/kearan-petersen/

using UnityEngine;
using UnityEngine.Events;

namespace JellyFish.Internal.Utilities
{
    [CreateAssetMenu(menuName = "JellyFish/Utilities/Camera/Resolution State", order = 30)]
    public class ResolutionState : ScriptableObject
    {
        /// <summary>
        ///     The current screen resolution.
        /// </summary>
        private Vector2 _currentScreenResolution;

        /// <summary>
        /// The Current World Size.
        /// </summary>
        private Vector2 _currentWorldSize;

        /// <summary>
        ///     The screen resolution the application was designed for.
        /// </summary>
        public Vector2 DesignedScreenResolution;

        /// <summary>
        ///     Event raised when the screen resolution changes.
        /// </summary>
        public UnityEvent OnScreenResolutionChanged = new UnityEvent();

        /// <summary>
        ///     The current screen resolution.
        /// </summary>
        public Vector2 CurrentScreenResolution
        {
            get => _currentScreenResolution;
            set
            {
                _currentScreenResolution = value;
                OnScreenResolutionChanged.Invoke();
            }
        }

        /// <summary>
        /// The Current World Screen Size.
        /// </summary>
        public Vector2 CurrentWorldScreenSize
        {
            get => _currentWorldSize;
            set => _currentWorldSize = value;
        }

        /// <summary>
        /// Bound a Transform within the World.
        /// </summary>
        /// <param name="localPosition"></param>
        /// <returns></returns>
        public void BoundWithinScreen(ref Vector3 localPosition)
        {
            float worldHeight = _currentWorldSize.y;
            float worldWidth  = _currentWorldSize.x;

            localPosition.y = Mathf.Clamp(localPosition.y, -worldHeight, worldHeight);
            localPosition.x = Mathf.Clamp(localPosition.x, -worldWidth, worldWidth);
        }
    }
}