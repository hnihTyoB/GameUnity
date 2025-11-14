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
    private int totalHitPenalty = 0; // Total points lost from hitting enemies
    private int totalKillPenalty = 0; // Total points lost from killing enemies
    
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
    /// Only reset when starting a new game (Scene1) or going to MainMenu
    /// Don't reset when progressing through levels (Scene1 -> Scene2)
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"ScoreManager: OnSceneLoaded called - Scene: {scene.name}, Mode: {mode}");
        
        // Reset score only when:
        // 1. Loading Scene1 (start of new game)
        // 2. Loading MainMenu (back to menu)
        if (scene.name == "Scene1" || scene.name == "MainMenu")
        {
            Debug.Log($"ScoreManager: Resetting score for scene: {scene.name}");
            ResetScore();
            Debug.Log($"ScoreManager: Score reset complete");
        }
        else
        {
            Debug.Log($"ScoreManager: Scene {scene.name} loaded, keeping current score: {currentScore}");
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
        totalHitPenalty = 0;
        totalKillPenalty = 0;
        OnScoreChanged?.Invoke(currentScore);
    }
    
    /// <summary>
    /// Add points when victim reaches safe zone (depends on difficulty)
    /// This is called by SafeZone when victim enters the safe zone
    /// </summary>
    public void AddSafeZonePoints()
    {
        int points = GetRescuePoints();
        currentScore += points; // Allow negative score temporarily
        victimsInSafeZone++;
        OnScoreChanged?.Invoke(Mathf.Max(0, currentScore)); // Display as 0 if negative
        Debug.Log($"ScoreManager: Victim reached safe zone! +{points} points (Total: {currentScore}, Victims: {victimsInSafeZone})");
    }
    
    /// <summary>
    /// Subtract points for hitting an enemy (depends on difficulty)
    /// </summary>
    public void AddHitPoints()
    {
        int penalty = GetHitPenalty();
        currentScore -= penalty; // Allow negative score temporarily
        enemiesHit++;
        totalHitPenalty += penalty;
        OnScoreChanged?.Invoke(Mathf.Max(0, currentScore)); // Display as 0 if negative
        Debug.Log($"ScoreManager: Hit enemy! -{penalty} points (Total: {currentScore})");
    }
    
    /// <summary>
    /// Subtract points for killing an enemy/shadow (depends on difficulty)
    /// </summary>
    public void AddKillPoints()
    {
        int penalty = GetKillPenalty();
        currentScore -= penalty; // Allow negative score temporarily
        enemiesKilled++;
        totalKillPenalty += penalty;
        OnScoreChanged?.Invoke(Mathf.Max(0, currentScore)); // Display as 0 if negative
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
        // Clamp final score to minimum 0
        currentScore = Mathf.Max(0, currentScore);
        
        // Save high score if current score is higher
        SaveHighScore();
        
        Debug.Log($"ScoreManager: Level complete! Final Score: {currentScore}, High Score: {GetHighScore()}");
        Debug.Log($"ScoreManager: Stats - Victims: {victimsInSafeZone}, Hits: {enemiesHit} (-{totalHitPenalty}), Kills: {enemiesKilled} (-{totalKillPenalty})");
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
    public int GetCurrentScore() => Mathf.Max(0, currentScore); // Always return non-negative
    public int GetVictimsInSafeZone() => victimsInSafeZone;
    public int GetEnemiesHit() => enemiesHit;
    public int GetEnemiesKilled() => enemiesKilled;
    public int GetTotalHitPenalty() => totalHitPenalty;
    public int GetTotalKillPenalty() => totalKillPenalty;
}

