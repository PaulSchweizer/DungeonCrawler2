using UnityEngine;

public class LevelPortal : MonoBehaviour
{
    public string Destination;
    public StringGameEvent OnLevelSwitchRequested;
    public GameEvent OnLevelSwitchAborted;
    public Transform SpawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnLevelSwitchRequested.Raise(Destination);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnLevelSwitchAborted.Raise();
        }
    }
}