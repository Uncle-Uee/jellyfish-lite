// Created by Kearan Petersen : https://www.blumalice.wordpress.com | https://www.linkedin.com/in/kearan-petersen/

using System;
using JellyFish.Data.Primitives;
using UnityEngine;

namespace JellyFish.Data.Primitive
{
    [Serializable]
    public class BoolField : DataField
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
        public bool ConstantValueType;

        /// <summary>
        ///     The explicitly inferred type for the Variable property.
        /// </summary>
        [HideInInspector]
        public BoolData VariableType;

        /// <summary>
        ///     The value of this field.
        /// </summary>
        public bool Value
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
        public bool ConstantValue
        {
            get => ConstantValueType;
            set => ConstantValueType = value;
        }

        /// <summary>
        ///     The non-volatile data for this field.
        /// </summary>
        public BoolData Variable
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

        public BoolField()
        {
        }

        public BoolField(bool value)
        {
            Value = value;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Value.ToString();
        }

        /// <summary>
        ///     Implicit conversion to the corresponding data type.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static implicit operator bool(BoolField data)
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