﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("Game Data")]
    public string SavedGamesPath = "SavedGames";

    [Header("Fading Screen")]
    public Canvas FadingCanvas;
    public Image FadingScreen;
    public float FadingSpeed = 3;
    public Text LoadingText;

    [Header("Prefabs")]
    public TextAsset DefaultGame;

    [Header("Internal Data")]
    // public string CurrentSceneName;
    public string NextSceneName;

    [Header("Events")]
    public GameEvent SceneLoaded;

    // Internals
    private string _rootDataPath;
    private string _gameToLoad;
    //private string _locationToLoad;

    private AsyncOperation resourceUnloadTask;
    private AsyncOperation sceneLoadTask;
    private enum SceneState { FadeOut, Preload, Load, Unload, Run, FadeIn, Count };
    private SceneState sceneState;
    private delegate void UpdateDelegate();
    private UpdateDelegate[] updateDelegates;

    //private enum LoadAction { StartGame, LoadLocation, LoadWorldmap, Count };
    //private delegate void LoadActionDelegate();
    //private LoadActionDelegate[] loadActionDelegates;

    private void Awake()
    {
        // Set internals
        _rootDataPath = Path.Combine(Application.persistentDataPath, SavedGamesPath);

        // Delegates
        DontDestroyOnLoad(transform.gameObject);
        updateDelegates = new UpdateDelegate[(int)SceneState.Count];
        updateDelegates[(int)SceneState.FadeOut] = UpdateSceneFadeOut;
        updateDelegates[(int)SceneState.Preload] = UpdateScenePreload;
        updateDelegates[(int)SceneState.Load] = UpdateSceneLoad;
        updateDelegates[(int)SceneState.Unload] = UpdateSceneUnload;
        updateDelegates[(int)SceneState.FadeIn] = UpdateSceneFadeIn;
        updateDelegates[(int)SceneState.Run] = UpdateSceneRun;
        sceneState = SceneState.Run;

        //loadActionDelegates = new LoadActionDelegate[(int)LoadAction.Count];
        //loadActionDelegates[(int)LoadAction.StartGame] = StartGame;
        ////loadActionDelegates[(int)LoadAction.LoadLocation] = LoadLocation;
        ////loadActionDelegates[(int)LoadAction.LoadWorldmap] = LoadWorldmap;
        //_loadAction = LoadAction.StartGame;
    }

    #region Actions

    public void NewGame()
    {
        Dictionary<string, string> data = SerializationUtilitites.DeserializeFromJson<Dictionary<string, string>>(DefaultGame.text);
        NextSceneName = data["Location"];
        InitializeGame(DefaultGame.text);
    }

    //public void LoadGame(string name)
    //{
    //    NextSceneName = "LevelTemplate";

    //    string root = Path.Combine(Path.Combine(Application.persistentDataPath, SavedGamesPath), name);
    //    JsonSavedGame savedGame = Instantiate(DefaultGame);
    //    GameState gameState = GameState.DeserializeFromJson(File.ReadAllText(Path.Combine(root, "GameState.json")));

    //    savedGame.Location = gameState.Location;

    //    string[] pcFiles = Directory.GetFiles(Path.Combine(root, "PCs"));
    //    string[] pcs = new string[pcFiles.Length];
    //    for (int i = 0; i < pcFiles.Length; i++)
    //    {
    //        pcs[i] = File.ReadAllText(pcFiles[i]);
    //    }
    //    savedGame.PCsData = pcs;

    //    savedGame.GlobalStateData = File.ReadAllText(Path.Combine(root, "GlobalState.json"));

    //    InitializeGame(savedGame);
    //}

    //public void SaveGame(string name)
    //{
    //    GameMaster.RootDataPath = Path.Combine(Application.persistentDataPath, SavedGamesPath);
    //    GameMaster.SaveCurrentGame(name);
    //}

    private void InitializeGame(string data)
    {
        _gameToLoad = data;
        sceneState = SceneState.FadeOut;
    }

    public static void SwitchLocation(string location)
    {
        // 1. Make the PlayerCharacters undeletable
        //foreach (PlayerCharacter pc in FindObjectsOfType<PlayerCharacter>())
        //{
        //    pc.gameObject.transform.SetParent(null);
        //    pc.ChangeState(pc.Idle);
        //    DontDestroyOnLoad(pc.gameObject);
        //}

        //if (location == "Worldmap")
        //{
        //    // Get the current location to determine the position on the map 
        //    Instance.NextSceneName = "Worldmap";
        //    Instance._locationToLoad = location;
        //    Instance._loadAction = LoadAction.LoadWorldmap;
        //}
        //else
        //{
        //    Instance.NextSceneName = "LevelTemplate";
        //    Instance._loadAction = LoadAction.LoadLocation;
        //}
        // Instance._locationToLoad = location;
        // Instance.sceneState = SceneState.FadeOut;
    }

    #endregion

    #region Events

    public void OnSceneReady()
    {
        sceneState = SceneState.FadeIn;
    }

    #endregion

    #region InitializationSequence

    private void Update()
    {
        updateDelegates[(int)sceneState]();
    }

    private void UpdateSceneFadeOut()
    {
        if (FadingScreen.color.a < 1)
        {
            Fade(1);
        }
        else
        {
            sceneState = SceneState.Preload;
        }
    }

    private void UpdateScenePreload()
    {
        GC.Collect();
        sceneLoadTask = SceneManager.LoadSceneAsync(NextSceneName);
        sceneState = SceneState.Unload;
        if (sceneLoadTask.isDone)
        {
            sceneState = SceneState.Unload;
        }
        else
        {
            // Update some scene loading progress bar
        }
    }

    private void UpdateSceneUnload()
    {
        if (resourceUnloadTask == null)
        {
            resourceUnloadTask = Resources.UnloadUnusedAssets();
        }
        else
        {
            if (resourceUnloadTask.isDone)
            {
                resourceUnloadTask = null;
                sceneState = SceneState.Load;
            }
        }
    }

    private void UpdateSceneLoad()
    {
        SceneLoaded.Raise();
    }

    private void UpdateSceneFadeIn()
    {
        if (FadingScreen.color.a > 0)
        {
            Fade(-1);
        }
        else
        {
            FadingCanvas.enabled = false;
            sceneState = SceneState.Run;
        }
    }

    private void UpdateSceneRun()
    {
    }

    #endregion

    #region Load Actions

    private void StartGame()
    {
  

        //// Initialize the GameMaster
        //GameMaster.InitializeGame(GameData.RulebookData, GameData.ArmoursData, GameData.ItemsData, GameData.WeaponsData,
        //                          GameData.SkillsData, GameData.MonstersData, GameData.LocationsData, GameData.CellBlueprintsData,
        //                          GameData.QuestsData);

        //// Load the Game
        //GameMaster.RootDataPath = _rootDataPath;
        //GameMaster.LoadGame(_gameToLoad.PCsData, _gameToLoad.Location, _gameToLoad.GlobalStateData);

        ////  Init the Tabletop and the Location
        //GameObject tabletop = Instantiate(TabletopPrefab);
        //Level level = tabletop.GetComponent<Level>();
        //level.Location = GameMaster.CurrentLocation;
        //level.Create();

        //// Camera
        //CameraRig camera = FindObjectOfType<CameraRig>();

        //// Load the PCs  
        //List<Character> pcs = GameMaster.CharactersOfType("Player");
        //foreach (Character character in pcs)
        //{
        //    GameObject player = Instantiate(PlayerCharacterPrefab);
        //    PlayerCharacter playerCharacter = player.GetComponent<PlayerCharacter>();
        //    playerCharacter.Data = character;
        //    playerCharacter.transform.position = new Vector3(character.Transform.Position.X, 0, character.Transform.Position.Y);
        //    playerCharacter.transform.Rotate(new Vector3(0, character.Transform.Rotation, 0), Space.World);
        //    camera.Target = player.transform;
        //}

        //// Initialize UIs
        //PlayerUI.Instance.Initialize();
        //GameOverUI.Instance.Initialize();
        //HUDMessageUI.Instance.Initialize();
    }

    private void LoadLocation()
    {
        //string[] tokens = _locationToLoad.Split('.');
        //string locationToLoad = tokens[0];
        //Debug.Log("Switching Location to: " + locationToLoad);

        //GameMaster.CurrentLocation = Rulebook.Instance.Locations[locationToLoad];
        //GameObject tabletop = Instantiate(TabletopPrefab);
        //Level level = tabletop.GetComponent<Level>();
        //level.Location = GameMaster.CurrentLocation;
        //level.Create();

        //// Camera
        ////
        //CameraRig camera = FindObjectOfType<CameraRig>();

        //// Apply the Player position
        ////
        //Vector3 destinationPosition = new Vector3(0, 0, 0);
        //if (tokens.Length > 1)
        //{
        //    destinationPosition = new Vector3(-float.Parse(tokens[1]) * level.ScalingFactor,
        //                                      0,
        //                                      float.Parse(tokens[2]) * level.ScalingFactor);
        //}
        //foreach (PlayerCharacter pc in PlayerCharacter.PlayerCharacters)
        //{
        //    pc.gameObject.SetActive(true);
        //    pc.gameObject.transform.position = destinationPosition;
        //    camera.Target = pc.transform;
        //}

        //// Initialize UIs
        //PlayerUI.Instance.Initialize();
        //GameOverUI.Instance.Initialize();
        //HUDMessageUI.Instance.Initialize();
    }

    //private void LoadWorldmap()
    //{
    //    Debug.Log("Switching Location to: Worldmap");

    //    // Hide the Players and the Camera
    //    foreach (PlayerCharacter pc in PlayerCharacter.PlayerCharacters)
    //    {
    //        pc.gameObject.SetActive(false);
    //    }
    //}

    #endregion

    #region Utility

    private void Fade(int direction)
    {
        FadingCanvas.sortingOrder = 999;
        FadingCanvas.enabled = true;
        FadingScreen.color = new Color(0, 0, 0, FadingScreen.color.a + direction * Time.deltaTime * FadingSpeed);
    }

    #endregion
}
