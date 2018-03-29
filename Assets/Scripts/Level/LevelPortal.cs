using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LevelPortal : MonoBehaviour
{
    public string Destination;

    public GameEvent OnLevelSwitch;

    //public ExitUI UI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //UI.Show(Destination);
            OnLevelSwitch.Raise(Destination);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if (other.CompareTag("Player"))
        //{
        //    UI.Close();
        //}
    }
}