using UnityEngine;
using TMPro;

/// <summary>
/// UI display for rescued victims count
/// Shows only the count, not the total (for scoring system later)
/// </summary>
public class RescueUI : Singleton<RescueUI>
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI rescueCountText;
    [SerializeField] private string prefix = "Rescued: "; // "Rescued: X"
    
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
        if (rescueCountText != null)
        {
            rescueCountText.text = $"{prefix}{count}";
        }
    }
}

