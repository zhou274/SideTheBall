using UnityEngine;
using System;
//using GoogleMobileAds;
//using GoogleMobileAds.Api;

public class AdmobController : MonoBehaviour
{
    

    private bool _isBannerVisible, retryLoadingBanner;

    public bool isBannerVisible
    {
        get { return _isBannerVisible; }
        set { _isBannerVisible = value; if (onBannerChanged != null) onBannerChanged(value); }
    }

    public static AdmobController instance;
    public Action<bool> onBannerChanged;

    private void Awake()
    {
        instance = this;
    }

    int npaValue = -1;
    //"npa"=Non Personalized Ads

    private void Start()
    {
        npaValue = PlayerPrefs.GetInt("npa", 0);

        Debug.Log("npa = " + npaValue.ToString());
        Advertisements.Instance.Initialize();

        Advertisements.Instance.ShowBanner(BannerPosition.BOTTOM, BannerType.Banner);

        if (!CUtils.IsBuyItem() && !CUtils.IsAdsRemoved())
        {
           
        }

       
    }

    

    

    

   

    

    // Returns an ad request with custom ad targeting.
    

    

    

    

    

   

  
}
