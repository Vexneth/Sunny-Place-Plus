using UnityEngine;

[System.Serializable]
public class GameData
{
    public int currentScore;
    public string currentLevel;
    public Vector3 playerPosition;
    public bool IsPlayerHurt;
    public SerializableDictionary<string, bool> collectablesCollected;
    public SerializableDictionary<string, bool> enemiesKilled;

    //These will be the default values for the new game. 
    public GameData()
    {
        currentScore = 0;
        currentLevel = "FirstLevel";
        playerPosition = new Vector3(19, 4, 0);
        IsPlayerHurt = false;
        collectablesCollected = new SerializableDictionary<string, bool>();
        enemiesKilled = new SerializableDictionary<string, bool>();
    }



}
