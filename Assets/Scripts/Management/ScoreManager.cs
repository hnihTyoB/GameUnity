using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages scoring system for non-violence game
/// - Adds points for rescuing victims
/// - Subtracts points for hitting/killing enemies
/// - Tracks high score per difficulty
/// </summary>
public class ScoreManager : Singleton<ScoreManager>
{
    [Header("Score Settings")]
    private int currentScore = 0;
    
    [Header("Statistics")]
    private int victimsInSafeZone = 0; // Victims that reached safe zone (scored)
    private int enemiesHit = 0;
    private int enemiesKilled = 0;
    
    [Header("Events")]
    public System.Action<int> OnScoreChanged; // Event when score changes
    public System.Action OnLevelEnd; // Event when level ends
    
    // PlayerPrefs keys for high scores
    private const string HIGH_SCORE_EASY_KEY = "HighScore_Easy";
    private const string HIGH_SCORE_NORMAL_KEY = "HighScore_Normal";
    private const string HIGH_SCORE_HARD_KEY = "HighScore_Hard";
    
    // Score values (base, multiplied by difficulty)
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
        // Reset score when level starts
        ResetScore();
        
        // Subscribe to scene loaded event to reset score on new level
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from scene loaded event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    /// <summary>
    /// Reset score when a new scene is loaded
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"ScoreManager: OnSceneLoaded called - Scene: {scene.name}, Mode: {mode}");
        
        // Don't reset if loading main menu
        if (scene.name != "MainMenu")
        {
            Debug.Log($"ScoreManager: Resetting score for scene: {scene.name}");
            ResetScore();
            Debug.Log($"ScoreManager: Score reset complete");
        }
        else
        {
            Debug.Log($"ScoreManager: MainMenu loaded, skipping score reset");
        }
    }
    
    /// <summary>
    /// Reset score at start of level
    /// </summary>
    public void ResetScore()
    {
        currentScore = 0;
        victimsInSafeZone = 0;
        enemiesHit = 0;
        enemiesKilled = 0;
        OnScoreChanged?.Invoke(currentScore);
    }
    
    /// <summary>
    /// Add points when victim reaches safe zone (depends on difficulty)
    /// This is called by SafeZone when victim enters the safe zone
    /// </summary>
    public void AddSafeZonePoints()
    {
        int points = GetRescuePoints();
        currentScore = Mathf.Max(0, currentScore + points); // Minimum = 0
        victimsInSafeZone++;
        OnScoreChanged?.Invoke(currentScore);
        Debug.Log($"ScoreManager: Victim reached safe zone! +{points} points (Total: {currentScore})");
    }
    
    /// <summary>
    /// Subtract points for hitting an enemy (depends on difficulty)
    /// </summary>
    public void AddHitPoints()
    {
        int penalty = GetHitPenalty();
        currentScore = Mathf.Max(0, currentScore - penalty); // Minimum = 0
        enemiesHit++;
        OnScoreChanged?.Invoke(currentScore);
        Debug.Log($"ScoreManager: Hit enemy! -{penalty} points (Total: {currentScore})");
    }
    
    /// <summary>
    /// Subtract points for killing an enemy/shadow (depends on difficulty)
    /// </summary>
    public void AddKillPoints()
    {
        int penalty = GetKillPenalty();
        currentScore = Mathf.Max(0, currentScore - penalty); // Minimum = 0
        enemiesKilled++;
        OnScoreChanged?.Invoke(currentScore);
        Debug.Log($"ScoreManager: Killed enemy! -{penalty} points (Total: {currentScore})");
    }
    
    /// <summary>
    /// Get rescue points based on current difficulty
    /// </summary>
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
    
    /// <summary>
    /// Get hit penalty based on current difficulty
    /// </summary>
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
    
    /// <summary>
    /// Get kill penalty based on current difficulty
    /// </summary>
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
    
    /// <summary>
    /// Save high score and trigger level end event
    /// </summary>
    public void OnLevelComplete()
    {
        // Save high score if current score is higher
        SaveHighScore();
        
        Debug.Log($"ScoreManager: Level complete! Final Score: {currentScore}, High Score: {GetHighScore()}");
        Debug.Log($"ScoreManager: OnLevelEnd event has {OnLevelEnd?.GetInvocationList().Length ?? 0} subscribers");
        
        // Trigger level end event
        OnLevelEnd?.Invoke();
        
        Debug.Log("ScoreManager: OnLevelEnd event invoked!");
    }
    
    /// <summary>
    /// Save high score for current difficulty
    /// </summary>
    private void SaveHighScore()
    {
        int highScore = GetHighScore();
        if (currentScore > highScore)
        {
            DifficultyManager.Difficulty difficulty = DifficultyManager.GetCurrentDifficulty();
            string key = GetHighScoreKey(difficulty);
            PlayerPrefs.SetInt(key, currentScore);
            PlayerPrefs.Save();
            Debug.Log($"ScoreManager: New high score! {currentScore} (Previous: {highScore})");
        }
    }
    
    /// <summary>
    /// Get high score for current difficulty
    /// </summary>
    public int GetHighScore()
    {
        DifficultyManager.Difficulty difficulty = DifficultyManager.GetCurrentDifficulty();
        string key = GetHighScoreKey(difficulty);
        return PlayerPrefs.GetInt(key, 0);
    }
    
    /// <summary>
    /// Get high score for specific difficulty
    /// </summary>
    public int GetHighScore(DifficultyManager.Difficulty difficulty)
    {
        string key = GetHighScoreKey(difficulty);
        return PlayerPrefs.GetInt(key, 0);
    }
    
    /// <summary>
    /// Get PlayerPrefs key for high score based on difficulty
    /// </summary>
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
    
    // Public getters
    public int GetCurrentScore() => currentScore;
    public int GetVictimsInSafeZone() => victimsInSafeZone;
    public int GetEnemiesHit() => enemiesHit;
    public int GetEnemiesKilled() => enemiesKilled;
}

