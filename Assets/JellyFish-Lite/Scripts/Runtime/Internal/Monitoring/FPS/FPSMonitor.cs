/**
 * Created By: Ubaidullah Effendi-Emjedi
 * LinkedIn : https://www.linkedin.com/in/ubaidullah-effendi-emjedi-202494183/
 */

using System;
using System.Collections;
using System.Globalization;
using UnityEngine;

namespace JellyFish.Monitor.FPS
{
    public class FPSMonitor : MonoBehaviour
    {
        #region VARIABLES

        /// <summary>
        /// Show FPS Flag
        /// </summary>
        [Header("Status")]
        public bool MonitorFPS;
        public bool ShowButtons;

        /// <summary>
        /// Font Colour
        /// </summary>
        [Header("Properties")]
        public Color FontColour = Color.white;

        /// <summary>
        /// Font Size
        /// </summary>
        [Range(16, 24)]
        public int FontSize = 16;


        /// <summary>
        /// GuiStyle for Text.
        /// </summary>
        private GUIStyle _textStyle = new GUIStyle();

        /// <summary>
        /// FPS Label.
        /// </summary>
        private string _fpsLabel = "";

        /// <summary>
        /// FPS Counter.
        /// </summary>
        private float _fpsCounter;

        /// <summary>
        /// FPS Rect 
        /// </summary>
        private Rect _fpsRect = new Rect(0, 4, 48, 32);

        /// <summary>
        /// Pause Button Rect
        /// </summary>
        private readonly Rect _pauseButtonRect = new Rect(8, 8, 56, 20);

        /// <summary>
        /// Play Button Rect
        /// </summary>
        private readonly Rect _playButtonRect = new Rect(8, 32, 48, 20);

        /// <summary>
        /// OnShowFPS Event
        /// </summary>
        private static event Action OnShowFPS;

        /// <summary>
        /// OnHideFPS Event
        /// </summary>
        private static event Action OnHideFPS;

        #endregion

        #region UNITY METHODS

        private void Awake()
        {
            _textStyle = new GUIStyle
                         {
                             normal = new GUIStyleState
                                      {
                                          textColor = FontColour
                                      },
                             fontSize  = FontSize,
                             fontStyle = FontStyle.Bold,
                             alignment = TextAnchor.MiddleCenter
                         };
        }

        private void OnEnable()
        {
            _textStyle.normal.textColor = FontColour;
            _textStyle.fontSize         = FontSize;

            ShowFps();

            OnShowFPS += ShowFps;
            OnHideFPS += HideFps;
        }

        private void OnDisable()
        {
            HideFps();

            OnShowFPS += ShowFps;
            OnHideFPS += HideFps;
        }

        private void OnGUI()
        {
            if (!MonitorFPS) return;
            UpdateLabelRectXOffset();
            DrawFpsLabel();
            TimeScaleButtons();
        }

        #endregion

        #region METHODS

        private void ShowFps()
        {
            MonitorFPS = true;
            StopCoroutine(CalculateFps());
            StartCoroutine(CalculateFps());
        }

        private void HideFps()
        {
            MonitorFPS = false;
            StopCoroutine(CalculateFps());
        }

        public static void CallShowFps()
        {
            OnShowFPS?.Invoke();
        }

        public static void CallHideFps()
        {
            OnHideFPS?.Invoke();
        }

        private IEnumerator CalculateFps()
        {
            while (MonitorFPS)
            {
                if (Time.timeScale > 0)
                {
                    _fpsCounter = 1f / Time.deltaTime;
                }

                yield return null;
            }
        }

        private void UpdateLabelRectXOffset()
        {
            _fpsRect.x = Screen.width - _fpsRect.width;
        }

        private void DrawFpsLabel()
        {
            _fpsLabel = Time.timeScale > 0 ? Mathf.Round(_fpsCounter).ToString(CultureInfo.InstalledUICulture) : "○";
            GUI.Label(_fpsRect, _fpsLabel, _textStyle);
        }

        private void TimeScaleButtons()
        {
            if (!ShowButtons) return;

            if (GUI.Button(_pauseButtonRect, "Pause"))
            {
                Time.timeScale = 0;
            }

            if (GUI.Button(_playButtonRect, "Play"))
            {
                Time.timeScale = 1;
            }
        }

        #endregion
    }
}