using UnityEngine;
using TMPro;


public class RescueUI : Singleton<RescueUI>
{
    private TMP_Text rescueCountText;
    
    const string RESCUE_AMOUNT_TEXT = "Rescue Amount Text";
    
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        UpdateRescueCount(0);
    }

    public void UpdateRescueCount(int count)
    {
        Debug.Log($"RescueUI: UpdateRescueCount called with count={count}");
        
        if (rescueCountText == null)
        {
            rescueCountText = GameObject.Find(RESCUE_AMOUNT_TEXT)?.GetComponent<TMP_Text>();
        }
        
        if (rescueCountText != null)
        {
            rescueCountText.text = count.ToString("D3");
        }
        else
        {
            Debug.LogError($"RescueUI: Could not find '{RESCUE_AMOUNT_TEXT}' GameObject!");
        }
    }
}

