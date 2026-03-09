using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

public class TalentProbabilitySystem
{
    // Paths
    private string scoutingConfigPath = "CAF FCs/modes/scouting/scouting_config.json";
    private string talentProbabilityPath = "CAF FCs/modes/scouting/african_talent_probabilities.json";
    private string scoutingPlayersPath = "CAF FCs/modes/scouting/scouting_players.json";

    private ScoutingConfig scoutingConfig;
    private AfricanTalentProbabilities talentProbabilities;
    private Random random = new Random();

    public TalentProbabilitySystem()
    {
        LoadConfigs();
    }

    private void LoadConfigs()
    {
        scoutingConfig = JsonConvert.DeserializeObject<ScoutingConfig>(File.ReadAllText(scoutingConfigPath));
        talentProbabilities = JsonConvert.DeserializeObject<AfricanTalentProbabilities>(File.ReadAllText(talentProbabilityPath));
    }

    public YouthPlayer GenerateYouthPlayer(string position, int? age = null, string customName = null)
    {
        // 1. Choose age if not specified
        int playerAge = age ?? scoutingConfig.youth_age_pool[random.Next(scoutingConfig.youth_age_pool.Length)];

        // 2. Choose region
        var regions = new List<RegionProbability>(talentProbabilities.regions.Values);
        RegionProbability chosenRegion = regions[random.Next(regions.Count)];

        // 3. Choose country from region
        int countryId = chosenRegion.countries[random.Next(chosenRegion.countries.Length)];

        // 4. Determine card type based on probability
        string cardType = DetermineCardType(chosenRegion.probabilities);

        // 5. Determine overall and potential
        int overall = random.Next(scoutingConfig.overall_rating_ranges[playerAge.ToString()]["min"],
                                   scoutingConfig.overall_rating_ranges[playerAge.ToString()]["max"] + 1);
        int potential = DeterminePotential(cardType);

        // 6. Select face image
        string faceImagePath = GetFaceImage(playerAge);

        // 7. Assign card template
        string cardTemplate = scoutingConfig.card_templates[cardType.ToLower()];

        // 8. Skill tree init
        var skillTree = new Dictionary<string,int> {
            { "dribbling", random.Next(1,6) },
            { "shooting", random.Next(1,6) },
            { "passing", random.Next(1,6) },
            { "defense", random.Next(1,6) },
            { "physical", random.Next(1,6) }
        };

        // 9. Special moves & synergy init
        var specialMoves = new List<string>();
        int squadSynergy = 0;

        // 10. Create YouthPlayer object
        YouthPlayer player = new YouthPlayer
        {
            youth_id = GenerateUniqueId(),
            name = customName ?? GenerateRandomName(),
            country_id = countryId,
            age = playerAge,
            position = position,
            overall_rating = overall,
            potential = potential,
            card_type = cardType,
            card_template = cardTemplate,
            face_image = faceImagePath,
            skill_tree = skillTree,
            special_moves = specialMoves,
            squad_synergy = squadSynergy
        };

        // 11. Save to scouting_players.json
        AddPlayerToScoutingList(player);

        return player;
    }

    private string DetermineCardType(Dictionary<string, double> probabilities)
    {
        double roll = random.NextDouble();
        double cumulative = 0;
        foreach (var kvp in probabilities)
        {
            cumulative += kvp.Value;
            if (roll <= cumulative) return kvp.Key;
        }
        return "Common"; // fallback
    }

    private int DeterminePotential(string cardType)
    {
        var range = scoutingConfig.potential_ranges[cardType.ToLower()];
        return random.Next(range["min"], range["max"] + 1);
    }

    private string GetFaceImage(int age)
    {
        string folder = scoutingConfig.face_image_paths[age.ToString()];
        string[] images = Directory.GetFiles(folder, "*.png");
        return images[random.Next(images.Length)];
    }

    private int GenerateUniqueId()
    {
        return (int)(DateTime.UtcNow.Ticks % 1000000); // simple unique id
    }

    private string GenerateRandomName()
    {
        string[] firstNames = { "Abdul", "Ibrahim", "Peter", "Joseph", "Mohamed" };
        string[] lastNames = { "Kamara", "Diallo", "Ndlovu", "Mensah", "Traore" };
        return firstNames[random.Next(firstNames.Length)] + " " + lastNames[random.Next(lastNames.Length)];
    }

    private void AddPlayerToScoutingList(YouthPlayer player)
    {
        List<YouthPlayer> currentPlayers = new List<YouthPlayer>();
        if (File.Exists(scoutingPlayersPath))
        {
            var json = File.ReadAllText(scoutingPlayersPath);
            var data = JsonConvert.DeserializeObject<ScoutingPlayers>(json);
            if (data?.youth_players != null) currentPlayers = new List<YouthPlayer>(data.youth_players);
        }
        currentPlayers.Add(player);
        var saveData = new ScoutingPlayers { youth_players = currentPlayers.ToArray() };
        File.WriteAllText(scoutingPlayersPath, JsonConvert.SerializeObject(saveData, Formatting.Indented));
    }
}

// --- Data Classes ---
public class ScoutingPlayers
{
    public YouthPlayer[] youth_players { get; set; }
}

public class YouthPlayer
{
    public int youth_id { get; set; }
    public string name { get; set; }
    public int country_id { get; set; }
    public int age { get; set; }
    public string position { get; set; }
    public int overall_rating { get; set; }
    public int potential { get; set; }
    public string card_type { get; set; }
    public string card_template { get; set; }
    public string face_image { get; set; }
    public Dictionary<string,int> skill_tree { get; set; }
    public List<string> special_moves { get; set; }
    public int squad_synergy { get; set; }
}

public class ScoutingConfig
{
    public int weekly_scouting_limit { get; set; }
    public int default_cooldown_days { get; set; }
    public int[] youth_age_pool { get; set; }
    public Dictionary<string, Dictionary<string,int>> overall_rating_ranges { get; set; }
    public Dictionary<string, Dictionary<string,int>> potential_ranges { get; set; }
    public Dictionary<string,string> card_templates { get; set; }
    public Dictionary<string,string> face_image_paths { get; set; }
}

public class AfricanTalentProbabilities
{
    public Dictionary<string, RegionProbability> regions { get; set; }
}

public class RegionProbability
{
    public int[] countries { get; set; }
    public Dictionary<string,double> probabilities { get; set; }
}
