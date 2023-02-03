using BreakInfinity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    public IdleGame game;

    public Text eventTokensText;

    public BigDouble eventTokenBoost => game.data.eventTokens / 100;
    public GameObject eventRewardPopUp;

    public GameObject[] events = new GameObject[7];
    public GameObject[] eventsUnlocked = new GameObject[7];
    public Text[] rewardText = new Text[7];
    public Text[] currencyText = new Text[7];
    public Text[] startText = new Text[7];
    public Text[] costText = new Text[7];
    
    public BigDouble[] reward = new BigDouble[7];
    public BigDouble[] currencies = new BigDouble[7];
    public BigDouble[] cost = new BigDouble[7];

    public int[] levels = new int[7];

    public bool eventActive;
    public int eventActiveID;

    public string DayOfTheWeek()
    {
        var dt = DateTime.Now;
        return dt.DayOfWeek.ToString();
    }

    public string previousDayChecked;

    private void Start()
    {
        eventActive = false;
        previousDayChecked = DayOfTheWeek();
    }
    private void Update()
    {
        var data = game.data;
        eventTokensText.text = $"Event Tokens: {game.NotationMethod(data.eventTokens, "F2")} ({game.NotationMethod(eventTokenBoost, "F2")})";
        reward[0] = BigDouble.Log10(currencies[0] + 1);
        reward[1] = BigDouble.Log10(currencies[1] / 5  + 1);

        for (int i = 0; i < 2; i++)
            cost[i] = 10 * BigDouble.Pow(1.15, levels[i]);

        if(previousDayChecked != DayOfTheWeek() & eventActive)
        {
            eventActiveID = 0;
            for (var i = 0; i < 2; i++)
                data.eventCooldown[i] = 0;
        }

        switch(DayOfTheWeek())
        {
            case "Monday":
                RunEventUI(1);
                break;
            case "Tuesday":
                RunEventUI(2);
                break;
        }

        previousDayChecked = DayOfTheWeek();

    }
    private void RunEventUI(int id)
    {
        var data = game.data;
        for (var i = 0; i < 2; i++)
            events[i].gameObject.SetActive(false);
        events[id].gameObject.SetActive(true);

        if (eventActiveID != id) return;
        eventsUnlocked[id].gameObject.SetActive(true);
        rewardText[id].text = $"+{reward[id]} Event Tokens";
        currencyText[id].text = $"{currencies[id]} Currency";
        costText[id].text = $"Cost: {cost[id]}";

        if(eventActiveID == 0)
        {
            var time = TimeSpan.FromSeconds(data.eventCooldown[id]);
            startText[id].text = data.eventCooldown[id] > 0 ? "" : "Start Event";
        }

    }
}
