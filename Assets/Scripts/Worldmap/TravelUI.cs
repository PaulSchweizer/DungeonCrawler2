using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TravelUI : MonoBehaviour
{
    public GameObject Canvas;
    public Text DestinationText;
    public Button OkButton;
    public Button CancelButton;
    public StringGameEvent OnStartTravel;
    public StringGameEvent OnLevelSwitch;

    private string Destination;

    private void Awake()
    {
        Canvas.SetActive(false);
    }

    public void ShowStartTravel(string destination)
    {
        Canvas.SetActive(true);
        Destination = destination;
        DestinationText.text = string.Format("Do you want to travel to {0}?", destination);
        OkButton.onClick.RemoveAllListeners();
        OkButton.onClick.AddListener(StartTravel);
    }

    public void ShowLevelSwitch(string destination)
    {
        Canvas.SetActive(true);
        Destination = destination;
        DestinationText.text = string.Format("Do you want to enter {0}?", destination);
        OkButton.onClick.RemoveAllListeners();
        OkButton.onClick.AddListener(SwitchLevel);
    }

    public void StartTravel()
    {
        OnStartTravel.Raise(Destination);
        Close();
    }

    public void SwitchLevel()
    {
        OnLevelSwitch.Raise(Destination);
        Close();
    }

    public void Close()
    {
        Canvas.SetActive(false);
    }
}

