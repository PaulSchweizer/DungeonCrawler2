using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Components")]
    public UnityEngine.AI.NavMeshAgent NavMeshAgent;

    [Header("Data")]
    public Stats Stats;
    public Inventory Inventory;

    public void Awake()
    {
        NavMeshAgent.radius = Stats.Radius;
    }

    public Dictionary<string, object> SerializeToData()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data["Stats"] = Stats.SerializeToData();
        data["Inventory"] = Inventory.SerializeToData();
        data["Transform"] = new float[] { gameObject.transform.position.x,
                                          gameObject.transform.position.y,
                                          gameObject.transform.position.z };
        return data;
    }

    public void DeserializeFromJson(string json)
    {
        Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        Stats.DeserializeFromData(SerializationUtilitites.DeserializeFromObject(data["Stats"]));
        Inventory.DeserializeFromData(SerializationUtilitites.DeserializeFromObject(data["Inventory"]));

        float[] transformData = JsonConvert.DeserializeObject<float[]>(JsonConvert.SerializeObject(data["Transform"]));
        transform.position = new Vector3(transformData[0], transformData[1], transformData[2]);
    }
}

