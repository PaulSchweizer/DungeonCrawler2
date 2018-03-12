using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SlotSystem;

public class PlayerUI : MonoBehaviour
{

    [Header("HUD")]
    public GameObject MenuPanel;
    public RectTransform PortraitsPanel;

    [Header("Main Menu")]
    public GameObject CharacterButton;
    public GameObject SkillsButton;
    public GameObject InventoryButton;
    public GameObject QuestsButton;
    public InventoryUI InventoryView;

    [Header("Prefabs")]
    public RectTransform CharacterPortrait;

    protected void Awake()
    {
        MenuPanel.SetActive(false);
    }

    #region Actions

    public void ToggleMenu()
    {
        MenuPanel.SetActive(!MenuPanel.activeSelf);
    }

    #endregion

    #region Updates

    #endregion

    #region Events

    #endregion
}
