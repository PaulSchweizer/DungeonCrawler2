using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.SceneManagement;
using UnityEngine;

public class CharacterTests
{
    public PlayerCharacter PlayerA;
    public PlayerCharacter PlayerB;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        SceneManager.LoadScene("Scripts/IntegrationTests/TestScenes/CharacterTests");
    }

    [SetUp]
    public void SetUp()
    {
        PlayerA = GameObject.Find("PlayerA").GetComponent<PlayerCharacter>();
        PlayerB = GameObject.Find("PlayerB").GetComponent<PlayerCharacter>();
    }

    [Test]
    public void SerializePlayer()
    {
        PlayerA.transform.position = new Vector3(1, 2, 3);
        string jsonPlayerA = SerializationUtilitites.SerializeToJson(PlayerA.SerializeToData());

        PlayerB.DeserializeFromJson(jsonPlayerA);
        string jsonPlayerB = SerializationUtilitites.SerializeToJson(PlayerB.SerializeToData());
        Debug.Log(jsonPlayerA);
        Debug.Log(jsonPlayerB);
        Assert.AreEqual(jsonPlayerB, jsonPlayerA);
    }
}
