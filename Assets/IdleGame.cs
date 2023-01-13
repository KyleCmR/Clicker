using System;
using UnityEngine;
using UnityEngine.UI;
using BreakInfinity;
using static BreakInfinity.BigDouble;

[Serializable]
public class PlayerData
{
    // Main Currency Value
    public BigDouble coins;
    public BigDouble coinsClickValue;
    public BigDouble coinsPerSecond;
    public BigDouble clickUpgrade1Level;
    public BigDouble clickUpgrade2Level;
    public BigDouble clickUpgrade2Cost;
    public BigDouble productionUpgrade1Level;
    public BigDouble productionUpgrade1Cost;
    public BigDouble productionUpgrade2Level;
    public BigDouble productionUpgrade2Power;
    public BigDouble productionUpgrade2Cost;
    public BigDouble gems;
    public BigDouble gemBoost;
    public BigDouble gemsToGet;

    public PlayerData() 
    { 
         FullReset();
    }
    public void FullReset()
    {
        coins = 0;
        coinsPerSecond = 0;
        coinsClickValue = 1;
        productionUpgrade2Power = 00;
        productionUpgrade1Level = 0;
        productionUpgrade2Level = 0;
        clickUpgrade1Level = 0;
        clickUpgrade2Level = 0;
        gems = 0;
        gemBoost = 0;
        gemsToGet = 1;
    }
}

public class IdleGame  : MonoBehaviour
{
    public PlayerData data;

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

    public Image clickUpgrade1Bar;

    public CanvasGroup mainMenuGroup;
    public CanvasGroup upgradesGroup;
    public int tabSwitcher;

    public GameObject settings;

    public void Start()
    {
        Application.targetFrameRate = 60;

        CanvasGroupChanger(true, mainMenuGroup);
        CanvasGroupChanger(false, upgradesGroup);
        tabSwitcher = 0;

        SaveSystem.LoadPlayer(ref data);
    }

    public void CanvasGroupChanger(bool x, CanvasGroup y)
    {
        if (x)
        {
            y.alpha = 1;
            y.blocksRaycasts = true;
            y.interactable = true;
            return;
        }
        y.alpha = 0;
        y.interactable = false;
        y.blocksRaycasts = false;
    }

