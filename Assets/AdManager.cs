using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.UI;

public class AdManager : MonoBehaviour
{
    public IdleGame game;
    public RewardedAd rewardedAd;
    public int timesFailed;
    public Button watchAd;
    public GameObject rewardPopUp;

    private void Start()
    {
#if UNITY_ANDROID
        string appID = "Test";
#elif UNITY_IPHONE
        string appID = "unexpected_platform";
#else
        string appID = "unexpected_platform";
#endif

        //MobileAds.Initialize(appID);
        rewardPopUp.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(rewardedAd.IsLoaded())
            watchAd.gameObject.SetActive(true);
        else
            watchAd.gameObject.SetActive(false);
    }
    public void CreateAndLoadRewardedAd()
    {
        rewardedAd = new RewardedAd("Test");
/*
        rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;

        AdRequest reques = new AdRequest.Builder().Build();
        rewardedAd.LoadAd(reques);*/
    }
    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
    {
        switch (timesFailed)
        {
            case 0:
                Invoke("CreateAndLoadRewardedAd()", 10);
                break;
            case 1:
                Invoke("CreateAndLoadRewardedAd()", 30);
                break;
            case 2:
                Invoke("CreateAndLoadRewardedAd()", 60);
                break;
            case 3:
                Invoke("CreateAndLoadRewardedAd()", 120);
                break;
            default:
                Invoke("CreateAndLoadRewardedAd()", 240);
                break;
        }
        timesFailed++;
    }
    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        CreateAndLoadRewardedAd();
    }
    public void HandleUserEarnedReward(object sender, AdErrorEventArgs args)
    {
        //reward 
        //game.data.coins += args.Amount;
        CreateAndLoadRewardedAd();
    }

    public void WatchAd()
    {
        if(rewardedAd.IsLoaded())
        {
            rewardedAd.Show();
            watchAd.gameObject.SetActive(false);
            rewardPopUp.gameObject.SetActive(true);
        }
    }

    public void CloseAdReward()
    {
        rewardPopUp.gameObject.SetActive(false);
    }
}
