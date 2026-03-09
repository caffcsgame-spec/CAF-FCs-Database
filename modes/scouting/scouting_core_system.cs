using System;
using System.Collections.Generic;
using UnityEngine;

public class ScoutingCoreSystem : MonoBehaviour
{

    [Header("Scout Progression")]
    public int scoutLevel = 1;
    public int scoutXP = 0;

    private Dictionary<int,int> levelXP = new Dictionary<int,int>()
    {
        {1,100},
        {2,250},
        {3,500},
        {4,900}
    };



    [Serializable]
    public class RatingRange
    {
        public int min;
        public int max;
    }



    [Serializable]
    public class CardProbability
    {
        public int Base;
        public int Common;
        public int Rare;
        public int Epic;
    }



    [Serializable]
    public class ScoutLevelData
    {
        public int level;
        public RatingRange ratingRange;
        public CardProbability probability;
    }



    private List<ScoutLevelData> scoutLevels = new List<ScoutLevelData>()
    {

        new ScoutLevelData{
        level=1,
        ratingRange=new RatingRange{min=60,max=75},
        probability=new CardProbability{Base=70,Common=25,Rare=5,Epic=0}
        },

        new ScoutLevelData{
        level=2,
        ratingRange=new RatingRange{min=65,max=80},
        probability=new CardProbability{Base=55,Common=30,Rare=13,Epic=2}
        },

        new ScoutLevelData{
        level=3,
        ratingRange=new RatingRange{min=70,max=85},
        probability=new CardProbability{Base=40,Common=35,Rare=20,Epic=5}
        },

        new ScoutLevelData{
        level=4,
        ratingRange=new RatingRange{min=75,max=90},
        probability=new CardProbability{Base=25,Common=40,Rare=28,Epic=7}
        },

        new ScoutLevelData{
        level=5,
        ratingRange=new RatingRange{min=80,max=95},
        probability=new CardProbability{Base=10,Common=45,Rare=35,Epic=10}
        }

    };



    public ScoutedPlayer StartScouting()
    {

        ScoutedPlayer player = GeneratePlayer();

        GainXP(50);

        return player;

    }



    void GainXP(int xp)
    {

        scoutXP += xp;

        if(levelXP.ContainsKey(scoutLevel))
        {
            if(scoutXP >= levelXP[scoutLevel])
            {
                scoutXP = 0;
                scoutLevel++;
                Debug.Log("Scout Level Up: " + scoutLevel);
            }
        }

    }



    ScoutedPlayer GeneratePlayer()
    {

        ScoutLevelData data = scoutLevels.Find(x=>x.level==scoutLevel);

        int overall = UnityEngine.Random.Range(
        data.ratingRange.min,
        data.ratingRange.max+1
        );

        string cardType = GetCardType(data.probability);

        string position = GetRandomPosition();

        int age = GenerateAge();

        bool isYouth = age <= 20;

        PlayerStats stats = GenerateStats(position,overall);

        ScoutedPlayer player = new ScoutedPlayer();

        player.player_id = UnityEngine.Random.Range(1000,999999);

        player.name = GenerateName();

        player.country_id = UnityEngine.Random.Range(1,55);

        player.age = age;

        player.position = position;

        player.jersey_number = UnityEngine.Random.Range(1,100);

        player.overall_rating = overall;

        player.card_type = cardType;

        player.card_id = GenerateCardID(cardType);

        player.stats = stats;

        player.is_youth = isYouth;

        player.face_image = SelectYouthImage(isYouth);

        return player;

    }



    int GenerateAge()
    {

        int roll = UnityEngine.Random.Range(1,101);

        if(roll <= 35)
        return UnityEngine.Random.Range(16,21);

        if(roll <= 70)
        return UnityEngine.Random.Range(21,26);

        if(roll <= 90)
        return UnityEngine.Random.Range(26,30);

        return UnityEngine.Random.Range(30,35);

    }



    string SelectYouthImage(bool youth)
    {

        if(youth)
        {

            string[] youthFaces =
            {
            "youth_face_01",
            "youth_face_02",
            "youth_face_03",
            "youth_face_04",
            "youth_face_05",
            "youth_face_06",
            "youth_face_07",
            "youth_face_08"
            };

            return youthFaces[UnityEngine.Random.Range(0,youthFaces.Length)];

        }

        else
        {

            string[] seniorFaces =
            {
            "face_01",
            "face_02",
            "face_03",
            "face_04",
            "face_05",
            "face_06",
            "face_07",
            "face_08"
            };

            return seniorFaces[UnityEngine.Random.Range(0,seniorFaces.Length)];

        }

    }