    public void Update()
    {
        // The Higer the > 1e15 is the longer it takes to ear peels
        data.gemsToGet = 150 * Sqrt(data.coins / 1e7);
        data.gemBoost = data.gems * 0.05 + 1; //0.02

        gemsToGetText.text = "Prestige:\n+" + Floor(data.gemsToGet).ToString("F0") + " Gems";
        gemsText.text = "Gems: " + Floor(data.gems).ToString("F0");
        gemsBoostText.text = data.gemBoost.ToString("F2") + "x boost";

        data.coinsPerSecond = (data.productionUpgrade1Level + (data.productionUpgrade2Power * data.productionUpgrade2Level)) * data.gemBoost;

        clickValueText.text = "Collect\n+" + NotationMethod(data.coinsClickValue, "F0") + " Money";
        coinsText.text = "$" + NotationMethod(data.coins, "F0");
        coinsPerSecText.text = "$" + NotationMethod(data.coinsPerSecond, "F0") + "/s";

        // Click Upgrade Exponant Starts

        string clickUpgrade1CostString;
        var clickUpgrade1Cost = 10 * Pow(1.07, data.clickUpgrade1Level);
        clickUpgrade1CostString = NotationMethod(clickUpgrade1Cost, "F0");

        string clickUpgrade1LevelString;
        clickUpgrade1LevelString = NotationMethod(data.clickUpgrade1Level, "F0");

        string clickUpgrade2CostString;
        var clickUpgrade2Cost = 25 * Pow(1.07, data.clickUpgrade2Level);
        clickUpgrade2CostString = NotationMethod(clickUpgrade2Cost, "F0");

        string clickUpgrade2LevelString;
        clickUpgrade2LevelString = NotationMethod(data.clickUpgrade2Level, "F0");


        clickUpgrade1Text.text = "Click UPG 1\nCost: " + clickUpgrade1CostString + " Money\nPower: +1 Click\nLevel: " + clickUpgrade1LevelString;

        clickUpgrade2Text.text = "Click UPG 2\nCost: " + clickUpgrade2CostString + " Money\nPower: +5 Click\nLevel: " + clickUpgrade2LevelString;

        clickUpgrade1MaxText.text = "Buy Max (" + BuyClickUpgrade1MaxCount() + ")";

        clickUpgrade2MaxText.text = "Buy Max (" + BuyClickUpgrade2MaxCount() + ")";

        // Click Upgrade Exponant Ends

        // Production Upgrade Exponant Starts

        string productionUpgrade1CostString;
        var productionUpgrade1Cost = 25 * Pow(1.07, data.productionUpgrade1Level);
        if (productionUpgrade1Cost > 1000)
        {
            var exponent = Floor(Log10(Abs(productionUpgrade1Cost)));
            var mantissa = productionUpgrade1Cost / Pow(10, exponent);
            productionUpgrade1CostString = mantissa.ToString("F2") + "e" + exponent;
        }
        else
            productionUpgrade1CostString = productionUpgrade1Cost.ToString("F0");

        string productionUpgrade1LevelString;
        if (data.productionUpgrade1Level > 1000)
        {
            var exponent = Floor(Log10(Abs(data.productionUpgrade1Level)));
            var mantissa = data.productionUpgrade1Level / Pow(10, exponent);
            productionUpgrade1LevelString = mantissa.ToString("F2") + "e" + exponent;
        }
        else
            productionUpgrade1LevelString = data.productionUpgrade1Level.ToString("F0");

        string productionUpgrade2CostString;
        var productionUpgrade2Cost = 250 * Pow(1.07, data.productionUpgrade1Level);
        if (productionUpgrade2Cost > 1000)
        {
            var exponent = Floor(Log10(Abs(productionUpgrade2Cost)));
            var mantissa = productionUpgrade2Cost / Pow(10, exponent);
            productionUpgrade2CostString = mantissa.ToString("F2") + "e" + exponent;
        }
        else
            productionUpgrade2CostString = productionUpgrade2Cost.ToString("F0");

        string productionUpgrade2LevelString;
        if (data.productionUpgrade1Level > 1000)
        {
            var exponent = Floor(Log10(Abs(data.productionUpgrade2Level)));
            var mantissa = data.productionUpgrade1Level / Pow(10, exponent);
            productionUpgrade2LevelString = mantissa.ToString("F2") + "e" + exponent;
        }
        else
            productionUpgrade2LevelString = data.productionUpgrade2Level.ToString("F0");

        productionUpgrade1Text.text = "Production UPG 1\nCost: " + productionUpgrade1CostString + " Money\nPower: +" + data.gemBoost.ToString("F2") + " coins/s\nLevel: " + productionUpgrade1LevelString;
        productionUpgrade2Text.text = "Production UPG 2\nCost: " + productionUpgrade2CostString + " Money\nPower: +" + (data.productionUpgrade2Power * data.gemBoost).ToString("F2") + " coins/s\nLevel: " + productionUpgrade2LevelString;

        productionUpgrade1MaxText.text = "Buy Max (" + BuyProductionUpgrade1MaxCount() + ")";
        productionUpgrade2MaxText.text = "Buy Max (" + BuyProductionUpgrade2MaxCount() + ")";

        // Production Upgrade Exponants End
        data.coins += data.coinsPerSecond * Time.deltaTime;

        SaveSystem.SavePlayer(data);
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
        if (data.coins > 1000)
        {
            data.coins = 0;
            data.coinsClickValue = 1;
            data.clickUpgrade2Cost = 100;
            data.productionUpgrade1Cost = 25;
            data.productionUpgrade2Cost = 250;
            data.productionUpgrade2Power = 5;

            data.productionUpgrade1Level = 0;
            data.productionUpgrade2Level = 0;
            data.clickUpgrade1Level = 0;
            data.clickUpgrade2Level = 0;

            data.gems += data.gemsToGet;
        }
    }
    // Кнопки
    public void Click()
    {
        data.coins += data.coinsClickValue;
    }

