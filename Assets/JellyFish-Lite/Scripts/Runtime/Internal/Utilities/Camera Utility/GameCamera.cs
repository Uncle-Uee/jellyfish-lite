// Created by Kearan Petersen : https://www.blumalice.wordpress.com | https://www.linkedin.com/in/kearan-petersen/

using UnityEngine;

namespace JellyFish.Internal.Utilities
{
    [RequireComponent(typeof(Camera))]
    public class GameCamera : MonoBehaviour
    {
        /// <summary>
        ///     Indicates whether the game camera has already been registered.
        /// </summary>
        private static bool _hasRegisteredGameCamera;

        /// <summary>
        ///     Indicates whether this is the primary game camera.
        /// </summary>
        private bool _primaryGameCamera;

        /// <summary>
        ///     The game camera reference.
        /// </summary>
        public CameraReference GameCameraReference;

        /// <summary>
        ///     The scene game camera reference.
        /// </summary>
        public Camera SceneCameraReference;

        /// <summary>
        /// Do not Destroy Instance.
        /// </summary>
        public bool DontDestroy = false;

        /// <summary>
        ///     Registers the game camera.
        /// </summary>
        public void Start()
        {
            if (!_hasRegisteredGameCamera)
            {
                GameCameraReference.Camera = SceneCameraReference;

                if (DontDestroy)
                {
                    DontDestroyOnLoad(gameObject.ParentGameObject());
                }

                _hasRegisteredGameCamera = true;
                _primaryGameCamera       = true;
            }
            else
            {
                if (!_primaryGameCamera && !DontDestroy)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}