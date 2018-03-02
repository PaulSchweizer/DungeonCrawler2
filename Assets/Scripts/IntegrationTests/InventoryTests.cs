using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;

public class InventoryTests
{

    public Inventory InventoryA;
    public Inventory InventoryB;
    public Item Item;

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

        Item = ScriptableObject.CreateInstance<Item>();
        Item.Id = "0";
        Item.Name = "TestItem";
    }

    [Test]
	public void Add_and_remove_items()
    {
        Debug.Log(InventoryA.Items.Count);
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

    [Test]
    public void Serialize_Inventory()
    {
        InventoryA.AddItem(Item, 10);
        string json = JsonUtility.ToJson(InventoryA);

        Debug.Log(json);
    }
}
