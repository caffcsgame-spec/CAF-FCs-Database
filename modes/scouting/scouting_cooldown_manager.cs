using System;
using UnityEngine;
using UnityEngine.UI;

public class ScoutingCooldownManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Button scoutButton;
    public Text cooldownText;

    [Header("Cooldown Settings")]
    public int cooldownDays = 7;
    private DateTime lastScoutTime;
    private DateTime nextAvailableTime;

    void Start()
    {
        // Load last scout timestamp from saved data
        LoadCooldown();

        // Update UI immediately
        UpdateUI();
        
        // Check cooldown periodically
        InvokeRepeating("UpdateUI", 0f, 1f);
    }

    public void OnScoutButtonPressed()
    {
        if (IsScoutingAvailable())
        {
            // Trigger scouting logic here
            Debug.Log("Scouting player...");

            // Update cooldown timestamps
            lastScoutTime = DateTime.UtcNow;
            nextAvailableTime = lastScoutTime.AddDays(cooldownDays);

            // Save cooldown
            SaveCooldown();

            // Refresh UI
            UpdateUI();
        }
    }

    private bool IsScoutingAvailable()
    {
        return DateTime.UtcNow >= nextAvailableTime;
    }

    private void UpdateUI()
    {
        if (IsScoutingAvailable())
        {
            scoutButton.interactable = true;
            cooldownText.text = "Scout now!";
        }
        else
        {
            scoutButton.interactable = false;
            TimeSpan remaining = nextAvailableTime - DateTime.UtcNow;
            cooldownText.text = $"Next scouting available in {remaining.Hours}h {remaining.Minutes}m {remaining.Seconds}s";
        }
    }

    private void LoadCooldown()
    {
        string lastScoutStr = PlayerPrefs.GetString("last_scout_time", "");
        if (!string.IsNullOrEmpty(lastScoutStr))
        {
            lastScoutTime = DateTime.Parse(lastScoutStr, null, System.Globalization.DateTimeStyles.RoundtripKind);
            nextAvailableTime = lastScoutTime.AddDays(cooldownDays);
        }
        else
        {
            lastScoutTime = DateTime.MinValue;
            nextAvailableTime = DateTime.MinValue;
        }
    }

    private void SaveCooldown()
    {
        PlayerPrefs.SetString("last_scout_time", lastScoutTime.ToString("o")); // ISO 8601 format
        PlayerPrefs.Save();
    }
}
