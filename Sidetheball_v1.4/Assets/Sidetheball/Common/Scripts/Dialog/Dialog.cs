﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class Dialog : MonoBehaviour
{
    public Animator anim;
    public AnimationClip hidingAnimation;
    public Action<Dialog> onDialogOpened;
    public Action<Dialog> onDialogClosed;
    public Action onDialogCompleteClosed;
    public DialogType dialogType;
    public bool enableAd = true;
    public bool enableEscape = true;

    private AnimatorStateInfo info;
    private bool isShowing;

    protected virtual void Awake()
    {
        if (anim == null) anim = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        onDialogCompleteClosed += OnDialogCompleteClosed;
        GetComponent<Canvas>().worldCamera = Camera.main;
        CFirebase.LogEvent("dialog_show", GetType().Name);
    }

    private void Update()
    {
        if (enableEscape && Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
        if (anim != null && IsIdle())
        {
            isShowing = true;
            anim.SetTrigger("show");
            onDialogOpened(this);
        }

        if (enableAd)
        {
            //int ran = UnityEngine.Random.Range(0, 2);
            //if(ran == 0 || ran == 1)
           // CUtils.ShowInterstitialAd();
        }
    }

    public virtual void Close()
    {
        if (isShowing == false) return;
        isShowing = false;
        if (anim != null && IsIdle() && hidingAnimation != null)
        {
            anim.SetTrigger("hide");
            Timer.Schedule(this, hidingAnimation.length, DoClose);
        }
        else
        {
            DoClose();
        }
    }

    private void DoClose()
    {
        onDialogClosed(this);
        Destroy(gameObject);
        if (onDialogCompleteClosed != null) onDialogCompleteClosed();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        isShowing = false;
    }

    public bool IsIdle()
    {
        info = anim.GetCurrentAnimatorStateInfo(0);
        return info.IsName("Idle");
    }

    public bool IsShowing()
    {
        return isShowing;
    }

    public virtual void OnDialogCompleteClosed()
    {
        onDialogCompleteClosed -= OnDialogCompleteClosed;
    }
}
