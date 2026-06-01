using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;


public class DataPersistenceManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private bool initializeDataIfNull = false;

    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    private GameData gameData;
    private List<IDataPersistence> dataPersistencesObjects;
    private FileDataHandler dataHandler;
    private bool isNewGame = false;

    public static DataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            //Debug.Log("Found more than one Data Persistence Manager in the scene. Destroying the newest one!");
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);

        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
    }


    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log("onSceneLoaded!");
        if (dataHandler == null)
        {
            //Debug.Log("why am I not dead yet!");
            return;
        }

        this.dataPersistencesObjects = FindAllDataPersistenceObjects();
        if (isNewGame)
        {
            foreach (IDataPersistence dataPersistenceObj in dataPersistencesObjects)
            {
                dataPersistenceObj.LoadData(gameData);
            }
        }
        else
        {
            LoadGame();
        }
    }


    public void NewGame()
    {
        this.gameData = new GameData();
        isNewGame = true;
    }

    public void LoadGame()
    {
        this.gameData = dataHandler.Load();

        if (this.gameData == null && initializeDataIfNull)
        {
            NewGame();
        }

        if (this.gameData == null)
        {
            Debug.Log("No data was found, start a new game instead.");
            return;
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistencesObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }

    }

    public void SaveGame()
    {
        if (gameData == null)
        {
            Debug.LogWarning("No data was found. A new game needs to be started before data can be saved!");
        }
        foreach (IDataPersistence dataPersistenceObj in dataPersistencesObjects)
        {
            dataPersistenceObj.SaveData(gameData);
        }

        dataHandler.Save(gameData);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        //IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public bool HasGameData()
    {
        return gameData != null;
    }

    public String GetLevelName()
    {
        if (gameData != null)
        {
            return gameData.currentLevel;
        }
        return null;
    }
    public bool WasPlayerDead()
    {
        if (gameData != null)
        {
            return gameData.IsPlayerHurt;
        }
        return true;
    }
}
