// Created by Kearan Petersen : https://www.blumalice.wordpress.com | https://www.linkedin.com/in/kearan-petersen/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SOFlow.Fading
{
    public class GenericFader : MonoBehaviour
    {
        /// <summary>
        ///     The fade curve.
        /// </summary>
        public AnimationCurve FadeCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

        /// <summary>
        ///     The faded colour.
        /// </summary>
        public Color FadedColour = Color.white;

        /// <summary>
        ///     The fade targets.
        /// </summary>
        public List<Fadable> FadeTargets = new List<Fadable>();

        /// <summary>
        ///     The fade time.
        /// </summary>
        public float FadeTime;

        /// <summary>
        ///     Event raised when the fading is completed.
        /// </summary>
        public UnityEvent OnFadeComplete = new UnityEvent();

        /// <summary>
        ///     Event raised before the fade starts.
        /// </summary>
        public UnityEvent OnFadeStart = new UnityEvent();

        /// <summary>
        ///     Event raised when waiting between fades.
        /// </summary>
        public UnityEvent OnFadeWait = new UnityEvent();

        /// <summary>
        ///     Enable to only allow fading in.
        /// </summary>
        public bool OnlyFade;

        /// <summary>
        ///     The unfade curve.
        /// </summary>
        public AnimationCurve UnfadeCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

        /// <summary>
        ///     The unfaded colour.
        /// </summary>
        public Color UnfadedColour = Color.white;

        /// <summary>
        ///     The unfade time.
        /// </summary>
        public float UnfadeTime;

        /// <summary>
        ///     The wait between fades.
        /// </summary>
        public float WaitBetweenFades;

        /// <summary>
        ///     Indicates whether we are currently fading.
        /// </summary>
        public bool Fading { get; private set; }

        /// <summary>
        /// Cached Wait For Seconds
        /// </summary>
        private WaitForSeconds _waitBetweenFrames;

        #region UNITY METHODS

        private void Awake()
        {
            _waitBetweenFrames = new WaitForSeconds(WaitBetweenFades);
        }

        #endregion


        #region METHODS

        public void StartFade()
        {
            StopCoroutine(nameof(Fade));
            StartCoroutine(nameof(Fade));
        }

        public void StartUnfade()
        {
            StopCoroutine(nameof(Unfade));
            StartCoroutine(nameof(Unfade));
        }

        /// <summary>
        ///     Initiates the fade.
        /// </summary>
        public IEnumerator Fade()
        {
            if (!Fading && gameObject.activeInHierarchy)
            {
                OnFadeStart.Invoke();
                Fading = true;

                float startTime = Time.realtimeSinceStartup;
                float endTime   = startTime + FadeTime;

                while (Time.realtimeSinceStartup < endTime)
                {
                    float percentage = (Time.realtimeSinceStartup - startTime) /
                                       (endTime                   - startTime);

                    foreach (Fadable fadable in FadeTargets)
                        fadable.OnUpdateColour(Color.Lerp(UnfadedColour, FadedColour, FadeCurve.Evaluate(percentage)),
                                               percentage);

                    yield return null;
                }

                foreach (Fadable fadable in FadeTargets) fadable.OnUpdateColour(FadedColour, 1f);

                if (!OnlyFade)
                {
                    OnFadeWait.Invoke();

                    yield return _waitBetweenFrames;

                    StopCoroutine(nameof(Unfade));
                    StartCoroutine(nameof(Unfade));
                }
                else
                {
                    Fading = false;
                    OnFadeComplete.Invoke();
                }
            }
        }

        private IEnumerator Unfade()
        {
            float startTime = Time.realtimeSinceStartup;
            float endTime   = startTime + UnfadeTime;

            while (Time.realtimeSinceStartup < endTime)
            {
                float percentage = (Time.realtimeSinceStartup - startTime) /
                                   (endTime                   - startTime);

                foreach (Fadable fadable in FadeTargets)
                    fadable.OnUpdateColour(Color.Lerp(FadedColour, UnfadedColour, UnfadeCurve.Evaluate(percentage)),
                                           percentage);

                yield return null;
            }

            foreach (Fadable fadable in FadeTargets) fadable.OnUpdateColour(UnfadedColour, 0f);

            Fading = false;
            OnFadeComplete.Invoke();
        }

        #endregion
    }
}