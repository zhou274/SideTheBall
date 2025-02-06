using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class HomeController : BaseController {

    private const int STAR = 2;
    

    public GameObject playButton;
    public TextMeshProUGUI txtPlay;

    [SerializeField] GameObject GDPR_Popup;


    protected override void Start()
    {
        Invoke("CheckForGDPR", 0.5f);
        base.Start();

        Advertisements.Instance.ShowBanner(BannerPosition.BOTTOM, BannerType.Banner);
        // playButton.transform.position = playButton.transform.position - Vector3.right * 5;
        //iTween.MoveBy(playButton, iTween.Hash("amount", Vector3.right * 5, "easetype", iTween.EaseType.easeOutBack, "time", 0.4f, "delay", 0.4f));
        if (Prefs.continuePlayWorld == 0 && Prefs.continuePlayLevel == 0)
        {
            txtPlay.text = "开始游戏";
        }
        else
        {
            txtPlay.text = "关卡包 " + (Prefs.continuePlayWorld + 1) + "\n" + "关卡 " + (Prefs.continuePlayLevel + 1);
            txtPlay.fontSize = 40;
        }
       
        dotmob.Utils.SetMusic();
    }


    //GDPR
    void CheckForGDPR()
    {
        if (Advertisements.Instance.UserConsentWasSet() == false)
        {
            //show gdpr popup
            GDPR_Popup.SetActive(true);
            //pause the game
            Time.timeScale = 0;
        }
    }

    //Popup events
    public void OnUserClickAccept()
    {
        Advertisements.Instance.SetUserConsent(true);
        //hide gdpr popup
        GDPR_Popup.SetActive(false);
        //play the game
        Time.timeScale = 1;
        Sound.instance.PlayButton();
    }

    public void OnUserClickCancel()
    {
        Advertisements.Instance.SetUserConsent(false);
        //hide gdpr popup
        GDPR_Popup.SetActive(false);
        //play the game
        Time.timeScale = 1;
        Sound.instance.PlayButton();
    }

    public void OnUserClickPrivacyPolicy()
    {
        Sound.instance.PlayButton();
        Application.OpenURL("https://privacyURL.com"); //your privacy url
    }

    public void OnClickMoreGame()
    {
        Sound.instance.PlayButton();
        Application.OpenURL("https://google.com");
    }


    public void OnClick(int index)
    {
        switch (index)
        {
            
            case STAR:
                Prefs.currentMode = "Star";
                CUtils.LoadScene(1, true);
                break;
          
        }
        Sound.instance.PlayButton();
    }

    public void OnPlayClick()
    {
        Prefs.currentMode = "Star";
        Prefs.currentWorld = Prefs.continuePlayWorld;
        Prefs.currentLevel = Prefs.continuePlayLevel;



        iTween.MoveBy(playButton, iTween.Hash("amount", Vector3.right * 5, "easetype", iTween.EaseType.easeInBack, "time", 0.4f));

        CUtils.LoadScene(3, true);
        Sound.instance.PlayButton();
    }
}
