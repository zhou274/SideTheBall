﻿using UnityEngine;
using System;

[System.Serializable]
public class GameConfig
{
    public Admob admob;

    [Header("")]
    public int interstitialAdPeriod;
    public int rewardedVideoPeriod;
    public int rewardedVideoAmount;
    public string androidPackageID;
    public string iosAppID;
    public string facebookPageID;
  
}

[System.Serializable]
public class Admob
{
    [Header("Interstitial")]
    public string androidInterstitial;
    public string iosInterstitial;
    [Header("Banner")]
    public string androidBanner;
    public string iosBanner;
    [Header("RewardedVideo")]
    public string androidRewarded;
    public string iosRewarded;
}
