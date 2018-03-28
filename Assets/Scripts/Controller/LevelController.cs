using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject PlayerPrefab;
    public CameraController CameraRigPrefab;

    [Header("Events")]
    public GameEvent SceneReady;

    public void Start()
    {
        InitializeScene();
    }

    public void InitializeScene()
    {
        GameObject player = Instantiate(PlayerPrefab);

        CameraController cameraRig = FindObjectOfType<CameraController>();
        if (cameraRig == null) cameraRig = Instantiate(CameraRigPrefab);
        cameraRig.Target = player.transform;



        SceneReady.Raise();
    }
}
