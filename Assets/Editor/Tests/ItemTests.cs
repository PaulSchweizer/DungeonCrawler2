using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class ItemSerializationTests
{
    [Test]
    public void JsonSerialization()
    {
        string json = TestUtilities.JsonResourceFromFile("Item");

        Item deserializedItem = ScriptableObject.CreateInstance<Item>();
        JsonConvert.PopulateObject(json, deserializedItem);

        Item fromSerialized  = ScriptableObject.CreateInstance<Item>();
        json = SerializationUtilitites.SerializeToJson(deserializedItem);
        JsonConvert.PopulateObject(json, fromSerialized);

        Assert.AreEqual(deserializedItem.Id, fromSerialized.Id);
        Assert.AreEqual(deserializedItem.Name, fromSerialized.Name);
    }
}