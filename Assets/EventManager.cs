using BreakInfinity;
using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    public IdleGame game;

    public Text eventTokensText;
    public BigDouble eventTokenBoost => (game.data.eventTokens / 100) + 1;

    public GameObject eventRewardPopUp;
    public Text eventRewardText;

    public GameObject[] events = new GameObject[7];
    public GameObject[] eventsUnlocked = new GameObject[7];
    public Text[] rewardText = new Text[7];
    public Text[] currencyText = new Text[7];
    public Text[] startText = new Text[7];
    public Text[] costText = new Text[7];
    
    public BigDouble[] reward = new BigDouble[7];
    public BigDouble[] currencies = new BigDouble[7];
    public BigDouble[] costs = new BigDouble[7];

    public int[] levels = new int[7];

    public bool eventActive;

    private string DayOfTheWeek()
    {
        var dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        return dt.DayOfWeek.ToString();
    }

    public string previousDayChecked;

    public void StartEvents()
    {
        eventActive = false;
        previousDayChecked = DayOfTheWeek();
        if (game.data.eventActiveID != 0) return;
        game.data.eventActiveID = 0;
        game.data.eventCooldown = new float[7];
    }
    private void Update()
    {
        var data = game.data;
        eventTokensText.text = $"Event Tokens: {game.NotationMethod(data.eventTokens, "F2")} ({game.NotationMethod(eventTokenBoost, "F2")}x)";
        reward[0] = BigDouble.Log10(currencies[0] + 1);
        reward[1] = BigDouble.Log10(currencies[1] / 5  + 1);

        for (int i = 0; i < 2; i++)
            costs[i] = 10 * BigDouble.Pow(1.15, levels[i]);

        if(previousDayChecked != DayOfTheWeek() & eventActive)
        {
            data.eventActiveID = 0;
            for (var i = 0; i < 2; i++)
                data.eventCooldown[i] = 0;
        }

        switch (DayOfTheWeek())
        {
            case "Monday":
                if (game.eventsGroup.gameObject.activeSelf)
                    RunEventUI(0);
                break;
            case "Tuesday":
                if (game.eventsGroup.gameObject.activeSelf)
                    RunEventUI(1);
                RunEvent(1);
                break;
        }
        if (data.eventActiveID == 0 & game.data.eventCooldown[CurrentDay()] > 0)
            game.data.eventCooldown[CurrentDay()] -= Time.deltaTime;
        else if (data.eventActiveID != 0 & game.data.eventCooldown[CurrentDay()] > 0)
            game.data.eventCooldown[CurrentDay()] -= Time.deltaTime;
        else if (data.eventActiveID == 0 & game.data.eventCooldown[CurrentDay()] <= 0)
            CompleteEvent(CurrentDay());

        previousDayChecked = DayOfTheWeek();
    }
 
    public int CurrentDay()
    {
        switch (DayOfTheWeek())
        {
            case "Monday": return 0;
            case "Tuesday": return 1;
        }
        return 0;
    }

    public void Click(int id)
    {
        switch (id)
        {
            case 1:
                currencies[id] += 1 + levels[id];
                break;
            case 2:
                currencies[id] += 1;
                break;
        }
        currencies[id] += 1;
    }

    public void Buy(int id)
    {
        if (currencies[id] >= costs[id])
            currencies[id] -= costs[id];
            levels[id]++;
    }

    public void ToggleEvent(int id)
    {
        var id2 = id - 1;
        var data = game.data;
        DateTime now = DateTime.Now;

        if (data.eventActiveID == 0 & data.eventCooldown[id2] <= 0 & !(now.Hour == 23 & now.Minute >= 55)) // Start
        {
            data.eventActiveID = id;
            data.eventCooldown[id2] = 300;

            currencies[id2] = 0;
            levels[id2] = 0;
        }
        else if (now.Hour == 23 & now.Minute >= 55 & data.eventActiveID == 0) ;
        else if (data.eventCooldown[id2] > 0 & data.eventActiveID == 0);
        else // Exit
            CompleteEvent(id2);
    }

    private void CompleteEvent(int id)
    {
        var data = game.data;

        data.eventTokens *= reward[id];
        eventRewardText.text = $"+{reward[id]} Event Tokens";

        currencies[id] = 0;
        levels[id] = 0;
        data.eventActiveID = 0;
        data.eventCooldown[id] = 7200;

        eventRewardPopUp.gameObject.SetActive(true);
    }

    public void CloseEventReward()
    {
        eventRewardPopUp.gameObject.SetActive(false);
    }

    private void RunEventUI(int id)
    {
        
        var data = game.data;

        for (var i = 0; i < 2; i++)
            if (i == id)
                events[id].gameObject.SetActive(true);
            else
                events[i].gameObject.SetActive(false);

        if (data.eventActiveID == 0)
        {
            var time = TimeSpan.FromSeconds(data.eventCooldown[id]);
            startText[id].text = data.eventCooldown[id] > 0 ? time.ToString(@"hh\:mm\:ss") : "Start Event";
        }
        else
            startText[id].text = "Start Event";

        if (data.eventActiveID != id + 1) return;
        eventsUnlocked[id].gameObject.SetActive(true);
        rewardText[id].text = $"+{reward[id]} Event Tokens";
        currencyText[id].text = $"{currencies[id]} Currency";
        costText[id].text = $"Cost: {costs[id]}";

    }

    private void RunEvent(int id)
    {
        switch(id)
        {
            case 2:
                currencies[id] += levels[id] * Time.deltaTime;
                break;
        }
    }
}
