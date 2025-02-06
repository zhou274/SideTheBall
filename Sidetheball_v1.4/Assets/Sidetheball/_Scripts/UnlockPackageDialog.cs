using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if IAP && UNITY_PURCHASING
using UnityEngine.Purchasing;
#endif

public class UnlockPackageDialog : Dialog
{
    public int worldIndex;
    private bool usedUnlockCoin;

    protected override void Start()
    {
        base.Start();
       // Purchaser.instance.onItemPurchased += OnItemPurchased;
    }

    #if IAP && UNITY_PURCHASING
    public void OnUnlock()
    {
        Sound.instance.PlayButton();
        Purchaser.instance.BuyProduct(5);
        CFirebase.LogEvent("iap", "item_clicked_" + 5);

        Close();
    }
#endif

    public void OnUnlockbyCoin()
    {
        Sound.instance.PlayButton();
        if ( CurrencyController.DebitBalance(500))
        {
            //usedUnlockCoin = true;
            Prefs.UnlockWorld(Prefs.currentMode, worldIndex);
            WorldController.instance.UpdateUI();
            Debug.Log("Unlock :" + worldIndex);
            Close();
        }
        else
        {
            //DialogController.instance.CloseDialog(DialogType.UnlockPackage);
            DialogController.instance.ShowDialog(DialogType.Shop, DialogShow.STACK);
        }
        
        
    }

//    private void OnItemPurchased(IAPItem item, int index)
//    {
//        // A consumable product has been purchased by this user.
//        if (item.productType == ProductType.Consumable)
//        {
//            Toast.instance.ShowMessage("Your purchase is successful");
//            CUtils.SetBuyItem();

//            Prefs.UnlockWorld(Prefs.currentMode, worldIndex);
//            //WorldController.instance.UpdateUI();

//#if !UNITY_EDITOR
//            CFirebase.LogEvent("iap", "item_purchased_" + index);
//#endif
//        }
//    }

    private void OnDestroy()
    {
     //   Purchaser.instance.onItemPurchased -= OnItemPurchased;
    }
}
