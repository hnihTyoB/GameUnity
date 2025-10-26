using UnityEngine;
using TMPro;

/// <summary>
/// UI display for rescued victims count
/// Shows only the count, not the total (for scoring system later)
/// </summary>
public class RescueUI : Singleton<RescueUI>
{
    private TMP_Text rescueCountText;
    
    const string RESCUE_AMOUNT_TEXT = "Rescue Amount Text";
    
    protected override void Awake()
    {
        base.Awake();
        Debug.Log("RescueUI: Awake called");
    }

    private void Start()
    {
        Debug.Log("RescueUI: Start called");
        UpdateRescueCount(0);
    }

    public void UpdateRescueCount(int count)
    {
        Debug.Log($"RescueUI: UpdateRescueCount called with count={count}");
        
        if (rescueCountText == null)
        {
            rescueCountText = GameObject.Find(RESCUE_AMOUNT_TEXT)?.GetComponent<TMP_Text>();
            Debug.Log($"RescueUI: Searching for '{RESCUE_AMOUNT_TEXT}', found: {rescueCountText != null}");
        }
        
        if (rescueCountText != null)
        {
            rescueCountText.text = count.ToString("D3");
            Debug.Log($"RescueUI: Text updated to '{count.ToString("D3")}'");
        }
        else
        {
            Debug.LogError($"RescueUI: Could not find '{RESCUE_AMOUNT_TEXT}' GameObject!");
        }
    }
}

