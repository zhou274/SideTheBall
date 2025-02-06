//using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardedVideoButton : MonoBehaviour
{
    private const string ACTION_NAME = "rewarded_video";

    private void Start()
    {

//#if UNITY_ANDROID || UNITY_IOS
//        if (AdmobController.instance.rewardBasedVideo != null)
//        {
//            AdmobController.instance.rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
//        }
//#endif
    }

    public void OnClick()
    {
        if (Advertisements.Instance.IsRewardVideoAvailable())
        {
            //AdmobController.instance.ShowRewardBasedVideo();
            Advertisements.Instance.ShowRewardedVideo(CompleteMethod);
        }
        else
        {
            Toast.instance.ShowMessage("Ad is not available now, please wait..");
        }
        
        Sound.instance.PlayButton();
    }

    private void CompleteMethod(bool completed, string advertiser)
    {
        Debug.Log("Closed rewarded from: " + advertiser + " -> Completed " + completed);
        if (completed == true)
        {
            int amount = ConfigController.Config.rewardedVideoAmount;
            CurrencyController.CreditBalance(amount);
            Toast.instance.ShowMessage("You've received " + amount + " coins", 3);
            CUtils.SetActionTime(ACTION_NAME);
           // CFirebase.LogEvent("rewarded_video", "on_rewarded");
        }
        else
        {
            Toast.instance.ShowMessage("Reward is not available now, please wait..");
        }
    }



    //public bool IsAvailableToShow()
    //{
    //    //return IsActionAvailable() && IsAdAvailable();
    //}

    private bool IsActionAvailable()
    {
        return CUtils.IsActionAvailable(ACTION_NAME, ConfigController.Config.rewardedVideoPeriod);
    }

    //private bool IsAdAvailable()
    //{
    //    if (AdmobController.instance.rewardBasedVideo == null) return false;
    //    bool isLoaded = AdmobController.instance.rewardBasedVideo.IsLoaded();
    //    return isLoaded;
    //}

  
}
