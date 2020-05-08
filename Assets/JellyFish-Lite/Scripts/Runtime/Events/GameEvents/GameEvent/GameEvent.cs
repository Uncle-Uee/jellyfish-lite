// Created by Kearan Petersen : https://www.blumalice.wordpress.com | https://www.linkedin.com/in/kearan-petersen/

using System;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace JellyFish.Data.Events
{
    [CreateAssetMenu(menuName = "JellyFish/Events/Game Event", order = 20)]
    public class GameEvent : ScriptableObject
    {
        /// <summary>
        ///     All listeners registered to this game event.
        /// </summary>
        [HideInInspector]
        public List<IEventListener> Listeners = new List<IEventListener>();

        /// <summary>
        ///     Notifies all registered listeners to invoke their events.
        /// </summary>
        public void Raise()
        {
            for (int i = Listeners.Count - 1; i >= 0; i--)
            {
                try
                {
                    Listeners[i].OnEventRaisedRoutine(this);
                }
                catch (Exception e)
                {
                    Exception baseException = e.GetBaseException();

                    Debug.LogError($"[Game Event Error] {DateTime.Now:T} | {i} | {name} | {(Listeners[i].GetGameObject() == null ? "No Game Object" : Listeners[i].GetGameObject().name)} | {Listeners[i].GetObjectType().Name}");
                    Debug.LogException(baseException);
                }
            }
        }

        /// <summary>
        ///     Registers a listener to this game event.
        /// </summary>
        /// <param name="listener"></param>
        /// <param name="registerLast"></param>
        public void RegisterListener(IEventListener listener, bool registerLast = false)
        {
            if (registerLast)
                Listeners.Insert(0, listener);
            else
                Listeners.Add(listener);
        }

        /// <summary>
        ///     Deregisters a listener from this game event.
        /// </summary>
        /// <param name="listener"></param>
        public void DeregisterListener(IEventListener listener)
        {
            Listeners.Remove(listener);
        }
    }
}