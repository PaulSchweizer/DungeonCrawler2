using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [Header("HUD")]
    public GameObject MenuPanel;
    public GameObject InventoryContent;
    public GameObject StatsContent;

    protected void Awake()
    {
        SwitchContent("Inventory");
        MenuPanel.SetActive(false);
    }

    public void ToggleUI()
    {
        MenuPanel.SetActive(!MenuPanel.activeSelf);
    }

    public void SwitchContent(string content)
    {
        if (content == "Inventory")
        {
            InventoryContent.SetActive(true);
            StatsContent.SetActive(false);
        }
        else if (content == "Skills")
        {
            InventoryContent.SetActive(false);
            StatsContent.SetActive(true);
        }
    }
}
