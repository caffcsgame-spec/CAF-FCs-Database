using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScoutingRevealAnimation : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject revealPanel;
    public Image silhouetteImage;
    public Image playerCard;
    public Text playerName;
    public Text playerRating;
    public Text playerPosition;
    public Text playerStats;

    [Header("Animation Settings")]
    public float revealDelay = 2f;
    public float fadeSpeed = 2f;

    void Start()
    {
        revealPanel.SetActive(false);
    }

    public void StartReveal(ScoutedPlayer player)
    {
        StartCoroutine(RevealSequence(player));
    }

    IEnumerator RevealSequence(ScoutedPlayer player)
    {
        revealPanel.SetActive(true);

        silhouetteImage.canvasRenderer.SetAlpha(1f);
        playerCard.canvasRenderer.SetAlpha(0f);

        playerName.text = "";
        playerRating.text = "";
        playerPosition.text = "";
        playerStats.text = "";

        yield return new WaitForSeconds(revealDelay);

        playerCard.CrossFadeAlpha(1f, fadeSpeed, false);
        silhouetteImage.CrossFadeAlpha(0f, fadeSpeed, false);

        yield return new WaitForSeconds(1f);

        playerName.text = player.name;
        playerRating.text = "OVR " + player.overall_rating;
        playerPosition.text = player.position;

        yield return new WaitForSeconds(0.5f);

        playerStats.text =
            "PAC " + player.stats.pace + "\n" +
            "SHO " + player.stats.shooting + "\n" +
            "PAS " + player.stats.passing + "\n" +
            "DRI " + player.stats.dribbling + "\n" +
            "DEF " + player.stats.defense + "\n" +
            "PHY " + player.stats.physical;
    }
}
