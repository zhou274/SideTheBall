using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public enum PromoteType { QuitDialog, PopupDialog };
public enum RewardType { None, RemoveAds, Currency };

public class PromoteController : ConnectServer
{
    public List<Promote> promotes;

    public static PromoteController instance;

    //public string KeyPref
    //{
    //    get { return "promotes_" + versionAPI; }
    //}

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        promotes = GetPromotes();
        UpdatePromotion();

//#if UNITY_ANDROID || UNITY_EDITOR
//        string promoteUrl = rootUrl + versionAPI + "/promote.txt";
//        StartCoroutine(GetDataFromServer(promoteUrl, ApplyPromotion));
//#endif
    }

    public Promote GetPromote(PromoteType promoteType)
    {
        if (promotes == null) return null;
        var results = promotes.FindAll(x => x.type == promoteType && !CUtils.IsAppInstalled(x.package) && CUtils.IsCacheExists(x.featureUrl));
        if (results == null || results.Count == 0) return null;
        return CUtils.GetRandom(results.ToArray());
    }

    private List<string> GetPackages()
    {
        return promotes.Select(x => x.package).ToList();
    }

    public void OnApplicationPause(bool pause)
    {
        if (pause == false)
        {
            UpdatePromotion();
        }
    }

    private void UpdatePromotion()
    {
        if (promotes == null) return;

        var apps = promotes.FindAll(x => CUtils.IsAppInstalled(x.package) && x.rewardType == RewardType.RemoveAds);
        if (apps.Count == 0) CUtils.SetRemoveAds(false);

        apps = promotes.FindAll(x => !CUtils.IsAppInstalled(x.package) && x.rewardType == RewardType.RemoveAds && IsRewarded(x.package));
        foreach (var promote in apps)
        {
            CPlayerPrefs.SetBool(promote.package + "_rewarded", false);
        }

        var packages = GetInstalledApp();
        Reward(packages);
    }

    private List<string> GetInstalledApp()
    {
        return GetPackages().FindAll(x => CUtils.IsAppInstalled(x) && !IsRewarded(x));
    }

    private void Reward(List<string> packages)
    {
        foreach (string package in packages)
        {
            if (CPlayerPrefs.GetBool(package + "_clicked_install"))
            {
                Reward(package);
            }
        }
    }

    private bool IsRewarded(string package)
    {
        return CPlayerPrefs.GetBool(package + "_rewarded");
    }

    private void Reward(string package)
    {
        CFirebase.LogEvent("promote_install_app", package);

        CPlayerPrefs.SetBool(package + "_rewarded", true);
        Promote promote = promotes.Find(x => x.package == package);
        if (promote == null) return;

        switch (promote.rewardType)
        {
            case RewardType.RemoveAds:
                CUtils.SetRemoveAds(true);
                Toast.instance.ShowMessage(promote.rewardMessage);
                break;
            case RewardType.Currency:
                CurrencyController.CreditBalance(promote.rewardValue);
                Toast.instance.ShowMessage(promote.rewardMessage);
                break;
        }
    }

    private void CacheFeature()
    {
        if (promotes == null) return;
        foreach (Promote promote in promotes)
        {
            StartCoroutine(CUtils.CachePicture(promote.featureUrl, null));
        }
    }

    public void ApplyPromotion(string data)
    {
       // PlayerPrefs.SetString(KeyPref, data);

        promotes = GetPromotes(data);
        CacheFeature();
    }

    private List<Promote> GetPromotes(string data)
    {
        try
        {
            return JsonUtility.FromJson<Promotes>(data).promotes;
        }
        catch
        {
            return null;
        }
    }

    private List<Promote> GetPromotes()
    {
        //if (promotes != null) return promotes;

        //if (!PlayerPrefs.HasKey(KeyPref))
        //{
        //    return null;
        //}

        //string data = PlayerPrefs.GetString(KeyPref);
        //return GetPromotes(data);
        return null;
    }

    public void OnInstallClick(string package)
    {
        CPlayerPrefs.SetBool(package + "_clicked_install", true);
    }
}