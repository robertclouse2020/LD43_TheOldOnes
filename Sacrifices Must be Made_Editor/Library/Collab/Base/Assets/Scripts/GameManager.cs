using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{

    public CharacterPoolClass characterPool;
    public CultClass cult;

    public TextMeshProUGUI textCult;


    public int day;

    public GameObject scrollContextMenu;

    TownClass town;

    public Slider visSlider;
    public Slider beleiverSlider;
    public Slider nonBeleiversSlider;
    public Slider toWinSlider;

    public TextMeshProUGUI moneyText;

    const int DEFAULT_RECRUITMENT = 5;
    const int DEFAULT_SACRIFICE = 25;
    const int DEFAULT_VISIBILITY = 5;

    public GameObject SoundManager;

    [System.NonSerialized]
    public AudioSource sfxManager;
    [System.NonSerialized]
    public AudioSource UISFX;

    private void Awake()
    {
        day = 0;
        town = new TownClass();
        characterPool = new CharacterPoolClass();
        cult = new CultClass();


        SoundManager = GameObject.Find("SoundManager");
        sfxManager = SoundManager.transform.Find("SFX").GetComponent<AudioSource>();
        UISFX = SoundManager.transform.Find("UI").GetComponent<AudioSource>();
    }

    // Use this for initialization
    void Start()
    {


        moneyText.text = "Money: " +  cult.money.ToString("c2");
        

        foreach (CharacterClass c in characterPool.allCharacters)
        {
            //print(c.name);
        }

        textCult.text = cult.name;

        visSlider.value = town.visibility;
        nonBeleiversSlider.value = town.nonBeleivers;
        beleiverSlider.value = town.beleivers;

    }

    // Update is called once per frame
    void Update()
    {

        moneyText.text = "Money: " + cult.money.ToString("c2");

        if(town.visibility > 100)
        {
            Debug.Log("you lose");
        }


        //if town.


    }

    public IEnumerator NotEnoughCashAnim()
    {
        float timeElapsed = 0f;
        float totalTime = 1.0f;

        Color startColor = moneyText.color;
        Color endColor = Color.red;

        while (timeElapsed < totalTime)
        {
            timeElapsed += Time.deltaTime;
            if (timeElapsed > totalTime/2)
            {
                moneyText.color = Color.Lerp(moneyText.color, startColor, timeElapsed / totalTime);
            }
            else
            {
                moneyText.color = Color.Lerp(startColor, endColor, timeElapsed / totalTime);
            }
            yield return null;
        }

        yield return null;
    }

    public void nextDay()
    {
        day++;

        List<BuildingStats> buildingStats = new List<BuildingStats>();


        float totalSacrifices = DEFAULT_SACRIFICE;
        float totalVisiblity = DEFAULT_VISIBILITY;
        float totalRecrutiment = DEFAULT_RECRUITMENT;
        float cashGain = 0;

        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Building"))
        {
            buildingStats.Add(g.GetComponent<BuildingStats>());
        }

        foreach (BuildingStats b in buildingStats)
        {
            //mutliply all default stats by manager skill
            b.calcManagerImpact();

            //b.visibility 
            totalRecrutiment += b.recruitmentPercent;
            totalVisiblity += b.visibilityPercent;
            totalSacrifices += b.sacrificesAmount;
            cashGain += b.cashGain;
        }


        //recruit townies
        town.beleivers += totalRecrutiment;
        town.nonBeleivers -= totalRecrutiment;

        //kill off townies from sacrifice
        town.nonBeleivers -= totalSacrifices;

        //adjust for visibiity
        town.visibility += totalVisiblity;

        town.oldGodsPercent += Mathf.RoundToInt(totalSacrifices / 36);

        print("visibility: " + town.visibility);
        print("non beleivers: " + town.nonBeleivers);
        print("beleivers: " + town.beleivers);


        visSlider.value = town.visibility;
        nonBeleiversSlider.value = town.nonBeleivers;
        beleiverSlider.value = town.beleivers;
    }



}
