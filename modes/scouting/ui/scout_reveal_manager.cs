using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoutRevealManager : MonoBehaviour
{
    [Header("UI References")]
    public Image cardBackground;
    public Image faceImage;
    public Text playerName;
    public Text ageText;
    public Text positionText;
    public Text overallText;
    public Text potentialText;
    public Text skillTreeText;
    public Text specialMovesText;

    [Header("Animation Settings")]
    public float revealDuration = 2.5f;
    public AnimationCurve scaleCurve;

    private string scoutingPlayersPath = "CAF FCs/modes/scouting/scouting_players.json";

    private YouthPlayer[] playersToReveal;
    private int currentIndex = 0;

    void Start()
    {
        LoadScoutedPlayers();
        if (playersToReveal.Length > 0)
        {
            StartCoroutine(RevealPlayerCard(playersToReveal[currentIndex]));
        }
    }

    private void LoadScoutedPlayers()
    {
        if (!File.Exists(scoutingPlayersPath))
        {
            Debug.LogError("Scouting players file not found: " + scoutingPlayersPath);
            playersToReveal = new YouthPlayer[0];
            return;
        }

        var json = File.ReadAllText(scoutingPlayersPath);
        var data = JsonConvert.DeserializeObject<ScoutingPlayers>(json);
        playersToReveal = data?.youth_players ?? new YouthPlayer[0];
    }

    private IEnumerator RevealPlayerCard(YouthPlayer player)
    {
        // 1. Load card background
        var bgSprite = LoadSprite(player.card_template);
        cardBackground.sprite = bgSprite;

        // 2. Load face image
        var faceSprite = LoadSprite(player.face_image);
        faceImage.sprite = faceSprite;

        // 3. Set text info
        playerName.text = player.name;
        ageText.text = $"Age: {player.age}";
        positionText.text = $"Position: {player.position}";
        overallText.text = $"Overall: {player.overall_rating}";
        potentialText.text = $"Potential: {player.potential}";
        skillTreeText.text = FormatSkillTree(player.skill_tree);
        specialMovesText.text = FormatSpecialMoves(player.special_moves);

        // 4. Animate scale + fade-in
        float timer = 0f;
        Vector3 initialScale = Vector3.zero;
        Vector3 finalScale = Vector3.one;

        while (timer < revealDuration)
        {
            timer += Time.deltaTime;
            float t = scaleCurve.Evaluate(timer / revealDuration);
            transform.localScale = Vector3.LerpUnclamped(initialScale, finalScale, t);
            yield return null;
        }

        transform.localScale = finalScale;

        // Optional: wait before revealing next player
        yield return new WaitForSeconds(1f);

        currentIndex++;
        if (currentIndex < playersToReveal.Length)
        {
            StartCoroutine(RevealPlayerCard(playersToReveal[currentIndex]));
        }
    }

    private string FormatSkillTree(System.Collections.Generic.Dictionary<string,int> skillTree)
    {
        string result = "";
        foreach (var kvp in skillTree)
        {
            result += $"{kvp.Key}: {kvp.Value}  ";
        }
        return result;
    }

    private string FormatSpecialMoves(System.Collections.Generic.List<string> moves)
    {
        if (moves.Count == 0) return "None";
        return string.Join(", ", moves);
    }

    private Sprite LoadSprite(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("Sprite not found: " + path);
            return null;
        }

        byte[] bytes = File.ReadAllBytes(path);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(bytes);
        return Sprite.Create(tex, new Rect(0,0,tex.width, tex.height), new Vector2(0.5f,0.5f));
    }
}

// --- Data Classes ---
[Serializable]
public class ScoutingPlayers
{
    public YouthPlayer[] youth_players;
}

[Serializable]
public class YouthPlayer
{
    public int youth_id;
    public string name;
    public int country_id;
    public int age;
    public string position;
    public int overall_rating;
    public int potential;
    public string card_type;
    public string card_template;
    public string face_image;
    public System.Collections.Generic.Dictionary<string,int> skill_tree;
    public System.Collections.Generic.List<string> special_moves;
    public int squad_synergy;
}
