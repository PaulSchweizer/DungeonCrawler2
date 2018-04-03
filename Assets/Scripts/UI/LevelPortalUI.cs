using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LevelPortalUI : MonoBehaviour
{
    public GameObject Canvas;
    public Text DestinationText;
    public Button OkButton;
    public Button CancelButton;
    public StringGameEvent OnLevelSwitch;

    private string Destination;

    private void Awake()
    {
        Canvas.SetActive(false);
    }

    public void Show(string destination)
    {
        Canvas.SetActive(true);
        Destination = destination;
        DestinationText.text = string.Format("Do you want to travel to {0}?", destination);
    }

    public void SwitchLocation()
    {
        OnLevelSwitch.Raise(Destination);
    }

    public void Close()
    {
        Canvas.SetActive(false);
    }
}

