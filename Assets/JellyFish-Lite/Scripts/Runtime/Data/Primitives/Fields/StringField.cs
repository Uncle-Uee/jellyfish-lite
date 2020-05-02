// Created by Kearan Petersen : https://www.blumalice.wordpress.com | https://www.linkedin.com/in/kearan-petersen/

using System;
using UnityEngine;

namespace Data.Primitive
{
    [Serializable]
    public class StringField
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
        public string ConstantValueType = "";

        /// <summary>
        ///     The explicitly inferred type for the Variable property.
        /// </summary>
        [HideInInspector]
        public StringData VariableType;

        /// <summary>
        ///     The value of this field.
        /// </summary>
        public string Value
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
        public string ConstantValue
        {
            get => ConstantValueType;
            set => ConstantValueType = value;
        }

        /// <summary>
        ///     The non-volatile data for this field.
        /// </summary>
        public StringData Variable
        {
            get => VariableType;
            set => VariableType = value;
        }

        public StringField()
        {
        }

        public StringField(string value)
        {
            Value = value;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Value != null ? Value : base.ToString();
        }

        /// <summary>
        ///     Implicit conversion to the corresponding data type.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static implicit operator string(StringField data)
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