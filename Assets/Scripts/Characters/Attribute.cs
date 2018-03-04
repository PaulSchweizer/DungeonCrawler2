using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class Attribute
{
    public int Value;
    public int MaxValue;
    public int MinValue;

    public Attribute(int value, int maxValue, int minValue)
    {
        Value = value;
        MaxValue = maxValue;
        MinValue = minValue;
    }

    public void DeserializeFromData(Dictionary<string, object> data)
    {
        Value = Convert.ToInt32(data["Value"]);
        MaxValue = Convert.ToInt32(data["MaxValue"]);
        MinValue = Convert.ToInt32(data["MinValue"]);
    }
}