    public void BuyUpgrade(string upgradeID)
    {
        //p means production
        var pcost = 25 * Pow(1.07, data.productionUpgrade1Level);
        var pcost2 = 250 * Pow(1.07, data.productionUpgrade2Level);

        //normal cost is the click upgrade cost
        var cost1 = 10 * Pow(1.07, data.clickUpgrade1Level);
        var cost2 = 10 * Pow(1.07, data.clickUpgrade2Level);

        switch (upgradeID)
        {
            case "C1":
                if (data.coins >= cost1)
                {
                    data.clickUpgrade1Level++;
                    data.coins -= cost1;
                    data.coinsClickValue++;
                }
                break;
            case "C2":
                if (data.coins >= cost2)
                {
                    data.clickUpgrade2Level++;
                    data.coins -= cost2;
                    data.coinsClickValue += 5;
                }
                break;
            case "P1":
                if (data.coins >= pcost)
                {
                    data.productionUpgrade1Level++;
                    data.coins -= pcost;
                }
                break;
            case "P2":
                if (data.coins >= pcost2)
                {
                    data.productionUpgrade2Level++;
                    data.coins -= pcost2;
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
            data.clickUpgrade1Level += n;
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
            data.clickUpgrade2Level += n;
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
            data.productionUpgrade1Level += n;
            data.coins -= cost;
            data.coinsPerSecond += n;
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
            data.productionUpgrade2Level += n;
            data.coins -= cost;
            data.coinsPerSecond += n;
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
        switch(id)
        {
            case "upgrades":
                CanvasGroupChanger(false, mainMenuGroup);
                CanvasGroupChanger(true, upgradesGroup);
                break;
            case "main":
                CanvasGroupChanger(true, mainMenuGroup);
                CanvasGroupChanger(false, upgradesGroup);
                break;
        }
    }
    public void FullResed()
    {
        data.FullReset();
    }

    public void GoToSettings()
    {
        settings.gameObject.SetActive(true);
    }
    public void GoBackToSettings()
    {
        settings.gameObject.SetActive(false);
    }

    /*
    public double BuyClickUpgrade1MaxCount()
    {
        var b = 10;
        var c = coins;
        var r = 1.07;
        var k = clickUpgrade1Level;
        var n = Floor(Log(c * (r - 1) / (b * Pow(r, k)) + 1, r));

        return n;
    }
    public double BuyClickUpgrade2MaxCount()
    {
        exampleImage.gameObject.SetActive(true);
        var b = 10;
        var c = coins;
        var r = 1.07;
        var k = clickUpgrade1Level;
        var n = Floor(Log(c * (r - 1) / (b * Pow(r, k)) + 1, r));

        return n;
    }
    public void BuyUpgrade(string upgradeID)
    {
        switch (upgradeID)
        {
            case "C1":
                var cost1 = 10 * Pow(1.07, clickUpgrade1Level);
                if (coins >= cost1)
                {
                    clickUpgrade1Level++;
                    coins -= cost1;
                    coinsClickValue++;
                }
                break;
            case "C1MAX":
                var b = 10;
                var c = coins;
                var r = 1.07;
                var k = clickUpgrade1Level;
                var n = Floor(Log(c * (r - 1) / (b * Pow(r, k)) + 1, r));
                var cost2 = b * (Pow(r, k) * (Pow(r, n) - 1) / (r - 1));

                if (coins >= cost2)
                {
                    clickUpgrade1Level += (int)n;
                    coins -= cost2;
                    coinsClickValue += n;
                }
                break;
            case "C2":
                if (coins >= clickUpgrade2Cost)
                {
                    clickUpgrade2Level++;
                    coins -= clickUpgrade2Cost;
                    clickUpgrade2Cost *= 1.09;
                    coinsClickValue += 5;
                }
                break;
            case "P1":
                if (coins >= productionUpgrade1Cost)
                {
                    productionUpgrade1Level++;
                    coins -= productionUpgrade1Cost;
                    productionUpgrade1Cost *= 1.07;
                }
                break;
            case "P2":
                if (coins >= productionUpgrade2Cost)
                {
                    productionUpgrade2Level++;
                    coins -= productionUpgrade1Cost;
                    productionUpgrade2Cost *= 1.07;
                }
                break;
        }
        public void Load()
    {
        coins = Parse(PlayerPrefs.GetString("coins", "0"));
        coinsClickValue = Parse(PlayerPrefs.GetString("coinsClickValue", "1"));
        productionUpgrade2Power = Parse(PlayerPrefs.GetString("productionUpgrade2Power", "5"));

        gems = Parse(PlayerPrefs.GetString("gems", "0"));

        productionUpgrade1Level = Parse(PlayerPrefs.GetString("productionUpgrade1Level", "0"));
        productionUpgrade2Level = Parse(PlayerPrefs.GetString("productionUpgrade2Level", "0"));
        clickUpgrade1Level = Parse(PlayerPrefs.GetString("clickUpgrade1Level", "0"));
        clickUpgrade2Level = Parse(PlayerPrefs.GetString("clickUpgrade2Level", "0"));
    }
    public void Save()
    {
        PlayerPrefs.SetString("coins", coins.ToString());
        PlayerPrefs.SetString("coinsClickValue", coinsClickValue.ToString());
        PlayerPrefs.SetString("productionUpgrade2Power", productionUpgrade2Power.ToString());

        PlayerPrefs.SetString("gems", gems.ToString());

        PlayerPrefs.SetString("productionUpgrade1Level", productionUpgrade1Level.ToString());
        PlayerPrefs.SetString("productionUpgrade2Level", productionUpgrade2Level.ToString());
        PlayerPrefs.SetString("clickUpgrade1Level", clickUpgrade1Level.ToString());
        PlayerPrefs.SetString("clickUpgrade2Level", clickUpgrade2Level.ToString());

    }
    }*/

}
