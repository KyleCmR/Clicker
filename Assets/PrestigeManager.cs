using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrestigeManager : MonoBehaviour
{
    public IdleGame game;
    public Canvas prestige;

    public Text[] costText = new Text[3];
    public Image[] costBars = new Image[3];
    public Image[] costBarsSmooth = new Image[3];

    public string[] costDesc = { "" };

    //данные об игроке
    public BigDouble[] costs;
    public int[] levels;

    public BigDouble cost1 => 5 * BigDouble.Pow(1.5, game.data.prestigeUlevel1);
    public BigDouble cost2 => 10 * BigDouble.Pow(1.5, game.data.prestigeUlevel2);
    public BigDouble cost3 => 100 * BigDouble.Pow(1.5, game.data.prestigeUlevel3);

    public BigDouble gemsTemp;

    public void StartPrestige()
    {
        costs = new BigDouble[3];
        levels = new int[3];
        costDesc = new[]{"Click is 50% more effective", "You gain 10% more coins per second", "Gems are +1.01x better"};
    }

    public void Run()
    {
        ArrayManager();
        UI();

        void UI()
        {
            if (!prestige.gameObject.activeSelf) return;
            for (int i = 0; i < costText.Length; i++)
            {
                costText[i].text = $"Level {levels[i]}\n{costDesc[i]}\nCost: {game.NotationMethod(costs[i], "F2")} coins";
                Methods.SmoothNumber(ref gemsTemp, game.data.gems);
                Methods.BigDoubleFill(game.data.gems, costs[i], ref costBars[i]);
                Methods.BigDoubleFill(gemsTemp, costs[i], ref costBarsSmooth[i]);
            }
        }
    }
    public void BuyUpgrade(int id)
    {
        var data = game.data;
        
        switch (id)
        {
            case 0:
                Buy(ref data.prestigeUlevel1);
                break;
            case 1:
                Buy(ref data.prestigeUlevel2);
                break;
            case 2:
                Buy(ref data.prestigeUlevel3);
                break;
        }

        void Buy(ref int level)
        {
            if (data.gems < costs[id]) return;
            data.gems -= costs[id];
            level++;
        }
    }

    public void ArrayManager()
    {
        var data = game.data;

        costs[0] = cost1;
        costs[1] = cost2;
        costs[2] = cost3;

        levels[0] = data.prestigeUlevel1;
        levels[1] = data.prestigeUlevel2;
        levels[2] = data.prestigeUlevel3;
    }
}
