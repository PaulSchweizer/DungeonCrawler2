﻿using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;

public class InventoryTests
{

    public Inventory InventoryA;
    public Inventory InventoryB;
    public Item TestItem;
    public ItemDatabase Database;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        SceneManager.LoadScene("Scripts/IntegrationTests/TestScenes/InventoryTests");
    }
    [SetUp]
    public void SetUp()
    {
        InventoryA = GameObject.Find("InventoryA").GetComponent<Inventory>();
        InventoryB = GameObject.Find("InventoryB").GetComponent<Inventory>();
        Database = InventoryA.ItemDatabase;
        TestItem = Database.ItemByName<Item>("Coin");
        InventoryA.Clear();
        InventoryB.Clear();

        Database.ItemByName<Weapon>("Sword");
    }

    [Test]
	public void Add_and_remove_items()
    {
        InventoryA.AddItem(TestItem, 10);

        Assert.AreEqual(1, InventoryA.Items.Count);
        Assert.AreEqual(10, InventoryA.Amount(TestItem));

        InventoryA.RemoveItem(TestItem);
        Assert.AreEqual(9, InventoryA.Amount(TestItem));

        InventoryA.RemoveItem(TestItem, 10);
        Assert.AreEqual(0, InventoryA.Amount(TestItem));
    }

    [Test]
    public void Add_two_Inventories()
    {
        InventoryA.AddItem(TestItem, 10);

        InventoryB += InventoryA;

        Assert.AreEqual(1, InventoryB.Items.Count);
        Assert.AreEqual(10, InventoryB.Amount(TestItem));
        Assert.AreEqual(0, InventoryA.Items.Count);
    }

    [Test]
    public void Access_Items_from_ItemDatabase_by_Name_or_Id()
    {
        Item Coin = Database.ItemByName<Item>("Coin");
        Assert.AreEqual("Coin", Coin.Name);
        Assert.AreEqual(Coin, Database.ItemByIdentifier<Item>(Coin.Identifier));
    }

    [Test]
    public void Serialize_Inventory()
    {
        InventoryA.AddItem(TestItem, 10);
        string json = InventoryA.SerializeToJson();
        InventoryB.DeserializeFromJson(json);

        Assert.AreEqual(InventoryA.Items.Count, InventoryB.Items.Count);
        for (int i = 0; i < InventoryA.Items.Count; i++)
        {
            Assert.AreEqual(InventoryA.Items[i].Amount, InventoryB.Items[i].Amount);
            Assert.AreSame(InventoryA.Items[i].Item, InventoryB.Items[i].Item);
            Assert.AreSame(Database.ItemByIdentifier<Item>(InventoryA.Items[i].Item.Identifier), InventoryB.Items[i].Item);
        }
    }
}
