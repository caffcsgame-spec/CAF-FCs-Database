using UnityEngine;
using UnityEngine.UI;

public class ScoutMenuUI : MonoBehaviour
{

    public Button startScoutButton;
    public Button viewResultButton;

    public Text scoutLevelText;
    public Text scoutXPText;
    public Text cooldownText;

    public ScoutingCoreSystem scoutingSystem;
    public ScoutResultUI resultUI;

    private float cooldownTime = 60f;
    private float cooldownTimer = 0f;

    void Start()
    {
        startScoutButton.onClick.AddListener(StartScout);
        viewResultButton.onClick.AddListener(OpenLastResult);

        UpdateUI();
    }

    void Update()
    {

        if(cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;

            cooldownText.text = "Next Scout: " + Mathf.Ceil(cooldownTimer) + "s";
        }
        else
        {
            cooldownText.text = "Ready";
        }

    }

    void StartScout()
    {

        if(cooldownTimer > 0)
            return;

        ScoutedPlayer player = scoutingSystem.StartScouting();

        resultUI.SetResult(player);

        cooldownTimer = cooldownTime;

        UpdateUI();

    }

    void OpenLastResult()
    {
        resultUI.Show();
    }

    void UpdateUI()
    {

        scoutLevelText.text = "Level: " + scoutingSystem.scoutLevel;

        scoutXPText.text = "XP: " + scoutingSystem.scoutXP;

    }

}
