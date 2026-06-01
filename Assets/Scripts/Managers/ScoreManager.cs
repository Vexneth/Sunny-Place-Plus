using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour, IDataPersistence
{
    [Header("= Score =")]
    [SerializeField, ReadOnly] private int currentScore = 0;
    [SerializeField] private TMP_Text scoreText;

    void Start()
    {
        GameEventsManager.instance.OnCollectableCollected += OnCollectableCollected;
        GameEventsManager.instance.OnEnemyDeath += OnEnemyDeath;
        scoreText.text = "Score:" + currentScore;
    }
    void OnDestroy()
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.OnCollectableCollected -= OnCollectableCollected;
            GameEventsManager.instance.OnEnemyDeath -= OnEnemyDeath;
        }
    }

    public void LoadData(GameData data)
    {
        this.currentScore = data.currentScore;
        scoreText.text = "Score:" + currentScore;
    }

    public void SaveData(GameData data)
    {
        data.currentScore = this.currentScore;
    }

    private void OnCollectableCollected(int score)
    {
        currentScore += score;
        scoreText.text = "Score:" + currentScore;
    }

    private void OnEnemyDeath(int score)
    {
        currentScore += score;
        scoreText.text = "Score:" + currentScore;
    }

    public void SetHighscore()
    {
        PlayerPrefs.SetInt("Highscore", currentScore);
    }

    public bool CheckHighscore()
    {
        if (PlayerPrefs.GetInt("Highscore") < currentScore)
            return true;
        return false;
    }

    public int GetScore()
    {
        return currentScore;
    }

}