    string GetCardType(CardProbability p)
    {

        int roll = UnityEngine.Random.Range(1,101);

        if(roll <= p.Base) return "Base";

        if(roll <= p.Base + p.Common) return "Common";

        if(roll <= p.Base + p.Common + p.Rare) return "Rare";

        return "Epic";

    }



    string GetRandomPosition()
    {

        string[] pos =
        {"GK","CB","LB","RB","CDM","CM","CAM","LW","RW","ST"};

        return pos[UnityEngine.Random.Range(0,pos.Length)];

    }



    PlayerStats GenerateStats(string position,int overall)
    {

        PlayerStats s = new PlayerStats();

        int v = 6;

        if(position=="ST"||position=="LW"||position=="RW")
        {
            s.pace=Rand(overall+5,v);
            s.shooting=Rand(overall+4,v);
            s.passing=Rand(overall,v);
            s.dribbling=Rand(overall+4,v);
            s.defense=Rand(overall-20,v);
            s.physical=Rand(overall,v);
        }

        else if(position=="CM"||position=="CAM")
        {
            s.pace=Rand(overall,v);
            s.shooting=Rand(overall,v);
            s.passing=Rand(overall+5,v);
            s.dribbling=Rand(overall+3,v);
            s.defense=Rand(overall-5,v);
            s.physical=Rand(overall,v);
        }

        else if(position=="CDM")
        {
            s.pace=Rand(overall-2,v);
            s.shooting=Rand(overall-10,v);
            s.passing=Rand(overall+2,v);
            s.dribbling=Rand(overall-2,v);
            s.defense=Rand(overall+5,v);
            s.physical=Rand(overall+4,v);
        }

        else if(position=="CB"||position=="LB"||position=="RB")
        {
            s.pace=Rand(overall-2,v);
            s.shooting=Rand(overall-20,v);
            s.passing=Rand(overall-5,v);
            s.dribbling=Rand(overall-5,v);
            s.defense=Rand(overall+6,v);
            s.physical=Rand(overall+5,v);
        }

        else if(position=="GK")
        {
            s.pace=30;
            s.shooting=20;
            s.passing=Rand(overall-5,v);
            s.dribbling=20;
            s.defense=Rand(overall+5,v);
            s.physical=Rand(overall+3,v);
        }

        return s;

    }



    int Rand(int baseValue,int variance)
    {

        int value = UnityEngine.Random.Range(
        baseValue-variance,
        baseValue+variance
        );

        return Mathf.Clamp(value,20,99);

    }



    int GenerateCardID(string type)
    {

        if(type=="Epic")
        return UnityEngine.Random.Range(1000,1999);

        if(type=="Rare")
        return UnityEngine.Random.Range(2000,2999);

        if(type=="Common")
        return UnityEngine.Random.Range(3000,3999);

        return UnityEngine.Random.Range(4000,4999);

    }



    string GenerateName()
    {

        string[] first =
        {
        "Abdul","Samuel","David","Emmanuel","Mohamed",
        "Peter","Joseph","Paul","Kofi","Ali",
        "Youssouf","Sadio","Ibrahim","Bakari"
        };

        string[] last =
        {
        "Okoro","Mensah","Diallo","Kamara","Banda",
        "Mabiala","Ndlovu","Kone","Traore",
        "Sissoko","Abdallah","Balde"
        };

        return first[UnityEngine.Random.Range(0,first.Length)] +
        " " +
        last[UnityEngine.Random.Range(0,last.Length)];

    }

}



[Serializable]
public class PlayerStats
{
    public int pace;
    public int shooting;
    public int passing;
    public int dribbling;
    public int defense;
    public int physical;
}



[Serializable]
public class ScoutedPlayer
{
    public int player_id;
    public string name;
    public int country_id;
    public int age;
    public string position;
    public int jersey_number;
    public int overall_rating;
    public string card_type;
    public int card_id;
    public bool is_youth;
    public string face_image;
    public PlayerStats stats;
}
