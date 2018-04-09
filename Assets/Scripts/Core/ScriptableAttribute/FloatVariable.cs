using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableAttribute
{
    [CreateAssetMenu]
    public class FloatVariable : ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        public string Description = "";
#endif
        public float Value;
        public float MinValue;
        public float MaxValue;

        public GameEvent OnChanged;

        public void SetValue(float value)
        {
            Value = value;
            if (OnChanged != null) OnChanged.Raise();
        }

        public void SetValue(FloatVariable value)
        {
            Value = value.Value;
            if (OnChanged != null) OnChanged.Raise();
        }

        public void ApplyChange(float amount)
        {
            Value += amount;
            if (OnChanged != null) OnChanged.Raise();
        }

        public void ApplyChange(FloatVariable amount)
        {
            Value += amount.Value;
            if (OnChanged != null) OnChanged.Raise();
        }

        public void DeserializeFromData(Dictionary<string, object> data)
        {
            Value = Convert.ToInt32(data["Value"]);
            MaxValue = Convert.ToInt32(data["MaxValue"]);
            MinValue = Convert.ToInt32(data["MinValue"]);
        }
    }
}