/**
 * Created By: Ubaidullah Effendi-Emjedi
 * LinkedIn : https://www.linkedin.com/in/ubaidullah-effendi-emjedi-202494183/
 */

using System.Globalization;
using JellyFish.Data.Primitive;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace JellyFish.Monitor.FPS
{
    public class FPSMonitor : MonoBehaviour
    {
        #region VARIABLES

        /// <summary>
        /// Show FPS Log.
        /// </summary>
        [Header("Status")]
        public BoolField ShowFPS;

        [Header("FPS Properties")]
        public float DelayInSeconds = 0.1f;

        /// <summary>
        /// FPS Label.
        /// </summary>
        private string _fpsLabel = "";

        /// <summary>
        /// FPS Counter.
        /// </summary>
        private float _fpsCounter;

        /// <summary>
        /// Play Mode FPS Rect 
        /// </summary>
        private Rect _playModeRect = new Rect(Screen.width, 4, 96, 24);

        /// <summary>
        /// Pause Mode FPS Rect
        /// </summary>
        private Rect _pausedModeRect = new Rect(Screen.width, 4, 96, 24);

        /// <summary>
        /// Delay Timer
        /// </summary>
        private float _timer = 0f;

        #endregion

        #region UNITY METHODS

        private void LateUpdate()
        {
            if (!ShowFPS) return;
            if (DelayTimer()) return;
            CalculateFps();
        }

        private void OnGUI()
        {
            if (!ShowFPS) return;

            SetFpsLabel();
            UpdateLabelRectXOffset();
            DrawFpsLabel();
        }

        #endregion

        #region METHODS

        private bool DelayTimer()
        {
            _timer += Time.deltaTime;
            if (!(_timer >= DelayInSeconds)) return true;
            _timer = 0f;
            return false;
        }

        private void CalculateFps()
        {
            if (Time.timeScale > 0)
            {
                _fpsCounter = 1f / Time.deltaTime;
            }
        }

        private void SetFpsLabel()
        {
            _fpsLabel = Time.timeScale > 0 ? Mathf.Round(_fpsCounter).ToString(CultureInfo.InstalledUICulture) : "Paused";
        }

        private void UpdateLabelRectXOffset()
        {
            _playModeRect.x   = Screen.width    - 32;
            _pausedModeRect.x = _playModeRect.x - 8;
        }

        private void DrawFpsLabel()
        {
            GUI.Label(Time.timeScale > 0 ? _playModeRect : _pausedModeRect, _fpsLabel);
        }

        #endregion
    }
}