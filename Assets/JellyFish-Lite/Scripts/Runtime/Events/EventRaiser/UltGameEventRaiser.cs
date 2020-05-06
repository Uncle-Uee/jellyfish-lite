using System;
using System.Threading;
using JellyFish.Data.Primitive;
using UltEvents;
using UnityAsync;
using UnityEngine;

namespace JellyFish.Data.Events
{
    public class UltGameEventRaiser : MonoBehaviour
    {
        /// <summary>
        ///     The game event to raise.
        /// </summary>
        public UltEvent Event = new UltEvent();

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

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private void Start()
        {
            if (RaiseOnStart) StartCoroutine(nameof(RaiseEvent));
        }

        /// <summary>
        ///     Raises this event after the specified period of time has passed.
        /// </summary>
        public async void RaiseEvent()
        {
            try
            {
                // Null any Previous Cancellation Schedules
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = new CancellationTokenSource();
                Event.CancellationToken  = _cancellationTokenSource.Token;

                await Await.Seconds(EventWaitTime).ConfigureAwait(_cancellationTokenSource.Token);

                Event.Invoke();

                if (RepeatEvent)
                {
                    RaiseEvent();
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        public void CancelEvent()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}