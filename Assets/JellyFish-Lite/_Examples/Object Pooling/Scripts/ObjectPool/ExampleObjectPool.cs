// Created by Kearan Petersen : https://www.blumalice.wordpress.com | https://www.linkedin.com/in/kearan-petersen/

using SOFlow.ObjectPooling;
using UnityEngine;

namespace Examples.ObjectPool
{
    namespace SOFlow.Examples
    {
        [CreateAssetMenu(menuName = "JellyFish/Examples/Example Object Pool")]
        public class ExampleObjectPool : ObjectPool<ExamplePoolObject>
        {
        }
    }
}