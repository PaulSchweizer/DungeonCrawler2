﻿using System;
using System.Collections.Generic;

namespace ScriptableAttribute
{
    [Serializable]
    public class FloatReference
    {
        public bool UseConstant = true;
        public float ConstantValue;
        public float ConstantMinValue;
        public float ConstantMaxValue;
        public FloatVariable Variable;

        public FloatReference()
        { }

        public FloatReference(float value)
        {
            UseConstant = true;
            ConstantValue = value;
        }

        public float Value
        {
            get { return UseConstant ? ConstantValue : Variable.Value; }
        }

        public float MinValue
        {
            get { return UseConstant ? ConstantMinValue : Variable.MinValue; }
        }

        public float MaxValue
        {
            get { return UseConstant ? ConstantMaxValue : Variable.MaxValue; }
        }

        public void ApplyChange(float amount)
        {
            if (UseConstant)
            {
                ConstantValue += amount;
            }
            else
            {
                Variable.ApplyChange(amount);
            }
        }

        public static implicit operator float(FloatReference reference)
        {
            return reference.Value;
        }

        public void DeserializeFromData(Dictionary<string, object> data)
        {
            if (UseConstant)
            {
                Variable.DeserializeFromData(data);
            }
            else
            {
                ConstantValue = Convert.ToInt32(data["Value"]);
                ConstantMaxValue = Convert.ToInt32(data["MaxValue"]);
                ConstantMinValue = Convert.ToInt32(data["MinValue"]);
            }
        }
    }
}