using UnityEngine;

/// <summary>
/// Singleton manager to track rescued victims count
/// Updates UI and handles rescue events
/// </summary>
public class RescueManager : Singleton<RescueManager>
{
    [Header("Rescue Tracking")]
    private int victimsRescued = 0;
    
    [Header("Events")]
    public System.Action<int> OnVictimRescuedEvent; // Event for UI update
    
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        // Initialize rescue count
        victimsRescued = 0;
        UpdateUI();
    }

    public void OnVictimRescued()
    {
        victimsRescued++;
        Debug.Log($"Victim rescued! Total: {victimsRescued}");
        
        // Notify UI
        UpdateUI();
        
        // Trigger event
        OnVictimRescuedEvent?.Invoke(victimsRescued);
    }

    private void UpdateUI()
    {
        // Update UI display
        if (RescueUI.Instance != null)
        {
            RescueUI.Instance.UpdateRescueCount(victimsRescued);
        }
    }

    public int GetVictimsRescued()
    {
        return victimsRescued;
    }

    public void ResetRescueCount()
    {
        victimsRescued = 0;
        UpdateUI();
    }
}

