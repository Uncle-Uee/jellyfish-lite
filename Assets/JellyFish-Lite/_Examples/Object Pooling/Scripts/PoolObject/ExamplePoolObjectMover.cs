using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Examples.ObjectPool
{
    public class ExamplePoolObjectMover : MonoBehaviour
    {
        #region VARIABLES

        [Header("Events")]
        public UnityEvent OnLifeExpired = new UnityEvent();

        private Vector3 _position;
        private float   _lifeTime = 0f;

        #endregion

        #region UNITY METHODS

        private void Update()
        {
            _position          =  transform.position;
            _position          += Vector3.right * (3f * Time.deltaTime);
            transform.position =  _position;

            _lifeTime += Time.deltaTime;
            if (_lifeTime >= 1f)
            {
                OnLifeExpired.Invoke();
            }
        }

        private void OnDisable()
        {
            _lifeTime = 0f;
        }

        #endregion
    }
}