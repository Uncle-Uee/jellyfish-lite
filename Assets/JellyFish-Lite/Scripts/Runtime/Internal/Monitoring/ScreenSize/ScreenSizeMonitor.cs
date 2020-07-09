/**
 * Created By: Ubaidullah Effendi-Emjedi
 * LinkedIn : https://www.linkedin.com/in/ubaidullah-effendi-emjedi-202494183/
 */

using System;
using System.Collections;
using JellyFish.Internal.Utilities;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace JellyFish.Monitor.ScreenSize
{
    public class ScreenSizeMonitor : MonoBehaviour
    {
        #region VARIABLES

        /// <summary>
        /// Monitor Screen Resolutions
        /// </summary>
        [Header("Status")]
        public bool MonitorResolution = false;

        /// <summary>
        /// Only Calculate World Size Flag.
        /// </summary>
        [Header("Settings")]
        public bool OnlyCalculateWorldSize = false;

        /// <summary>
        /// Only Calculate Screen Size Flag.
        /// </summary>
        public bool OnlyCalculateScreenSize = false;

        /// <summary>
        /// Game Camera Reference
        /// </summary>
        [Header("Game Camera Reference")]
        public CameraReference GameCamera;

        /// <summary>
        /// Resolution State.
        /// </summary>
        [Header("Resolution State")]
        public ResolutionState ResolutionState;

        /// <summary>
        /// Current Screen Size
        /// </summary>
        private Vector2Int _currentScreenSize;

        /// <summary>
        /// Previous Screen Size.
        /// </summary>
        private Vector2Int _previousScreenSize;

        /// <summary>
        /// Current World Size
        /// </summary>
        private Vector2 _currentWorldSize;

        /// <summary>
        /// Previous World Size.
        /// </summary>
        private Vector2 _previousWorldSize;

        /// <summary>
        /// On Start Monitoring Event
        /// </summary>
        private static event Action OnStartMonitoring;

        /// <summary>
        /// On Stop Monitoring Event.
        /// </summary>
        private static event Action OnStopMonitoring;

        #endregion

        #region UNITY METHODS

        private void OnEnable()
        {
            StartMonitoring();
            OnStartMonitoring += StartMonitoring;
            OnStopMonitoring  += StopMonitoring;
        }

        private void OnDisable()
        {
            StopMonitoring();
            OnStartMonitoring -= StartMonitoring;
            OnStopMonitoring  -= StopMonitoring;
        }

        #endregion

        #region SCREEN SIZE MONITOR METHODS

        /// <summary>
        /// Calculate the World and Screen Size.
        /// </summary>
        /// <param name="cameraPixelRect"></param>
        /// <param name="orthographicSize"></param>
        /// <param name="aspect"></param>
        /// <returns></returns>
        private IEnumerator ScreenSizeAndWorldBounds()
        {
            while (MonitorResolution)
            {
                CalculateScreenSizeAndWorldBounds(GameCamera.Camera.pixelRect, GameCamera.Camera.orthographicSize, GameCamera.Camera.aspect);
                yield return null;
            }
        }

        /// <summary>
        /// Calculate the World and Screen Size.
        /// </summary>
        private void CalculateScreenSizeAndWorldBounds(Rect cameraPixelRect, float orthographicSize, float aspect)
        {
            if (OnlyCalculateWorldSize)
            {
                CalculateWorldSize(orthographicSize, aspect);
            }

            if (OnlyCalculateScreenSize)
            {
                CalculateScreenResolution(cameraPixelRect);
            }
        }

        /// <summary>
        /// Calculate the World Size.
        /// </summary>
        /// <param name="orthographicSize"></param>
        /// <param name="aspect"></param>
        private void CalculateWorldSize(float orthographicSize, float aspect)
        {
            _currentWorldSize.y = orthographicSize;
            _currentWorldSize.x = _currentWorldSize.y * aspect;

            if (Mathf.Approximately(_currentWorldSize.x, _previousWorldSize.x) && Mathf.Approximately(_currentWorldSize.y, _previousWorldSize.y)) return;

            _previousWorldSize.y                   = _currentWorldSize.y;
            _previousWorldSize.x                   = _currentWorldSize.x;
            ResolutionState.CurrentWorldScreenSize = _previousWorldSize;
            print("World Size Changed!");
        }

        /// <summary>
        /// Calculate the Screen Resolution.
        /// </summary>
        /// <param name="cameraPixelRect"></param>
        private void CalculateScreenResolution(Rect cameraPixelRect)
        {
            _currentScreenSize.x = (int) cameraPixelRect.width;
            _currentScreenSize.y = (int) cameraPixelRect.height;

            if (_currentScreenSize.x == _previousScreenSize.x || _currentScreenSize.y == _previousScreenSize.y) return;
            _previousScreenSize.x                   = _currentScreenSize.x;
            _previousScreenSize.y                   = _currentScreenSize.y;
            ResolutionState.CurrentScreenResolution = _previousScreenSize;
            print("Screen Resolution Changed!");
        }

        /// <summary>
        /// Bound a Transform within the World.
        /// </summary>
        /// <param name="localPosition"></param>
        /// <returns></returns>
        public void BoundWithinScreen(ref Vector3 localPosition)
        {
            float worldWidth  = ResolutionState.CurrentWorldScreenSize.x;
            float worldHeight = ResolutionState.CurrentWorldScreenSize.y;

            localPosition.y = Mathf.Clamp(localPosition.y, -worldHeight, worldHeight);
            localPosition.x = Mathf.Clamp(localPosition.x, -worldWidth, worldWidth);
        }

        private void StartMonitoring()
        {
            MonitorResolution = true;
            StopCoroutine(ScreenSizeAndWorldBounds());
            StartCoroutine(ScreenSizeAndWorldBounds());
        }

        private void StopMonitoring()
        {
            MonitorResolution = false;
            StopCoroutine(ScreenSizeAndWorldBounds());
        }

        /// <summary>
        /// Call Start Monitoring Coroutine
        /// </summary>
        public static void CallStartMonitoring()
        {
            OnStartMonitoring?.Invoke();
        }

        /// <summary>
        /// Call Stop Monitoring Coroutine
        /// </summary>
        public static void CallStopMonitoring()
        {
            OnStopMonitoring?.Invoke();
        }

        #endregion
    }
}