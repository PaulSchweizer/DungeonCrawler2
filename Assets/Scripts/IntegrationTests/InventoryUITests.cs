using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[TestFixture]
public class InventoryUITests
{
    public Item TestItem;
    public InventoryUI InventoryUI;
    public ItemDatabase Database;
    public GameEventsLogger GameEventsLogger;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        SceneManager.LoadScene("Scripts/IntegrationTests/TestScenes/InventoryUITests");
    }

    [SetUp]
    public void SetUp()
    {
        TestItem = ScriptableObject.CreateInstance<Item>();
        TestItem.Name = "Coin";
        TestItem.Id = "0";
        Database = ScriptableObject.CreateInstance<ItemDatabase>();
        Database.Items = new Item[] { TestItem };

        InventoryUI = GameObject.Find("InventoryUI").GetComponent<InventoryUI>();
        Inventory inventory = ScriptableObject.CreateInstance<Inventory>();
        inventory.ItemDatabase = Database;
        GameEventsLogger = ScriptableObject.CreateInstance<GameEventsLogger>();
        inventory.GameEventsLogger = GameEventsLogger;
        InventoryUI.Inventory = inventory;
        InventoryUI.Initialize();
    }

    [Test]
    public void Add_and_remove_items()
    {
        for(int i = 0; i < 10; i++)
        {
            InventoryUI.Inventory.AddItem(TestItem, 100);
            Assert.AreEqual(1, InventoryUI.SlotView.Items.Count, "Expected 1 SlottableItem");
            Assert.AreEqual("100", InventoryUI.SlotView.Items[TestItem.Name].AmountText.text);

            InventoryUI.Inventory.RemoveItem(TestItem, 99);
            Assert.AreEqual(1, InventoryUI.SlotView.Items.Count, "Removed 99/100 Items, Expected to still have 1 SlottaleItem");
            Assert.AreEqual("1", InventoryUI.SlotView.Items[TestItem.Name].AmountText.text);

            InventoryUI.Inventory.RemoveItem(TestItem, 1);
            Assert.AreEqual(0, InventoryUI.SlotView.Items.Count, "Expected 0 SlottableItems");
        }
    }
}
