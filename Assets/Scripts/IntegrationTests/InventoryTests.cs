using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class InventoryTests
{

    public Inventory InventoryA;
    public Inventory InventoryB;
    public Item Item;

    [SetUp]
    public void SetUp()
    {
        InventoryA = new Inventory();
        InventoryB = new Inventory();

        Item = ScriptableObject.CreateInstance<Item>();
        Item.Id = "0";
        Item.Name = "TestItem";
    }

    [Test]
	public void Add_and_remove_items()
    {
        InventoryA.AddItem(Item, 10);

        Assert.AreEqual(1, InventoryA.Items.Count);
        Assert.AreEqual(10, InventoryA.Amount(Item));

        InventoryA.RemoveItem(Item);
        Assert.AreEqual(9, InventoryA.Amount(Item));

        InventoryA.RemoveItem(Item, 10);
        Assert.AreEqual(0, InventoryA.Amount(Item));
    }

    [Test]
    public void Add_two_Inventories()
    {
        InventoryA.AddItem(Item, 10);

        InventoryB += InventoryA;

        Assert.AreEqual(1, InventoryB.Items.Count);
        Assert.AreEqual(10, InventoryB.Amount(Item));
        Assert.AreEqual(0, InventoryA.Items.Count);
    }
}
