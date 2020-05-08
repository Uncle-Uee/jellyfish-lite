﻿// UltEvents // Copyright 2019 Kybernetik //

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityAsync;
using UnityEngine;
using UnityEngine.Events;

namespace UltEvents
{
    /// <summary>
    /// Allows you to expose the add and remove methods of an <see cref="UltEvent"/> without exposing the rest of its
    /// members such as the ability to invoke it.
    /// </summary>
    public interface IUltEventBase
    {
        /************************************************************************************************************************/

        /// <summary>Adds the specified 'method to the persistent call list.</summary>
        PersistentCall AddPersistentCall(Delegate method);

        /// <summary>Removes the specified 'method from the persistent call list.</summary>
        void RemovePersistentCall(Delegate method);

        /************************************************************************************************************************/
    }

    /// <summary>
    /// A serializable event which can be viewed and configured in the inspector.
    /// <para></para>
    /// This is a more versatile and user friendly implementation than <see cref="UnityEvent"/>.
    /// </summary>
    [Serializable]
    public abstract class UltEventBase : IUltEventBase
    {
        /************************************************************************************************************************/
        #region Fields and Properties
        /************************************************************************************************************************/

        public CancellationToken CancellationToken;
        
        [SerializeField]
        internal List<PersistentCall> _PersistentCalls;

