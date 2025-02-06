using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using SimpleJSON;

public class BaseController : MonoBehaviour {
    public GameObject gameMaster;
    public string sceneName;
    public Music.Type music = Music.Type.None;
    protected int numofEnterScene;

    public Transform[] noAdTransforms;
    public Transform[] hasAdTransforms;

    protected virtual void Awake()
    {
        if (GameMaster.instance == null && gameMaster != null)
            Instantiate(gameMaster);
        
        //iTween.dimensionMode = CommonConst.ITWEEN_MODE;
        
        CPlayerPrefs.useRijndael(CommonConst.ENCRYPTION_PREFS);

        numofEnterScene = CUtils.IncreaseNumofEnterScene(sceneName);
    }

    protected virtual void Start()
    {
        CPlayerPrefs.Save();
        if (JobWorker.instance.onEnterScene != null)
        {
            JobWorker.instance.onEnterScene(sceneName);
        }

#if UNITY_WSA && !UNITY_EDITOR
        StartCoroutine(SavePrefs());
#endif
        Music.instance.Play(music);

#if UNITY_ANDROID || UNITY_IOS
        OnBannerChanged(AdmobController.instance.isBannerVisible);
        AdmobController.instance.onBannerChanged += OnBannerChanged;
#endif

        CFirebase.LogEvent("scene_view", sceneName);
    }

    private void OnBannerChanged(bool visible)
    {
        if (visible)
        {
            int i = 0;
            foreach (Transform tr in noAdTransforms)
            {
                tr.position = hasAdTransforms[i].position;
#if UNITY_IOS
				tr.position += Vector3.up * 0.1f;
#endif
                i++;
            }
        }
    }

    public virtual void OnApplicationPause(bool pause)
    {
        Debug.Log("On Application Pause");
        CPlayerPrefs.Save();
        if (pause == false)
        {
            Timer.Schedule(this, 0.5f, () =>
            {
               // CUtils.ShowInterstitialAd();
            });
        }
    }

    private IEnumerator SavePrefs()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);
            CPlayerPrefs.Save();
        }
    }

    private void OnDestroy()
    {
#if UNITY_ANDROID || UNITY_IOS
        AdmobController.instance.onBannerChanged -= OnBannerChanged;
#endif
    }
}
