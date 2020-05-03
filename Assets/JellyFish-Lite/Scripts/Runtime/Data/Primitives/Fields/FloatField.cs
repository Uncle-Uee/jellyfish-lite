// Created by Kearan Petersen : https://www.blumalice.wordpress.com | https://www.linkedin.com/in/kearan-petersen/

using System;
using System.Globalization;
using JellyFish.Data.Primitives;
using UnityEngine;

namespace JellyFish.Data.Primitive
{
    [Serializable]
    public class FloatField : DataField
    {
        // /// <summary>
        // ///     Determines whether this field references volatile or non-volatile data.
        // /// </summary>
        // [HideInInspector]
        // public bool UseConstant = true;

        /// <summary>
        ///     The explicitly inferred type for the ConstantValue property.
        /// </summary>
        [HideInInspector]
        public float ConstantValueType;

        /// <summary>
        ///     The explicitly inferred type for the Variable property.
        /// </summary>
        [HideInInspector]
        public FloatData VariableType;

        /// <summary>
        ///     The value of this field.
        /// </summary>
        public float Value
        {
            get
            {
                if (UseConstant) return ConstantValue;

                if (Variable == null) return ConstantValue;

                return Variable.Value;
            }
            set
            {
                if (UseConstant)
                {
                    if (!ConstantValue.Equals(value))
                    {
                        ConstantValue = value;
                    }
                }
                else
                {
                    if (Variable != null) Variable.Value = value;
                }
            }
        }

        /// <summary>
        ///     The volatile data for this field.
        /// </summary>
        public float ConstantValue
        {
            get => ConstantValueType;
            set => ConstantValueType = value;
        }

        /// <summary>
        ///     The non-volatile data for this field.
        /// </summary>
        public FloatData Variable
        {
            get => VariableType;
            set => VariableType = value;
        }
        
        /// <summary>
        /// Indicates whether the value changed event should be displayed.
        /// </summary>
#pragma warning disable 0414
        [SerializeField, HideInInspector]
        private bool _displayValueChangedEvent = false;
#pragma warning restore 0414

        public FloatField()
        {
        }

        public FloatField(float value)
        {
            Value = value;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Implicit conversion to the corresponding data type.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static implicit operator float(FloatField data)
        {
            return data.Value;
        }

        /// <summary>
        ///     Gets the variable data.
        /// </summary>
        /// <returns></returns>
        public override PrimitiveData GetVariable()
        {
            return Variable;
        }
    }
}