using System;
using UnityEngine;
using UnityEngine.UI;
using BreakInfinity;
using static BreakInfinity.BigDouble;
using System.Collections.Generic;


[Serializable]
public class PlayerData
{
    // Main Currency Value
    public BigDouble coins;
    public BigDouble coinsCollected;
    public BigDouble coinsClickValue;
    public int clickUpgrade1Level;
    public int clickUpgrade2Level;
    //public int clickUpgrade2Cost;
    public int productionUpgrade1Level;
    //public BigDouble productionUpgrade1Cost;
    public int productionUpgrade2Level;
    public BigDouble productionUpgrade2Power;
    //public BigDouble productionUpgrade2Cost;
    public BigDouble gems;
    public BigDouble gemsToGet;
    public float achLevel1;
    public float achLevel2;

    //Events
    public BigDouble eventTokens;
    public float[] eventCooldown = new float[7];
    public int eventActiveID;

    #region Prestige
    public int prestigeUlevel1;
    public int prestigeUlevel2;
    public int prestigeUlevel3;
    #endregion


    public PlayerData() 
    { 
         FullReset();
    }
    public void FullReset()
    {
        coins = 0;
        coinsCollected = 0;
        coinsClickValue = 1;
        productionUpgrade2Power = 0;
        productionUpgrade1Level = 0;
        productionUpgrade2Level = 0;
        clickUpgrade1Level = 0;
        clickUpgrade2Level = 0;
        gems = 0;
        gemsToGet = 0;

        achLevel1= 0;
        achLevel2= 0;

        eventTokens = 0;
        for (var i = 0; i < eventCooldown.Length - 1; i++)
            eventCooldown[i] = 0;
        eventActiveID = 0;
    }
}

public class IdleGame : MonoBehaviour
{
    public PlayerData data;
    public EventManager events;
    public PrestigeManager prestige;

    public GameObject clickUpgrade1;
    public GameObject clickUpgrade2;
    public GameObject productionUpgrade1;
    public GameObject productionUpgrade2;

    // Texts
    public Text coinsText;
    public Text clickValueText;
    public Text coinsPerSecText;
    public Text clickUpgrade1Text;
    public Text clickUpgrade2Text;
    public Text clickUpgrade1MaxText;
    public Text clickUpgrade2MaxText;
    public Text productionUpgrade1MaxText;
    public Text productionUpgrade2MaxText;
    public Text productionUpgrade1Text;
    public Text productionUpgrade2Text;
    public Text gemsText;
    public Text gemsBoostText;
    public Text gemsToGetText;

    public Canvas mainMenuGroup;
    public Canvas upgradesGroup;
    public Canvas achievementsGroup;
    public Canvas eventsGroup;
    public int tabSwitcher;

    public GameObject settings;

    public Image clickUpgrade1Bar;
    public Image clickUpgrade1BarSmooth;
    public BigDouble clickUpgrade1BarTemp;


    //p means production
    private BigDouble pCost => 25 * Pow(1.07, data.productionUpgrade1Level);
    private BigDouble pCost2 => 250 * Pow(1.07, data.productionUpgrade2Level);

    //normal cost is the click upgrade cost
    private BigDouble clickCost1 => 10 * Pow(1.07, data.clickUpgrade1Level);
    public BigDouble coinsTemp;
    private BigDouble clickCost2 => 10 * Pow(1.07, data.clickUpgrade2Level);

    public GameObject achievementScreen;
    public List<Achievement> achievementList = new List<Achievement>();

    public void Start()
    {
        Application.targetFrameRate = 60;

        foreach (var x in achievementScreen.GetComponentsInChildren<Achievement>())
            achievementList.Add(x);

        //mainMenuGroup.gameObject.SetActive(true);
        //upgradesGroup.gameObject.SetActive(true);
        //prestige.presitge.gameObject.SetActive(false);
        
        tabSwitcher = 0;

        SaveSystem.LoadPlayer(ref data);
        events.StartEvents();
        prestige.StartPrestige();
    }


