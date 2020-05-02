using System;
using UnityEngine;

namespace Data.Primitive
{
    [Serializable]
    public class Vector2Field
    {
        /// <summary>
        ///     Determines whether this field references volatile or non-volatile data.
        /// </summary>
        [HideInInspector]
        public bool UseConstant = true;

        /// <summary>
        ///     The explicitly inferred type for the ConstantValue property.
        /// </summary>
        [HideInInspector]
        public Vector2 ConstantValueType;

        /// <summary>
        ///     The explicitly inferred type for the Variable property.
        /// </summary>
        [HideInInspector]
        public Vector2Data VariableType;

        /// <summary>
        ///     The value of this field.
        /// </summary>
        public Vector2 Value
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
        public Vector2 ConstantValue
        {
            get => ConstantValueType;
            set => ConstantValueType = value;
        }

        /// <summary>
        ///     The non-volatile data for this field.
        /// </summary>
        public Vector2Data Variable
        {
            get => VariableType;
            set => VariableType = value;
        }

        public Vector2Field()
        {
        }

        public Vector2Field(Vector2 value)
        {
            Value = value;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Value.x},{Value.y}";
        }

        /// <summary>
        ///     Implicit conversion to the corresponding data type.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static implicit operator Vector2(Vector2Field data)
        {
            return data.Value;
        }

        /// <summary>
        ///     Gets the variable data.
        /// </summary>
        /// <returns></returns>
        public PrimitiveData GetVariable()
        {
            return Variable;
        }
    }
}