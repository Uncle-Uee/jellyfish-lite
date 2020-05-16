// Created by Kearan Petersen : https://www.blumalice.wordpress.com | https://www.linkedin.com/in/kearan-petersen/

using SOFlow.ObjectPooling;
using UnityEngine;

namespace Examples.ObjectPool
{
    public class ExamplePoolObject : MonoBehaviour, IPoolObject<ExamplePoolObject>
    {
        /// <summary>
        /// The unique ID assigned to this object.
        /// </summary>
        public string UniqueID;

        /// <inheritdoc />
        public string ID => UniqueID;

        /// <inheritdoc />
        public ExamplePoolObject Instantiate(Transform container)
        {
            return Instantiate(this, container);
        }

        /// <inheritdoc />
        public void ActivateObject()
        {
            gameObject.SetActive(true);
        }

        /// <inheritdoc />
        public void DeactivateObject()
        {
            gameObject.SetActive(false);
        }

        /// <inheritdoc />
        public Object GetObjectInstance()
        {
            return this;
        }
    }
}