using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[TestFixture]
public class InventoryTests
{

    public Inventory InventoryA;
    public Inventory InventoryB;
    public Item TestItem;
    public ItemDatabase Database;
    public GameEventsLogger GameEventsLogger;

    [SetUp]
    public void SetUp()
    {
        GameEventsLogger = ScriptableObject.CreateInstance<GameEventsLogger>();

        TestItem = ScriptableObject.CreateInstance<Item>();
        TestItem.Name = "Coin";
        TestItem.Id = "0";
        Database = ScriptableObject.CreateInstance<ItemDatabase>();
        Database.Items = new Item[] { TestItem };
        InventoryA = ScriptableObject.CreateInstance<Inventory>();
        InventoryB = ScriptableObject.CreateInstance<Inventory>();
        InventoryA.ItemDatabase = Database;
        InventoryB.ItemDatabase = Database;
        InventoryA.GameEventsLogger = GameEventsLogger;
        InventoryB.GameEventsLogger = GameEventsLogger;
        InventoryA.Clear();
        InventoryB.Clear();
    }

    [Test]
	public void Add_and_remove_items()
    {
        InventoryA.AddItem(TestItem, 10);

        Assert.AreEqual(1, InventoryA.Entries.Count);
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

        Assert.AreEqual(1, InventoryB.Entries.Count);
        Assert.AreEqual(10, InventoryB.Amount(TestItem));
        Assert.AreEqual(0, InventoryA.Entries.Count);
    }

    [Test]
    public void Access_Items_from_ItemDatabase_by_Name_or_Id()
    {
        Assert.AreSame(TestItem, Database.ItemByName<Item>(TestItem.Name));
        Assert.AreSame(TestItem, Database.ItemByIdentifier<Item>(TestItem.Identifier));
    }

    [Test]
    public void Serialize_Inventory()
    {
        InventoryA.AddItem(TestItem, 10);
        string json = SerializationUtilitites.SerializeToJson(InventoryA.SerializeToData());
        InventoryB.DeserializeFromData(SerializationUtilitites.DeserializeFromJson<Dictionary<string, object>>(json));

        Assert.AreEqual(InventoryA.Entries.Count, InventoryB.Entries.Count);
        for (int i = 0; i < InventoryA.Entries.Count; i++)
        {
            Assert.AreEqual(InventoryA.Entries[i].Amount, InventoryB.Entries[i].Amount);
            Assert.AreSame(InventoryA.Entries[i].Item, InventoryB.Entries[i].Item);
            Assert.AreSame(Database.ItemByIdentifier<Item>(InventoryA.Entries[i].Item.Identifier), InventoryB.Entries[i].Item);
        }
    }
}
