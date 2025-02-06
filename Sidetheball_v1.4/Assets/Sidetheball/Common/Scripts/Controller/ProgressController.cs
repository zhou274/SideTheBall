using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System;

[System.Serializable]
public class QueryData
{
    public List<Progress> results;
}

[System.Serializable]
public class ProgressP
{
    public string userId;
    public int ruby;
    public int unlockedWorld;
    public int unlockedSubworld;
    public int unlockedLevel;
}

[System.Serializable]
public class Progress : ProgressP
{
    public string objectId;
}

public class ProgressController : MonoBehaviour {

    public string userID = null;
    public static ProgressController instance;

    private Dictionary<string, string> headers = new Dictionary<string, string>();

    public Action onProgressDownloaded;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
       // CurrencyController.onBalanceChanged += UploadProgress;

        

    
    }

   

  


  

    private int CompareLevel(Progress progress)
    {
        //int upWorld = progress.unlockedWorld;
        //int upSubworld = progress.unlockedSubworld;
        //int upLevel = progress.unlockedLevel;

        //int world = Prefs.unlockedWorld;
        //int subworld = Prefs.unlockedSubWorld;
        //int level = Prefs.unlockedLevel;


        //if (upWorld == world && upSubworld == subworld && upLevel == level) return 0;
        //if (upWorld > world || upWorld == world && upSubworld > subworld || upWorld == world && upSubworld == subworld && upLevel > level)
        //{
        //    return 1;
        //}
        return -1;
    }

    private bool ShouldDownload(Progress progress)
    {
        int upRuby = progress.ruby;
        int compare = CompareLevel(progress);
        return compare == 1 || compare == 0 && upRuby > CurrencyController.GetBalance();
    }

    private bool ShouldUpload(Progress progress)
    {
        int upRuby = progress.ruby;
        int compare = CompareLevel(progress);
        return compare == -1 || compare == 0 && upRuby != CurrencyController.GetBalance();
    }
}
