using UnityEngine;
using UnityEngine.UI;

public class ScoutResultUI : MonoBehaviour
{

    public GameObject resultPanel;

    public Text nameText;
    public Text countryText;
    public Text ageText;
    public Text positionText;
    public Text overallText;
    public Text cardTypeText;

    public Text paceText;
    public Text shootingText;
    public Text passingText;
    public Text dribblingText;
    public Text defenseText;
    public Text physicalText;

    public Image faceImage;

    public Sprite[] youthFaces;
    public Sprite[] seniorFaces;

    private ScoutedPlayer lastPlayer;

    public void SetResult(ScoutedPlayer player)
    {

        lastPlayer = player;

        nameText.text = player.name;
        countryText.text = "Country ID: " + player.country_id;
        ageText.text = "Age: " + player.age;
        positionText.text = player.position;
        overallText.text = "OVR " + player.overall_rating;
        cardTypeText.text = player.card_type;

        paceText.text = player.stats.pace.ToString();
        shootingText.text = player.stats.shooting.ToString();
        passingText.text = player.stats.passing.ToString();
        dribblingText.text = player.stats.dribbling.ToString();
        defenseText.text = player.stats.defense.ToString();
        physicalText.text = player.stats.physical.ToString();

        SetFace(player);

        Show();

    }

    void SetFace(ScoutedPlayer player)
    {

        if(player.is_youth)
        {
            faceImage.sprite = youthFaces[
                Random.Range(0,youthFaces.Length)
            ];
        }
        else
        {
            faceImage.sprite = seniorFaces[
                Random.Range(0,seniorFaces.Length)
            ];
        }

    }

    public void Show()
    {
        resultPanel.SetActive(true);
    }

    public void Hide()
    {
        resultPanel.SetActive(false);
    }

}
