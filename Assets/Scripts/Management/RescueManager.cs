using UnityEngine;


public class RescueManager : Singleton<RescueManager>
{
    [Header("Rescue Tracking")]
    private int victimsRescued = 0;
    
    [Header("Events")]
    public System.Action<int> OnVictimRescuedEvent; 
    
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
      
        victimsRescued = 0;
        UpdateUI();
    }

    public void OnVictimRescued()
    {
        victimsRescued++;
        
       
        UpdateUI();
        

        OnVictimRescuedEvent?.Invoke(victimsRescued);
    }

    private void UpdateUI()
    {
     
        if (RescueUI.Instance != null)
        {
            RescueUI.Instance.UpdateRescueCount(victimsRescued);
        }
        else
        {
            Debug.LogError("RescueManager: RescueUI.Instance is NULL!");
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