    public void Update()
    {
        RunAchievements();
        prestige.Run();

        Methods.SmoothNumber(ref coinsTemp, data.coins);
        Methods.BigDoubleFill(data.coins, clickCost1, ref clickUpgrade1Bar);
        Methods.BigDoubleFill(coinsTemp, clickCost1, ref clickUpgrade1BarSmooth);

        // The Higer the > 1e15 is the longer it takes to ear peels
        data.gemsToGet = 150 * Sqrt(data.coins / 1e7);

        coinsText.text = "$" + NotationMethod(data.coins, "F0");
        coinsPerSecText.text = "$" + NotationMethod(TotalCoinsPerSecond(), "F0") + "/s";

        gemsText.text = "Gems: " + Floor(data.gems).ToString("F0");
        gemsBoostText.text = TotalGemBoost().ToString("F2") + "x boost";

        if (mainMenuGroup.gameObject.activeSelf)
        {
            gemsToGetText.text = "Prestige:\n+" + Floor(data.gemsToGet).ToString("F0") + " Gems";
            clickValueText.text = "Collect\n+" + NotationMethod(TotalClickValue(), "F0") + " Money";
        }
        // Click Upgrade Exponant Starts
        if (upgradesGroup.gameObject.activeSelf)
        {
            var clickUpgrade1Cost = 10 * Pow(1.07, data.clickUpgrade1Level);
            var clickUpgrade2Cost = 25 * Pow(1.07, data.clickUpgrade2Level);
            var productionUpgrade1Cost = 25 * Pow(1.07, data.productionUpgrade1Level);
            var productionUpgrade2Cost = 250 * Pow(1.07, data.productionUpgrade1Level);

            var clickUpgrade1CostString = NotationMethod(clickUpgrade1Cost, "F0");
            var clickUpgrade1LevelString = NotationMethod(data.clickUpgrade1Level, "F0");
            var clickUpgrade2CostString = NotationMethod(clickUpgrade2Cost, "F0");
            var clickUpgrade2LevelString = NotationMethod(data.clickUpgrade2Level, "F0");

            var productionUpgrade1CostString = NotationMethod(productionUpgrade1Cost, "F2");
            var productionUpgrade1LevelString = NotationMethod(data.productionUpgrade1Level, "F2");
            var productionUpgrade2CostString = NotationMethod(productionUpgrade2Cost, "F2");
            var productionUpgrade2LevelString = NotationMethod(data.productionUpgrade2Level, "F2");

            clickUpgrade1Text.text = "Click UPG 1\nCost: " + clickUpgrade1CostString + " Money\nPower: +1 Click\nLevel: " + clickUpgrade1LevelString;
            clickUpgrade2Text.text = "Click UPG 2\nCost: " + clickUpgrade2CostString + " Money\nPower: +5 Click\nLevel: " + clickUpgrade2LevelString;
            clickUpgrade1MaxText.text = "Buy Max (" + BuyClickUpgrade1MaxCount() + ")";
            clickUpgrade2MaxText.text = "Buy Max (" + BuyClickUpgrade2MaxCount() + ")";

            productionUpgrade1Text.text = "Production UPG 1\nCost: " + productionUpgrade1CostString + " Money\nPower: +" + (TotalBoost() * Pow(1.1, prestige.levels[1])).ToString("F2") + " coins/s\nLevel: " + productionUpgrade1LevelString;
            productionUpgrade2Text.text = "Production UPG 2\nCost: " + productionUpgrade2CostString + " Money\nPower: +" + (data.productionUpgrade2Power * TotalBoost() * Pow(1.1, prestige.levels[1])).ToString("F2") + " coins/s\nLevel: " + productionUpgrade2LevelString;
            productionUpgrade1MaxText.text = BuyMaxFormat(BuyProductionUpgrade1MaxCount());
            productionUpgrade2MaxText.text = BuyMaxFormat(BuyProductionUpgrade2MaxCount());

            clickUpgrade1.gameObject.SetActive(data.coinsCollected >= 5);
            clickUpgrade2.gameObject.SetActive(data.coinsCollected >= 25);
            productionUpgrade1.gameObject.SetActive(data.coinsCollected >= 15);
            productionUpgrade2.gameObject.SetActive(data.coinsCollected >= 250);
        }

        string BuyMaxFormat(BigDouble x)
        {
            return $"Buy Max ({x})";
        }
        
        // Production Upgrade Exponants End
        data.coins += TotalCoinsPerSecond() * Time.deltaTime;
        data.coinsCollected += TotalCoinsPerSecond() * Time.deltaTime;

        saveTimer += Time.deltaTime;

        if (!(saveTimer >= 20)) return; // сохранения каждые 20 сек (данный метод должен находится в самом низу Update)
        SaveSystem.SavePlayer(data); // Система сохранения
        saveTimer = 0;
    }

    public float saveTimer;

    private static string[] AchievementString => new string[] { "Current Coins", "Total Coins Collected" };
    private BigDouble[] AchievementNumbers => new BigDouble[] { data.coins, data.coinsCollected };

