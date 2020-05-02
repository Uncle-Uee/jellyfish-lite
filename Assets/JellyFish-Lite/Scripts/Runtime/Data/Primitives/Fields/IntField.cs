using System;
using UnityEngine;

namespace Data.Primitive
{
    [Serializable]
    public class IntField
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
        public int ConstantValueType;

        /// <summary>
        ///     The explicitly inferred type for the Variable property.
        /// </summary>
        [HideInInspector]
        public IntData VariableType;

        /// <summary>
        ///     The value of this field.
        /// </summary>
        public int Value
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
        public int ConstantValue
        {
            get => ConstantValueType;
            set => ConstantValueType = value;
        }

        /// <summary>
        ///     The non-volatile data for this field.
        /// </summary>
        public IntData Variable
        {
            get => VariableType;
            set => VariableType = value;
        }

        public IntField()
        {
        }

        public IntField(int value)
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
        public static implicit operator int(IntField data)
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