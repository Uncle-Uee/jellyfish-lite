using System;
using System.Collections;
using System.Collections.Generic;
using JellyFish.Data.Primitive;
using UnityEngine;
using UnityEngine.Events;

namespace JellyFish.Data.Events
{
    public class UnityGameEventListener : MonoBehaviour, IEventListener
    {
        /// <summary>
        /// The delay before the event response is invoked.
        /// </summary>
        public FloatField ResponseDelay = new FloatField();

        /// <summary>
        ///     The game event to listen for.
        /// </summary>
        public List<GameEvent> Events = new List<GameEvent>();

        /// <summary>
        ///     Indicates whether this listener should be prioritized after other listeners when events are raised.
        /// </summary>
        public BoolField RegisterLast = new BoolField();

        /// <summary>
        ///     Indicates whether the event listener should register to the given event on Awake.
        /// </summary>
        public BoolField RegisterOnAwake = new BoolField();

        /// <summary>
        ///     Indicates whether this event should be debugged.
        /// </summary>
        public BoolField Debug = new BoolField();

        /// <summary>
        ///     The response to the game event.
        /// </summary>
        public UnityEvent Response = new UnityEvent();

        /// <summary>
        ///     The game object reference.
        /// </summary>
        private GameObject _gameObjectReference;

        /// <inheritdoc />
        public IEnumerator OnEventRaised(GameEvent raisedEvent)
        {
            if (ResponseDelay > 0f) yield return new WaitForSeconds(ResponseDelay);
            Response.Invoke();
            if (Debug) UnityEngine.Debug.Log($"|Game Event Listener| Responding to event : \n{raisedEvent.name}");
        }

        public void OnEventRaisedRoutine(GameEvent raisedEvent)
        {
            StartCoroutine(OnEventRaised(raisedEvent));
        }

        /// <inheritdoc />
        public GameObject GetGameObject()
        {
            return _gameObjectReference;
        }

        /// <inheritdoc />
        public Type GetObjectType()
        {
            return GetType();
        }

        /// <inheritdoc />
        public List<GameEvent> GetEvents()
        {
            return Events;
        }

        private void Awake()
        {
            _gameObjectReference = gameObject;

            if (!Application.isPlaying) return;

            if (RegisterOnAwake)
                foreach (GameEvent @event in Events)
                {
                    if (@event == null)
                    {
                        UnityEngine.Debug
                                   .LogWarning($"[Game Event Listener Set] No event supplied for listener at : \n{transform.GetPath()}");

                        continue;
                    }

                    @event.RegisterListener(this, RegisterLast);
                }
        }

        private void OnEnable()
        {
            if (!Application.isPlaying) return;

            if (!RegisterOnAwake)
            {
                foreach (GameEvent @event in Events)
                {
                    if (@event == null)
                    {
                        UnityEngine.Debug
                                   .LogWarning($"[Game Event Listener Set] No event supplied for listener at : \n{transform.GetPath()}");

                        continue;
                    }

                    @event.RegisterListener(this, RegisterLast);
                }
            }
        }

        private void OnDisable()
        {
            if (!Application.isPlaying) return;

            if (!RegisterOnAwake)
            {
                StopAllCoroutines();

                foreach (GameEvent @event in Events) @event.DeregisterListener(this);
            }
        }

        private void OnDestroy()
        {
            if (!Application.isPlaying) return;

            if (RegisterOnAwake)
            {
                StopAllCoroutines();

                foreach (GameEvent @event in Events) @event.DeregisterListener(this);
            }
        }
    }
}