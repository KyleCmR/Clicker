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
    public Text[] rewardText = new Text[7];
    public Text[] currencyText = new Text[7];
    public Text[] startText = new Text[7];

    public BigDouble[] reward = new BigDouble[7];
    public BigDouble[] currencies = new BigDouble[7];

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

    }
}