    private void RunAchievements()
    {
        UpdateAchievement(AchievementString[0], AchievementNumbers[0], ref data.achLevel1, ref achievementList[0].fill, ref achievementList[0].title, ref achievementList[0].progress);
        UpdateAchievement(AchievementString[1], AchievementNumbers[1], ref data.achLevel2, ref achievementList[1].fill, ref achievementList[1].title, ref achievementList[1].progress);

    }

    private void UpdateAchievement(string name, BigDouble number, ref float level, ref Image fill, ref Text title, ref Text progress)
    {
        var cap = BigDouble.Pow(10, level);

        if (achievementsGroup.gameObject.activeSelf) 
        {
            title.text = $"{name}\n({level})";
            progress.text = $"{NotationMethod(number, "F2")} / {NotationMethod(cap, "F2")}";

            Methods.BigDoubleFill(number, cap, ref fill);
        }

        if (number < cap) return;
        BigDouble levels = 0;
        if (number / cap >= 1)
            levels = Floor(Log10(number / cap)) + 1;
        level += (float)levels;
    }

    public string NotationMethod(BigDouble x, string y)
    {
        if (x > 1000)
        {
            var exponent = Floor(Log10(Abs(x)));
            var mantissa = x / Pow(10, exponent);
            return mantissa.ToString("F2") + "e" + exponent;
        }
        return x.ToString(y);
    }

    // Престиж
    public void Prestige()
    {
        if (data.coins > 1000) return;
        {
            data.coins = 1;
            data.coinsClickValue = 1;
            data.productionUpgrade2Power = 5;

            data.productionUpgrade1Level = 0;
            data.productionUpgrade2Level = 0;
            data.clickUpgrade1Level = 0;
            data.clickUpgrade2Level = 0;

            data.gems += data.gemsToGet;
        }
    }
    private BigDouble TotalBoost()
    {
        BigDouble temp = TotalGemBoost();
        temp *= events.eventTokenBoost;
        return temp;
    }
    private BigDouble TotalGemBoost()
    {
        var temp = data.gems;

        temp *= 0.05 + prestige.levels[2] * 0.01;
        return temp + 1;
    }

    private BigDouble TotalCoinsPerSecond()
    {
        BigDouble temp = 0;
        temp += data.productionUpgrade1Level;
        temp += data.productionUpgrade2Power * data.productionUpgrade2Level;
        temp *= TotalBoost();
        temp *= events.eventTokenBoost;
        temp *= Pow(1.1, prestige.levels[1]);
        return temp;
    }
    private BigDouble TotalClickValue()
    {
        var temp = data.coinsClickValue;
        temp *= TotalBoost();
        temp *= events.eventTokenBoost;
        temp *= Pow(1.5, prestige.levels[0]);
        return temp;
    }

    // Кнопки
    public void Click()
    {
        data.coins += TotalClickValue();
        data.coinsCollected += TotalClickValue();
    }

    public void BuyUpgrade(string upgradeID)
    {
        switch (upgradeID)
        {
            case "C1":
                if (data.coins >= clickCost1)
                {
                    data.clickUpgrade1Level++;
                    data.coins -= clickCost1;
                    data.coinsClickValue++;
                }
                break;
            case "C2":
                if (data.coins >= clickCost2)
                {
                    data.clickUpgrade2Level++;
                    data.coins -= clickCost2;
                    data.coinsClickValue += 5;
                }
                break;
            case "P1":
                if (data.coins >= pCost)
                {
                    data.productionUpgrade1Level++;
                    data.coins -= pCost;
                }
                break;
            case "P2":
                if (data.coins >= pCost2)
                {
                    data.productionUpgrade2Level++;
                    data.coins -= pCost2;
                }
                break;
        }
    }

    // Buy Upgrade 1 Buy Max Method Below

    public void BuyClickUpgrade1Max()
    {
        var b = 10;
        var c = data.coins;
        var r = 1.07;
        var k = data.clickUpgrade1Level;
        var n = Floor(Log(c * (r - 1) / (b * Pow(r, k)) + 1, r));
        var cost = b * (Pow(r, k) * (Pow(r, n) - 1) / (r - 1));

        if (data.coins >= cost)
        {
            data.clickUpgrade1Level += (int)n;
            data.coins -= cost;
            data.coinsClickValue += n;
        }
    }

    public BigDouble BuyClickUpgrade1MaxCount()
    {
        var b = 10;
        var c = data.coins;
        var r = 1.07;
        var k = data.clickUpgrade1Level;
        var n = Floor(Log(c * (r - 1) / (b * Pow(r, k)) + 1, r));
        return n;
    }

    //Buy Upgrade 2 Buy Max Method Below

