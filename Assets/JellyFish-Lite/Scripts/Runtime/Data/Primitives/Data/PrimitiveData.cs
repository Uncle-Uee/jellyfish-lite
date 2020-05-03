// Created by Kearan Petersen : https://www.blumalice.wordpress.com | https://www.linkedin.com/in/kearan-petersen/

using UnityEngine;

namespace JellyFish.Data.Primitive
{
    public class PrimitiveData : ScriptableObject
    {
        /// <summary>
        /// The developer description for this primitive data.
        /// </summary>
        [Multiline]
        public string Description;

#if UNITY_EDITOR
        public virtual void OnValidate()
        {
            // Resync the Play Mode safe representation with the
            // true asset value during editing.
            ResetValue();
        }
#endif

        /// <summary>
        ///     Returns the value of this data.
        /// </summary>
        /// <returns></returns>
        public virtual object GetValueData()
        {
            return null;
        }

        /// <summary>
        /// Resets the value of this data to its default state.
        /// </summary>
        public virtual void ResetValue()
        {
        }
    }
}