using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PlayerUI : GameEventListener
{
    [Header("HUD")]
    public GameObject MenuPanel;

    protected void Awake()
    {
        MenuPanel.SetActive(false);
    }

    public void ToggleUI()
    {
        MenuPanel.SetActive(!MenuPanel.activeSelf);
    }
}