        /// <summary>
        /// The serialized method and parameter details of this event.
        /// </summary>
        public List<PersistentCall> PersistentCallsList
        {
            get { return _PersistentCalls; }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// The non-serialized method and parameter details of this event.
        /// </summary>
        protected abstract Delegate DynamicCallsBase { get; set; }

        /// <summary>
        /// Clears the cached invocation list of <see cref="DynamicCallsBase"/>.
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        protected void OnDynamicCallsChanged()
        {
#if UNITY_EDITOR
            _DynamicCallInvocationList = null;
#endif
        }

        /************************************************************************************************************************/
#if UNITY_EDITOR
        /************************************************************************************************************************/

        internal bool HasAnyDynamicCalls()
        {
            return DynamicCallsBase != null;
        }

        /************************************************************************************************************************/

        private Delegate[] _DynamicCallInvocationList;

        internal Delegate[] GetDynamicCallInvocationList()
        {
            if (_DynamicCallInvocationList == null && DynamicCallsBase != null)
                _DynamicCallInvocationList = DynamicCallsBase.GetInvocationList();

            return _DynamicCallInvocationList;
        }

        internal int GetDynamicCallInvocationListCount()
        {
            if (DynamicCallsBase == null)
                return 0;
            return GetDynamicCallInvocationList().Length;
        }

        /************************************************************************************************************************/
#endif
        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Operators and Call Registration
        /************************************************************************************************************************/

        /// <summary>Ensures that 'e' isn't null and adds 'method' to its <see cref="PersistentCallsList"/>.</summary>
        public static PersistentCall AddPersistentCall<T>(ref T e, Delegate method) where T : UltEventBase, new()
        {
            if (e == null)
                e = new T();

            return e.AddPersistentCall(method);
        }

        /// <summary>Ensures that 'e' isn't null and adds 'method' to its <see cref="PersistentCallsList"/>.</summary>
        public static PersistentCall AddPersistentCall<T>(ref T e, Action method) where T : UltEventBase, new()
        {
            if (e == null)
                e = new T();

            return e.AddPersistentCall(method);
        }

        /************************************************************************************************************************/

        /// <summary>If 'e' isn't null, this method removes 'method' from its <see cref="PersistentCallsList"/>.</summary>
        public static void RemovePersistentCall(ref UltEventBase e, Delegate method)
        {
            if (e != null)
                e.RemovePersistentCall(method);
        }

        /// <summary>If 'e' isn't null, this method removes 'method' from its <see cref="PersistentCallsList"/>.</summary>
        public static void RemovePersistentCall(ref UltEventBase e, Action method)
        {
            if (e != null)
                e.RemovePersistentCall(method);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Add the specified 'method to the persistent call list.
        /// </summary>
        public PersistentCall AddPersistentCall(Delegate method)
        { 
            if (_PersistentCalls == null)
                _PersistentCalls = new List<PersistentCall>(4);

            PersistentCall call = new PersistentCall(method);
            call.EventReference = this;
            _PersistentCalls.Add(call);
            return call;
        }

        /// <summary>
        /// Remove the specified 'method from the persistent call list.
        /// </summary>
        public void RemovePersistentCall(Delegate method)
        {
            if (_PersistentCalls == null)
                return;

            for (int i = 0; i < _PersistentCalls.Count; i++)
            {
                PersistentCall call = _PersistentCalls[i];
                if (call.GetMethodSafe() == method.Method && ReferenceEquals(call.Target, method.Target))
                {
                    _PersistentCalls.RemoveAt(i);
                    return;
                }
            }
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/

        /// <summary>Invokes all <see cref="PersistentCallsList"/> registered to this event.</summary>
        protected async void InvokePersistentCalls()
        {
            int currentInvocationIndex = _invocationIndex;
            _invocationIndex++;

            try
            {
                if (_PersistentCalls != null)
                {
                    for (int i = 0; i < _PersistentCalls.Count; i++)
                    {
                        if(Application.isPlaying)
                        {
                            try
                            {
                                await Await.Seconds(_PersistentCalls[i].MethodDelay).ConfigureAwait(CancellationToken);
                            }
                            catch(OperationCanceledException)
                            {
                                LinkedValueDictionary[currentInvocationIndex] = null;
                                LinkedValueDictionary.Remove(currentInvocationIndex);
                                ReturnValueIndices.Remove(currentInvocationIndex);
                                
                                return;
                            }
                        }
                        else
                        {
                            CancellationToken.ThrowIfCancellationRequested();
                            await Task.Delay((int)(_PersistentCalls[i].MethodDelay * 1000f), CancellationToken);
                        }

                        _PersistentCalls[i].InvocationIndex = currentInvocationIndex;
                        _PersistentCalls[i].EventReference = this;
                        
                        object result = _PersistentCalls[i].Invoke();

                        if(!LinkedValueDictionary.ContainsKey(currentInvocationIndex))
                        {
                            LinkedValueDictionary.Add(currentInvocationIndex, new List<object>());
                        }

                        LinkedValueDictionary[currentInvocationIndex].Add(result);
                    }
                }
            }
            finally
            {
                LinkedValueDictionary[currentInvocationIndex] = null;
                LinkedValueDictionary.Remove(currentInvocationIndex);
                ReturnValueIndices.Remove(currentInvocationIndex);
            }
        }

        /************************************************************************************************************************/
        #region Linked Value Cache (Parameters and Returned Values)
        /************************************************************************************************************************/

        private Dictionary<int, List<object>> LinkedValueDictionary = new Dictionary<int, List<object>>();
        private Dictionary<int, int> ReturnValueIndices = new Dictionary<int, int>();
        private int _invocationIndex;

        public void CacheParameter(object value)
        {
            if(!LinkedValueDictionary.ContainsKey(_invocationIndex))
            {
                LinkedValueDictionary.Add(_invocationIndex, new List<object>());
            }
            
            LinkedValueDictionary[_invocationIndex].Add(value);

            if(!ReturnValueIndices.ContainsKey(_invocationIndex))
            {
                ReturnValueIndices.Add(_invocationIndex, LinkedValueDictionary[_invocationIndex].Count);
            }
            else
            {
                ReturnValueIndices[_invocationIndex] = LinkedValueDictionary[_invocationIndex].Count;
            }
        }

        public object GetParameter(int index, int invocationIndex)
        {
            return LinkedValueDictionary[invocationIndex][index];
        }

        public object GetReturnValue(int index, int invocationIndex)
        {
            if(ReturnValueIndices.Count == 0)
            {
                return LinkedValueDictionary[invocationIndex][index];
            }

            return LinkedValueDictionary[invocationIndex][ReturnValueIndices[invocationIndex] + index];
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Parameter Display
#if UNITY_EDITOR
        /************************************************************************************************************************/

        /// <summary>The type of each of this event's parameters.</summary>
        public abstract Type[] ParameterTypes { get; }

        /************************************************************************************************************************/

        private static readonly Dictionary<Type, ParameterInfo[]>
            EventTypeToParameters = new Dictionary<Type, ParameterInfo[]>();

        internal ParameterInfo[] Parameters
        {
            get
            {
                Type type = GetType();

                ParameterInfo[] parameters;
                if (!EventTypeToParameters.TryGetValue(type, out parameters))
                {
                    MethodInfo invokeMethod = type.GetMethod("Invoke", ParameterTypes);
                    if (invokeMethod == null || invokeMethod.DeclaringType == typeof(UltEvent) ||
                        invokeMethod.DeclaringType.Name.StartsWith(Names.UltEvent.Class + "`"))
                    {
                        parameters = null;
                    }
                    else
                    {
                        parameters = invokeMethod.GetParameters();
                    }

                    EventTypeToParameters.Add(type, parameters);
                }

                return parameters;
            }
        }

        /************************************************************************************************************************/

        private static readonly Dictionary<Type, string>
            EventTypeToParameterString = new Dictionary<Type, string>();

        internal string ParameterString
        {
            get
            {
                Type type = GetType();

                string parameters;
                if (!EventTypeToParameterString.TryGetValue(type, out parameters))
                {
                    if (ParameterTypes.Length == 0)
                    {
                        parameters = " ()";
                    }
                    else
                    {
                        ParameterInfo[] invokeMethodParameters = Parameters;

                        StringBuilder text = new StringBuilder();

                        text.Append(" (");
                        for (int i = 0; i < ParameterTypes.Length; i++)
                        {
                            if (i > 0)
                                text.Append(", ");

                            text.Append(ParameterTypes[i].GetNameCS(false));

                            if (invokeMethodParameters != null)
                            {
                                text.Append(" ");
                                text.Append(invokeMethodParameters[i].Name);
                            }
                        }
                        text.Append(")");

                        parameters = text.ToString();
                    }

                    EventTypeToParameterString.Add(type, parameters);
                }

                return parameters;
            }
        }

        /************************************************************************************************************************/
#endif
        #endregion
        /************************************************************************************************************************/

        /// <summary>
        /// Clears all <see cref="PersistentCallsList"/> and <see cref="DynamicCallsBase"/> registered to this event.
        /// </summary>
        public void Clear()
        {
            if (_PersistentCalls != null)
                _PersistentCalls.Clear();

            DynamicCallsBase = null;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Returns true if this event has any <see cref="PersistentCallsList"/> or <see cref="DynamicCallsBase"/> registered.
        /// </summary>
        public bool HasCalls
        {
            get
            {
                return (_PersistentCalls != null && _PersistentCalls.Count > 0) || DynamicCallsBase != null;
            }
        }

        /************************************************************************************************************************/

        /// <summary>Copies the contents of this the 'target' event to this event.</summary>
        public virtual void CopyFrom(UltEventBase target)
        {
            if (target._PersistentCalls == null)
            {
                _PersistentCalls = null;
            }
            else
            {
                if (_PersistentCalls == null)
                    _PersistentCalls = new List<PersistentCall>();
                else
                    _PersistentCalls.Clear();

                for (int i = 0; i < target._PersistentCalls.Count; i++)
                {
                    PersistentCall call = new PersistentCall();
                    call.CopyFrom(target._PersistentCalls[i]);
                    _PersistentCalls.Add(call);
                }
            }

            DynamicCallsBase = target.DynamicCallsBase;

#if UNITY_EDITOR
            _DynamicCallInvocationList = target._DynamicCallInvocationList;
#endif
        }

        /************************************************************************************************************************/

        /// <summary>Returns a description of this event.</summary>
        public override string ToString()
        {
            StringBuilder text = new StringBuilder();
            ToString(text);
            return text.ToString();
        }

        /// <summary>Appends a description of this event.</summary>
        public void ToString(StringBuilder text)
        {
            text.Append(GetType().GetNameCS());

            text.Append(": PersistentCalls=");
            UltEventUtils.AppendDeepToString(text, _PersistentCalls.GetEnumerator(), "\n    ");

            text.Append("\n    DynamicCalls=");
#if UNITY_EDITOR
            Delegate[] invocationList = GetDynamicCallInvocationList();
#else
            var invocationList = DynamicCallsBase != null ? DynamicCallsBase.GetInvocationList() : null;
#endif
            IEnumerator enumerator = invocationList != null ? invocationList.GetEnumerator() : null;
            UltEventUtils.AppendDeepToString(text, enumerator, "\n    ");
        }

        /************************************************************************************************************************/
    }
}