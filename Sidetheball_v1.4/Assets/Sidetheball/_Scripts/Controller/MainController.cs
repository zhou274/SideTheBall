using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainController : BaseController
{
    public static MainController instance;
    public bool isComplete;

    protected override void Awake()
    {
        base.Awake();
        instance = this;
    }

    protected override void Start()
    {
        base.Start();

        //Advertisements.Instance.ShowBanner(BannerPosition.BOTTOM, BannerType.Banner);

        CUtils.ShowBannerAd();

        var level = dotmob.Utils.GetLevel( Prefs.currentWorld, Prefs.currentLevel);
        Board.instance.LoadLevel(level);

        Prefs.continuePlayMode = Prefs.currentMode;
        Prefs.continuePlayWorld = Prefs.currentWorld;
        Prefs.continuePlayLevel = Prefs.currentLevel;

        dotmob.Utils.SetMusic();
    }

    public void OnComplete()
    {
        isComplete = true;

       // CUtils.ShowInterstitialAd();

        if (Prefs.currentLevel == Prefs.unlockedLevel)
        {
            Prefs.unlockedLevel++;
            if (Prefs.currentLevel == Const.NUMLEVEL - 1)
            {
                int nextWorld = Prefs.currentWorld + 1;
                Prefs.UnlockWorld(Prefs.currentMode, nextWorld);
            }
        }

        Prefs.continuePlayMode = Prefs.currentMode;
        if (Prefs.currentLevel == Const.NUMLEVEL - 1)
        {
            Prefs.continuePlayWorld = Prefs.currentWorld + 1;
            Prefs.continuePlayLevel = 0;
        }
        else
        {
            Prefs.continuePlayWorld = Prefs.currentWorld;
            Prefs.continuePlayLevel = Prefs.currentLevel + 1;
        }
    }

    public void OnBallToGoal()
    {
        int ran = UnityEngine.Random.Range(0, 3);
        Debug.Log("random =" + ran);
         

        Timer.Schedule(this, 0.5f, () =>
        {
            if (ran == 0 || ran == 2) { CUtils.ShowInterstitialAd(); }
              
            DialogController.instance.ShowDialog(DialogType.Win);
            Sound.instance.Play(Sound.Others.Win);
        });
    
    }
}
