using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.SceneManagement;
using UnityEngine;

public class AttackShapeTests
{
    public PlayerCharacter Player;
    public EnemyCharacter EnemyA;
    public EnemyCharacter EnemyB;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        SceneManager.LoadScene("Scripts/IntegrationTests/TestScenes/AttackShapeTests");
    }

    [SetUp]
    public void SetUp()
    {
        Player = GameObject.Find("Player").GetComponent<PlayerCharacter>();
        EnemyA = GameObject.Find("EnemyA").GetComponent<EnemyCharacter>();
        EnemyB = GameObject.Find("EnemyB").GetComponent<EnemyCharacter>();
    }

    [Test]
    public void Enemy_in_AttackShape()
    {
        EnemyA.transform.position = new Vector3(0, 0, -1);
        EnemyB.transform.position = new Vector3(0, 0, -1);
        List<BaseCharacter> enemies = Player.EnemiesInAttackShape();
        Assert.AreEqual(0, enemies.Count);

        // Move one of them into the radius
        EnemyA.transform.position = new Vector3(0, 0, 1);
        EnemyB.transform.position = new Vector3(0, 0, -1);
        enemies = Player.EnemiesInAttackShape();
        Assert.AreEqual(1, enemies.Count);

        // Move the Player
        Player.transform.position = new Vector3(0, 0, 10);
        Player.transform.Rotate(Vector3.up, 90);
        enemies = Player.EnemiesInAttackShape();
        Assert.AreEqual(0, enemies.Count);

        // Move the Enemies
        EnemyA.transform.position = new Vector3(1, 0, 10);
        enemies = Player.EnemiesInAttackShape();
        Assert.AreEqual(1, enemies.Count);
    }
}