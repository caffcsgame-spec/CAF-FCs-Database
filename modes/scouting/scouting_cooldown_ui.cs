using System;
using UnityEngine;
using UnityEngine.UI;

public class ScoutingCooldownUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Button scoutButton;
    public Text cooldownText;
    public Image buttonGlow; // Optional: glow image behind button

    [Header("Cooldown Settings")]
    public int cooldownDays = 7;
    private DateTime lastScoutTime;
    private DateTime nextAvailableTime;

    [Header("Glow Animation")]
    public float glowPulseSpeed = 2f;
    public float maxGlowAlpha = 0.6f;
    private Color originalGlowColor;

    void Start()
    {
        originalGlowColor = buttonGlow.color;
        LoadCooldown();
        UpdateUI();
        InvokeRepeating("UpdateUI", 0f, 1f);
    }

    void Update()
    {
        if (IsScoutingAvailable())
        {
            AnimateGlow();
        }
        else
        {
            buttonGlow.color = new Color(originalGlowColor.r, originalGlowColor.g, originalGlowColor.b, 0f);
        }
    }

    public void OnScoutButtonPressed()
    {
        if (IsScoutingAvailable())
        {
            Debug.Log("Scouting player...");

            lastScoutTime = DateTime.UtcNow;
            nextAvailableTime = lastScoutTime.AddDays(cooldownDays);

            SaveCooldown();
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
        PlayerPrefs.SetString("last_scout_time", lastScoutTime.ToString("o"));
        PlayerPrefs.Save();
    }

    private void AnimateGlow()
    {
        float alpha = Mathf.PingPong(Time.time * glowPulseSpeed, maxGlowAlpha);
        buttonGlow.color = new Color(originalGlowColor.r, originalGlowColor.g, originalGlowColor.b, alpha);
    }
}
