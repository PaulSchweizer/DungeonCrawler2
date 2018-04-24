using UnityEngine;

public class LevelController : MonoBehaviour
{
    [Header("Prefabs to load")]
    public PlayerCharacter PlayerPrefab;
    public CameraController CameraRigPrefab;

    [Header("UIs to load")]
    public GameObject MenuBarPrefab;
    public LevelPortalUI LevelPortalUIPrefab;
    public PlayerUI PlayerUIPrefab;
    public MarkedEnemyUI MarkedEnemyUIPrefab;

    [Header("Events")]
    public StringGameEvent OnSceneReady;

    public void Initialize(string previousSceneName)
    {
        // Player
        PlayerCharacter player = FindObjectOfType<PlayerCharacter>();
        if (player == null) player = Instantiate(PlayerPrefab);

        // Put the Player to the correct SpawnPoint
        foreach (LevelPortal portal in FindObjectsOfType<LevelPortal>())
        {
            if (previousSceneName == portal.Destination)
            {
                player.transform.position = portal.SpawnPoint.position;
                break;
            }
        }

        // Camera
        CameraController cameraRig = FindObjectOfType<CameraController>();
        if (cameraRig == null) cameraRig = Instantiate(CameraRigPrefab);
        cameraRig.Target = player.transform;

        // UIs
        if (GameObject.Find("MenuBar") == null) Instantiate(MenuBarPrefab);
        if (FindObjectOfType<PlayerUI>() == null) Instantiate(PlayerUIPrefab);
        if (FindObjectOfType<LevelPortalUI>() == null) Instantiate(LevelPortalUIPrefab);
        if (FindObjectOfType<MarkedEnemyUI>() == null) Instantiate(MarkedEnemyUIPrefab);

        OnSceneReady.Raise("");
    }
}
