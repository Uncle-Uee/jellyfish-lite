// Created by Kearan Petersen : https://www.blumalice.wordpress.com | https://www.linkedin.com/in/kearan-petersen/

using UnityEngine;

namespace JellyFish.Internal.Utilities
{
    [RequireComponent(typeof(Camera))]
    public class GameCamera : MonoBehaviour
    {
        /// <summary>
        ///     The scene game camera reference.
        /// </summary>
        [Header("Game Camera")]
        public Camera SceneCameraReference;


        /// <summary>
        ///     The game camera reference.
        /// </summary>
        [Header("Camera Reference")]
        public CameraReference GameCameraReference;


        /// <summary>
        ///     Registers the game camera.
        /// </summary>
        public void Awake()
        {
            GameCameraReference.Camera = SceneCameraReference;
        }
    }
}