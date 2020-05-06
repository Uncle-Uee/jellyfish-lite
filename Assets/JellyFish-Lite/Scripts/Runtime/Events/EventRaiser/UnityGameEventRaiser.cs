// Created by Kearan Petersen : https://www.blumalice.wordpress.com | https://www.linkedin.com/in/kearan-petersen/

using System.Collections;
using JellyFish.Data.Primitive;
using UnityEngine;
using UnityEngine.Events;

namespace JellyFish.Data.Events
{
    public class UnityGameEventRaiser : MonoBehaviour
    {
        /// <summary>
        ///     The game event to raise.
        /// </summary>
        public UnityEvent Event = new UnityEvent();

        /// <summary>
        ///     The time before the event is raised in seconds.
        /// </summary>
        public FloatField EventWaitTime = new FloatField();

        /// <summary>
        ///     Indicates whether the event should be raised automatically when Start is called.
        /// </summary>
        public BoolField RaiseOnStart = new BoolField(true);
        

        /// <summary>
        /// Indicates whether the event should be repeated.
        /// </summary>
        public BoolField RepeatEvent = new BoolField(false);

        /// <summary>
        /// Delay before Invoking Event.
        /// </summary>
        private WaitForSeconds _delay;

        private void Awake()
        {
            _delay = new WaitForSeconds(EventWaitTime);
        }

        private void Start()
        {
            if (RaiseOnStart) StartCoroutine(nameof(RaiseEvent));
        }

        /// <summary>
        ///     Raises this event after the specified period of time has passed.
        /// </summary>
        public IEnumerator RaiseEvent()
        {
            yield return _delay;
            Event.Invoke();

            while (RepeatEvent)
            {
                yield return _delay;
                Event.Invoke();
            }
        }
    }
}