    public void BuyClickUpgrade2Max()
    {
        var b = 25;
        var c = data.coins;
        var r = 1.07;
        var k = data.clickUpgrade2Level;
        var n = Floor(Log(c * (r - 1) / (b * Pow(r, k)) + 1, r));
        var cost = b * (Pow(r, k) * (Pow(r, n) - 1) / (r - 1));

        if (data.coins >= cost)
        {
            data.clickUpgrade2Level += (int)n;
            data.coins -= cost;
            data.coinsClickValue += n * 5;
        }
    }
    public BigDouble BuyClickUpgrade2MaxCount()
    {
        var b = 25;
        var c = data.coins;
        var r = 1.07;
        var k = data.clickUpgrade2Level;
        var n = Floor(Log(c * (r - 1) / (b * Pow(r, k)) + 1, r));
        return n;
    }

    // Buy Production Upgrade 1 Buy Max Method Below
    public void BuyProductionUpgrade1Max()
    {
        var b = 25;
        var c = data.coins;
        var r = 1.07;
        var k = data.productionUpgrade1Level;
        var n = Floor(Log(c * (r - 1) / (b * Pow(r, k)) + 1, r));
        var cost = b * (Pow(r, k) * (Pow(r, n) - 1) / (r - 1));
        if (data.coins >= cost)
        {
            data.productionUpgrade1Level += (int)n;
            data.coins -= cost;
        }
    }
    public BigDouble BuyProductionUpgrade1MaxCount()
    {
        var b = 25;
        var c = data.coins;
        var r = 1.07;
        var k = data.productionUpgrade1Level;
        var n = Floor(Log(c * (r - 1) / (b * Pow(r, k)) + 1, r));
        return n;
    }

    public void BuyProductionUpgrade2Max()
    {
        var b = 250;
        var c = data.coins;
        var r = 1.07;
        var k = data.productionUpgrade2Level;
        var n = Floor(Log(c * (r - 1) / (b * Pow(r, k)) + 1, r));
        var cost = b * (Pow(r, k) * (Pow(r, n) - 1) / (r - 1));
        if (data.coins >= cost)
        {
            data.productionUpgrade2Level += (int)n;
            data.coins -= cost;
        }
    }
    public BigDouble BuyProductionUpgrade2MaxCount()
    {
        var b = 250;
        var c = data.coins;
        var r = 1.07;
        var k = data.productionUpgrade2Level;
        var n = Floor(Log(c * (r - 1) / (b * Pow(r, k)) + 1, r));
        return n;
    }
    public void ChangeTabs(string id)
    {
        DisableAll();
        switch (id)
        {
            case "upgrades":
                upgradesGroup.gameObject.SetActive(true);
                break;
            case "main":
                mainMenuGroup.gameObject.SetActive(true);
                break;
            case "achievements":
                achievementsGroup.gameObject.SetActive(true);
                break;
            case "events":
                eventsGroup.gameObject.SetActive(true);
                break;
            case "prestige":
                prestige.prestige.gameObject.SetActive(true);
                break;
        }
        void DisableAll()
        {
            mainMenuGroup.gameObject.SetActive(false);
            upgradesGroup.gameObject.SetActive(false);
            achievementsGroup.gameObject.SetActive(false);
            eventsGroup.gameObject.SetActive(false);
            prestige.prestige.gameObject.SetActive(false);
        }
    }
    public void FullReset()
    {
        data.FullReset();
        ChangeTabs("main");
    }

    public void GoToSettings()
    {
        settings.gameObject.SetActive(true);
    }
    public void GoBackToSettings()
    {
        settings.gameObject.SetActive(false);
    }
}

public class Methods : MonoBehaviour
{
    public static void CanvasGroupChanger(bool x, CanvasGroup y)
    {
        y.alpha = x ?1 : 0;
        y.interactable = x;
        y.blocksRaycasts = x;
    }

    public static void BigDoubleFill(BigDouble x, BigDouble y, ref Image fill)
    {
        float z;
        var a = x / y;
        if (a < 0.001)
            z = 0;
        else if (a > 10)
            z = 1;
        else
            z = (float)a.ToDouble();
        fill.fillAmount = z;
    }
    public static void SmoothNumber(ref BigDouble smooth, BigDouble actual)
    {
        if (smooth > actual & actual == 0)
            smooth -= (smooth - actual) / 10 + 0.1 * Time.deltaTime;
        else if (Floor(smooth) < actual)
            smooth += (actual - smooth) / 10 + 0.1 * Time.deltaTime;
        else if (Floor(smooth) > actual)
            smooth -= (smooth - actual) / 10 + 0.1 * Time.deltaTime;
        else
        {
            smooth = actual;
        }
    }
}