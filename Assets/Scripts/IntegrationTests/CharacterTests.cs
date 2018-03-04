using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.SceneManagement;
using UnityEngine;

public class CharacterTests
{
    public Character PlayerA;
    public Character PlayerB;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        SceneManager.LoadScene("Scripts/IntegrationTests/TestScenes/CharacterTests");
    }

    [SetUp]
    public void SetUp()
    {
        PlayerA = GameObject.Find("PlayerA").GetComponent<Character>();
        PlayerB = GameObject.Find("PlayerB").GetComponent<Character>();
    }
    
    [Test]
    public void SerializePlayer()
    {
        PlayerA.transform.position = new Vector3(1, 2, 3);
        string jsonPlayerA = SerializationUtilitites.SerializeToJson(PlayerA.SerializeToData());

        PlayerB.DeserializeFromJson(jsonPlayerA);
        string jsonPlayerB = SerializationUtilitites.SerializeToJson(PlayerB.SerializeToData());
        Assert.AreEqual(jsonPlayerB, jsonPlayerA);
    }
}
