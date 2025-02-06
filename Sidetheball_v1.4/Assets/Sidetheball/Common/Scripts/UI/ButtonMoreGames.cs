using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonMoreGames : MyButton {

    public override void OnButtonClick()
    {
        base.OnButtonClick();
#if UNITY_ANDROID
        Application.OpenURL("https://www.google.com/");
#elif UNITY_IPHONE
        Application.OpenURL("https://www.google.com/");
#endif
    }
}
