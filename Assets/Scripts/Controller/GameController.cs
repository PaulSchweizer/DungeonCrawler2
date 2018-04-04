using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("Game Data")]
    public string SavedGamesPath = "SavedGames";
    public CharacterSet CharacterSet;

    [Header("Fading Screen")]
    public Canvas FadingCanvas;
    public Image FadingScreen;
    public float FadingSpeed = 3;
    public Text LoadingText;

    [Header("Prefabs")]
    public TextAsset DefaultGame;
    public PlayerCharacter PlayerPrefab;

    [Header("Internal Data")]
    public string PreviousSceneName;
    public string NextSceneName;

    [Header("Events")]
    public StringGameEvent OnSceneLoaded;

    // Internals
    private string _rootDataPath;

    private AsyncOperation resourceUnloadTask;
    private AsyncOperation sceneLoadTask;
    private enum SceneState { FadeOut, Preload, Load, Unload, Run, FadeIn, Count };
    private SceneState sceneState;
    private delegate void UpdateDelegate();
    private UpdateDelegate[] updateDelegates;

    public static GameController Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != null)
        {
            Destroy(gameObject);
        }

        // Set internals
        _rootDataPath = Path.Combine(Application.persistentDataPath, SavedGamesPath);

        // Delegates
        updateDelegates = new UpdateDelegate[(int)SceneState.Count];
        updateDelegates[(int)SceneState.FadeOut] = UpdateSceneFadeOut;
        updateDelegates[(int)SceneState.Preload] = UpdateScenePreload;
        updateDelegates[(int)SceneState.Load] = UpdateSceneLoad;
        updateDelegates[(int)SceneState.Unload] = UpdateSceneUnload;
        updateDelegates[(int)SceneState.FadeIn] = UpdateSceneFadeIn;
        updateDelegates[(int)SceneState.Run] = UpdateSceneRun;
        sceneState = SceneState.Run;
    }

    #region Actions

    public void NewGame()
    {
        Dictionary<string, string> data = SerializationUtilitites.DeserializeFromJson<Dictionary<string, string>>(DefaultGame.text);
        InitializeGame(data);
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

    private void InitializeGame(Dictionary<string, string> data)
    {
        NextSceneName = data["Location"];
        sceneState = SceneState.FadeOut;
    }

    public void SwitchLevel(string level)
    {
        PreviousSceneName = SceneManager.GetActiveScene().name;
        NextSceneName = level;
        sceneState = SceneState.FadeOut;
    }

    public void SaveGame()
    {
        string savedGameName = "SavedGame"; // TODO Replace this by a user provided name or the current date?
        string rootPath = Path.Combine(_rootDataPath, savedGameName);
        
        // Save the Game State
        string savePath = Path.Combine(rootPath, "GameSata.json");
        FileInfo saveFile = new FileInfo(savePath);
        saveFile.Directory.Create();
        Dictionary<string, string> data = new Dictionary<string, string>();
        data["Location"] = SceneManager.GetActiveScene().name;
        File.WriteAllText(saveFile.FullName, SerializationUtilitites.SerializeToJson(data));
        Debug.Log(string.Format("Saved Game to: {0}", savePath));

        // Save the Character
        foreach(BaseCharacter player in CharacterSet.Items)
        {
            savePath = Path.Combine(Path.Combine(rootPath, "Characters"), player.Stats.Name + ".json");
            saveFile = new FileInfo(savePath);
            saveFile.Directory.Create();
            File.WriteAllText(saveFile.FullName, SerializationUtilitites.SerializeToJson(player.SerializeToData()));
            Debug.Log(string.Format("Saved {0} to: {1}", player.Stats.Name, savePath));
        }
    }

    public void LoadGame()
    {
        string savedGameName = "SavedGame"; // TODO Replace this by a user provided name or the current date?
        string rootPath = Path.Combine(_rootDataPath, savedGameName);
        
        // Game State
        string savePath = Path.Combine(rootPath, "GameSata.json");
        string json = File.ReadAllText(savePath);
        Dictionary<string, string> data = SerializationUtilitites.DeserializeFromJson<Dictionary<string, string>>(json);
        InitializeGame(data);
        Debug.Log(string.Format("Loaded Game from: {0}", savePath));

        // Characters
        savePath = Path.Combine(rootPath, "Characters");
        foreach (string file in Directory.GetFiles(savePath, "*.json"))
        {
            json = File.ReadAllText(file);
            PlayerCharacter player = Instantiate(PlayerPrefab);
            player.DeserializeFromJson(json);
            Debug.Log(string.Format("Loaded Character from: {0}", file));
        }
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
        // Pass on the current scene name
        OnSceneLoaded.Raise(PreviousSceneName);
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
