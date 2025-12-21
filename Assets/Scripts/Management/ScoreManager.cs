using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : Singleton<ScoreManager>
{
    [Header("Score Settings")]
    private int currentScore = 0;
    
    [Header("Statistics")]
    private int victimsInSafeZone = 0; 
    private int enemiesHit = 0;
    private int enemiesKilled = 0;
    private int totalHitPenalty = 0; 
    private int totalKillPenalty = 0;
    
    [Header("Time Tracking")]
    private float playTime = 0f;
    private bool isTimerRunning = false;
    
    [Header("Time Bonus")]
    [SerializeField] private float timeBonusThreshold = 180f; 
    [SerializeField] private int timeBonusAmount = 500; 
    private bool timeBonusAwarded = false; 
    
    [Header("Events")]
    public System.Action<int> OnScoreChanged; 
    public System.Action OnLevelEnd; 
    
 
    private const string HIGH_SCORE_EASY_KEY = "HighScore_Easy";
    private const string HIGH_SCORE_NORMAL_KEY = "HighScore_Normal";
    private const string HIGH_SCORE_HARD_KEY = "HighScore_Hard";
    
  
    private const int RESCUE_POINTS_EASY = 50;
    private const int RESCUE_POINTS_NORMAL = 100;
    private const int RESCUE_POINTS_HARD = 200;
    
    private const int HIT_PENALTY_EASY = 2;
    private const int HIT_PENALTY_NORMAL = 5;
    private const int HIT_PENALTY_HARD = 10;
    
    private const int KILL_PENALTY_EASY = 20;
    private const int KILL_PENALTY_NORMAL = 50;
    private const int KILL_PENALTY_HARD = 100;
    
    protected override void Awake()
    {
        base.Awake();
    }
    
    private void Start()
    {
    
        ResetScore();
        
    
        StartTimer();
        
      
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void Update()
    {
      
        if (isTimerRunning && Time.timeScale > 0f)
        {
            playTime += Time.deltaTime;
        }
    }
    
    private void OnDestroy()
    {
      
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
      
        if (scene.name == "Scene1" || scene.name == "MainMenu")
        {
            ResetScore();
            
           
            if (scene.name == "Scene1" && RescueManager.Instance != null)
            {
                RescueManager.Instance.ResetRescueCount();
            }
            
        }
        else
        {
            Debug.Log($"ScoreManager: Scene {scene.name} loaded, keeping current score: {currentScore}");
        }
    }
    
   
    public void ResetScore()
    {
        currentScore = 0;
        victimsInSafeZone = 0;
        enemiesHit = 0;
        enemiesKilled = 0;
        totalHitPenalty = 0;
        totalKillPenalty = 0;
        playTime = 0f;
        timeBonusAwarded = false;
        OnScoreChanged?.Invoke(currentScore);
    }
    

    public void StartTimer()
    {
        isTimerRunning = true;
        Debug.Log("ScoreManager: Timer started");
    }
    
 
    public void StopTimer()
    {
        isTimerRunning = false;
        Debug.Log($"ScoreManager: Timer stopped at {GetPlayTimeFormatted()}");
    }
    

    public float GetPlayTime()
    {
        return playTime;
    }
    

    public string GetPlayTimeFormatted()
    {
        int minutes = Mathf.FloorToInt(playTime / 60f);
        int seconds = Mathf.FloorToInt(playTime % 60f);
        return $"{minutes:00}:{seconds:00}";
    }
    
 
    public void AddSafeZonePoints()
    {
        int points = GetRescuePoints();
        currentScore += points; 
        victimsInSafeZone++;
        OnScoreChanged?.Invoke(Mathf.Max(0, currentScore)); 
    }
    
  
    public void AddHitPoints()
    {
        int penalty = GetHitPenalty();
        currentScore -= penalty; 
        enemiesHit++;
        totalHitPenalty += penalty;
        OnScoreChanged?.Invoke(Mathf.Max(0, currentScore)); 
    }
    

    public void AddKillPoints()
    {
        int penalty = GetKillPenalty();
        currentScore -= penalty; 
        enemiesKilled++;
        totalKillPenalty += penalty;
        OnScoreChanged?.Invoke(Mathf.Max(0, currentScore)); 
    }
    
 
    private int GetRescuePoints()
    {
        DifficultyManager.Difficulty difficulty = DifficultyManager.GetCurrentDifficulty();
        switch (difficulty)
        {
            case DifficultyManager.Difficulty.Easy:
                return RESCUE_POINTS_EASY;
            case DifficultyManager.Difficulty.Normal:
                return RESCUE_POINTS_NORMAL;
            case DifficultyManager.Difficulty.Hard:
                return RESCUE_POINTS_HARD;
            default:
                return RESCUE_POINTS_NORMAL;
        }
    }

    private int GetHitPenalty()
    {
        DifficultyManager.Difficulty difficulty = DifficultyManager.GetCurrentDifficulty();
        switch (difficulty)
        {
            case DifficultyManager.Difficulty.Easy:
                return HIT_PENALTY_EASY;
            case DifficultyManager.Difficulty.Normal:
                return HIT_PENALTY_NORMAL;
            case DifficultyManager.Difficulty.Hard:
                return HIT_PENALTY_HARD;
            default:
                return HIT_PENALTY_NORMAL;
        }
    }
    
 
    private int GetKillPenalty()
    {
        DifficultyManager.Difficulty difficulty = DifficultyManager.GetCurrentDifficulty();
        switch (difficulty)
        {
            case DifficultyManager.Difficulty.Easy:
                return KILL_PENALTY_EASY;
            case DifficultyManager.Difficulty.Normal:
                return KILL_PENALTY_NORMAL;
            case DifficultyManager.Difficulty.Hard:
                return KILL_PENALTY_HARD;
            default:
                return KILL_PENALTY_NORMAL;
        }
    }
    

    public void OnLevelComplete()
    {
    
        StopTimer();
        
        if (playTime <= timeBonusThreshold)
        {
            currentScore += timeBonusAmount;
            timeBonusAwarded = true;
            OnScoreChanged?.Invoke(Mathf.Max(0, currentScore)); 
        }
        
 
        currentScore = Mathf.Max(0, currentScore);
        

        SaveHighScore();
        
       
        OnLevelEnd?.Invoke();
        
    }
    
 
    private void SaveHighScore()
    {
        DifficultyManager.Difficulty difficulty = DifficultyManager.GetCurrentDifficulty();
        string key = GetHighScoreKey(difficulty);
        int highScore = GetHighScore();
        
        
        if (currentScore > highScore)
        {
            PlayerPrefs.SetInt(key, currentScore);
            PlayerPrefs.Save();
            
            int savedScore = PlayerPrefs.GetInt(key, -1);
        }
        else
        {
            Debug.Log($"ScoreManager: No new high score. Current: {currentScore} <= High: {highScore}");
        }
    }
 
    public int GetHighScore()
    {
        DifficultyManager.Difficulty difficulty = DifficultyManager.GetCurrentDifficulty();
        string key = GetHighScoreKey(difficulty);
        int highScore = PlayerPrefs.GetInt(key, 0);
        return highScore;
    }
    

    public int GetHighScore(DifficultyManager.Difficulty difficulty)
    {
        string key = GetHighScoreKey(difficulty);
        return PlayerPrefs.GetInt(key, 0);
    }
    
 
    private string GetHighScoreKey(DifficultyManager.Difficulty difficulty)
    {
        switch (difficulty)
        {
            case DifficultyManager.Difficulty.Easy:
                return HIGH_SCORE_EASY_KEY;
            case DifficultyManager.Difficulty.Normal:
                return HIGH_SCORE_NORMAL_KEY;
            case DifficultyManager.Difficulty.Hard:
                return HIGH_SCORE_HARD_KEY;
            default:
                return HIGH_SCORE_NORMAL_KEY;
        }
    }
    

    public int GetCurrentScore() => Mathf.Max(0, currentScore); // Always return non-negative
    public int GetVictimsInSafeZone() => victimsInSafeZone;
    public int GetEnemiesHit() => enemiesHit;
    public int GetEnemiesKilled() => enemiesKilled;
    public int GetTotalHitPenalty() => totalHitPenalty;
    public int GetTotalKillPenalty() => totalKillPenalty;

    public bool WasTimeBonusAwarded() => timeBonusAwarded;
    public int GetTimeBonusAmount() => timeBonusAmount;
}

