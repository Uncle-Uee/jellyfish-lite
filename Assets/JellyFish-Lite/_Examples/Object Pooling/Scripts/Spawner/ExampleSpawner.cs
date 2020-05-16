using Examples.ObjectPool.SOFlow.Examples;
using UnityEngine;

namespace Examples.ObjectPool
{
    public class ExampleSpawner : MonoBehaviour
    {
        #region VARIABLES

        [Header("Object Pool Reference")]
        public ExampleObjectPool ObjectPool;

        private ExamplePoolObject _poolObject;

        #endregion

        #region METHODS

        public void SpawnObject(string id)
        {
            _poolObject                    = ObjectPool.GetObjectFromPool(id, false);
            _poolObject.transform.position = transform.position;
            _poolObject.gameObject.SetActive(true);
        }

        #endregion
    }
}