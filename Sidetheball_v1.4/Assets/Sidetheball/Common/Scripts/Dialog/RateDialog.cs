using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RateDialog : Dialog {

	public void OnYesClick()
    {
        CUtils.RateGame();
        CFirebase.LogEvent("rate_dialog", "rate");
    }

    public void OnNoClick()
    {
        Close();
        CFirebase.LogEvent("rate_dialog", "later");
    }
}
