using System;
using UnityEngine;
using UnityEngine.UI;

public class ScoutingCooldownUI : MonoBehaviour
{
    public Button scoutButton;
    public Text cooldownText;

    [Header("Cooldown")]
    public int cooldownDays = 7;

    [Header("Glow Animation")]
    public float pulseSpeed = 2f;
    public float scaleAmount = 0.05f;
    public Color readyColor = new Color(1f, 0.85f, 0.2f);

    private Vector3 originalScale;
    private Color originalColor;

    private DateTime lastScoutTime;
    private DateTime nextAvailableTime;

    void Start()
    {
        originalScale = scoutButton.transform.localScale;
        originalColor = scoutButton.image.color;

        LoadCooldown();
        UpdateUI();

        InvokeRepeating(nameof(UpdateUI), 0f, 1f);
    }

    void Update()
    {
        if (IsScoutingAvailable())
        {
            AnimateButton();
        }
        else
        {
            scoutButton.transform.localScale = originalScale;
            scoutButton.image.color = originalColor;
        }
    }

    public void OnScoutButtonPressed()
    {
        if (!IsScoutingAvailable())
            return;

        Debug.Log("Scouting player...");

        lastScoutTime = DateTime.UtcNow;
        nextAvailableTime = lastScoutTime.AddDays(cooldownDays);

        SaveCooldown();
        UpdateUI();
    }

    bool IsScoutingAvailable()
    {
        return DateTime.UtcNow >= nextAvailableTime;
    }

    void UpdateUI()
    {
        if (IsScoutingAvailable())
        {
            scoutButton.interactable = true;
            cooldownText.text = "Scout Ready";
        }
        else
        {
            scoutButton.interactable = false;

            TimeSpan remain = nextAvailableTime - DateTime.UtcNow;

            cooldownText.text =
                remain.Days + "d " +
                remain.Hours + "h " +
                remain.Minutes + "m " +
                remain.Seconds + "s";
        }
    }

    void AnimateButton()
    {
        float scale = 1 + Mathf.Sin(Time.time * pulseSpeed) * scaleAmount;

        scoutButton.transform.localScale =
            originalScale * scale;

        scoutButton.image.color =
            Color.Lerp(originalColor, readyColor,
            (Mathf.Sin(Time.time * pulseSpeed) + 1) / 2);
    }

    void LoadCooldown()
    {
        string lastScout = PlayerPrefs.GetString("last_scout_time", "");

        if (!string.IsNullOrEmpty(lastScout))
        {
            lastScoutTime =
                DateTime.Parse(lastScout, null,
                System.Globalization.DateTimeStyles.RoundtripKind);

            nextAvailableTime =
                lastScoutTime.AddDays(cooldownDays);
        }
        else
        {
            nextAvailableTime = DateTime.MinValue;
        }
    }

    void SaveCooldown()
    {
        PlayerPrefs.SetString(
            "last_scout_time",
            lastScoutTime.ToString("o")
        );

        PlayerPrefs.Save();
    }
}
