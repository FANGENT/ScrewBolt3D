using UnityEngine;
using UnityEngine.UI;
public class DialogBox : MonoBehaviour
{
    [SerializeField]
    private Text heading, description;

    [SerializeField]
    private Image displayImage;

    [SerializeField]
    private Button positiveButton, negativeButton;

    public bool showDisplayImage = true;
    public bool autoDestruct = false;
    public int destructTime = 2;
    public RectTransform dialog;
    // Use this for initialization
    void OnEnable()
    {
        positiveButton.onClick.AddListener(() => { OnPressingPositiveButton(); });
        negativeButton.onClick.AddListener(() => { OnPressingNegativeButton(); });


        AdsManager.Instance.onRewardedVideoResult += Instance_onRewardedVideoResult;
    }

    public void SetDialogBoxDetails(string _dialogName, string _heading, string _description, string _positiveButtonName, string _negativeButtonName = "", Sprite _displayImage = null)
    {
        gameObject.name = _dialogName;
        heading.text = _heading;
        description.text = _description;
        positiveButton.GetComponentInChildren<Text>().text = _positiveButtonName;
        negativeButton.GetComponentInChildren<Text>().text = _negativeButtonName;

        if (showDisplayImage)
        {
            if (_displayImage != null)
            {
                displayImage.overrideSprite = _displayImage;
                positiveButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(positiveButton.GetComponent<RectTransform>().anchoredPosition.x, positiveButton.GetComponent<RectTransform>().anchoredPosition.y, 0);
                negativeButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(negativeButton.GetComponent<RectTransform>().anchoredPosition.x, negativeButton.GetComponent<RectTransform>().anchoredPosition.y, 0);
                displayImage.SetNativeSize();
            }
            else
            {
                description.rectTransform.anchoredPosition = new Vector3(0, description.rectTransform.anchoredPosition.y, 0);
                displayImage.gameObject.SetActive(false);
            }
        }
        else
        {
            description.rectTransform.anchoredPosition = new Vector3(0, description.rectTransform.anchoredPosition.y, 0);
            //positiveButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(0 - negativeButton.GetComponent<RectTransform>().anchoredPosition.x / 2, positiveButton.GetComponent<RectTransform>().anchoredPosition.y, 0);
            //negativeButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(negativeButton.GetComponent<RectTransform>().anchoredPosition.x / 2, negativeButton.GetComponent<RectTransform>().anchoredPosition.y, 0);
            displayImage.gameObject.SetActive(false);
        }

        if (autoDestruct)
        {
            positiveButton.gameObject.SetActive(false);
            negativeButton.gameObject.SetActive(false);
        }
        dialog.anchoredPosition = new Vector2(0, Screen.height);

        LeanTween.move(dialog, Vector2.zero, 0.5f).setEase(LeanTweenType.easeOutBack);


        if (autoDestruct)
        {
            Invoke("CloseDialog", destructTime);
        }
    }

    void OnPressingPositiveButton()
    {
        switch (gameObject.name)
        {
            case "RateUs":
                {
                    Application.OpenURL("https://play.google.com/store/apps/details?id" + Application.identifier);
                    if (AdsManager.Instance)
                    {
                        AdsManager.Instance.SetEvent("RateUs Yes Presssed");
                        PlayerPrefs.SetString("RateUsPressed", "true");
                    }
                    CloseDialog();
                    break;
                }
            case "RemoveAds":
                {
                    if (AdsManager.Instance)
                    {
                     //   AdsManager.Instance.PurchaseProduct("remove_ads");
                        AdsManager.Instance.SetEvent("RemoveAds Yes Presssed");
                    }
                    CloseDialog();
                    break;
                }
            case "FreeHealthKits":
                {
                    //if (AdsManager.Instance && AdsManager.Instance.HasRewardedVideo())
                    //{
                    //    AdsManager.Instance.ShowRewardedVideo(RewardType.healthkit, 1);
                    //    AdsManager.Instance.SetEvent("FreeHealthKits watch video Presssed");
                    //}
                    //else
                    //{
                    //    if (AdsManager.Instance)
                    //    {
                    //        AdsManager.Instance.ShowDialogBox("VideoNotAvailable", "Video Not Available", "Try again is a few seconds or check your internet connection", "", "", UIElementsContainer.Instance.videoNotAvailable);
                    //    }
                    //    CloseDialog();
                    //}
                    break;
                }
            case "VideoNotAvailable":
                {
                    CloseDialog();
                    break;
                }
            case "FreeGrenades":
                {
                    //    if (AdsManager.Instance && AdsManager.Instance.HasRewardedVideo())
                    //    {
                    //        AdsManager.Instance.ShowRewardedVideo(RewardType.grenade, 2);
                    //        AdsManager.Instance.SetEvent("grenade watch video Presssed");
                    //    }
                    //    else
                    //    {
                    //        if (AdsManager.Instance)
                    //        {
                    //            AdsManager.Instance.ShowDialogBox("VideoNotAvailable", "Video Not Available", "Try again is a few seconds or check your internet connection", "", "", UIElementsContainer.Instance.videoNotAvailable);
                    //        }
                    //        CloseDialog();
                    //    }
                    break;
                }
            case "Consent":
                {
                    CloseDialog();
                    break;
                }
        }
    }

    private void Instance_onRewardedVideoResult(RewardType rewardType, float RewardAmount)
    {
        //if (rewardType == RewardType.healthkit)
        //{
        //    UIController.Instance.SetTimeScale(1);
        //    PlayerPrefs.SetInt("HealthKits", PlayerPrefs.GetInt("HealthKits", 1) + (int)RewardAmount);
        //    UIController.Instance.UpdateHealthKits();
        //    AdsManager.Instance.ShowDialogBox("FreeHealthAdded", "Health Kit Added", "Use health kits when you are low on health");
        //    CloseDialog();
        //}

    }

    void OnPressingNegativeButton()
    {
        switch (gameObject.name)
        {
            case "RateUs":
                {
                    CloseDialog();
                    AdsManager.Instance.SetEvent("RateUs No Presssed");
                    break;
                }
            case "RemoveAds":
                {
                    CloseDialog();
                    AdsManager.Instance.SetEvent("RemoveAds No Presssed");
                    break;
                }
            case "FreeHealthKits":
                {
                    CloseDialog();
                    break;
                }
            case "FreeGrenades":
                {
                    CloseDialog();
                    break;
                }
            case "VideoNotAvailable":
                {
                    CloseDialog();
                    break;
                }
            case "Consent":
                {
                    CloseDialog();
                    break;
                }

        }
    }
    private void Instance_onPurchaseSuccess(string sku)
    {
        if (sku == "remove_ads")
        {
            PlayerPrefs.SetString("RemoveAds", "true");
            AdsManager.Instance.HideBanner();
            AdsManager.Instance.ShowDialogBox("Purchased_RemoveAds", "Purchase Successful", "Enjoy Ad free Game! There will be no ads shown in game from now");
        }
    }
    void CloseDialog()
    {
        dialog.anchoredPosition = new Vector2(0, 0);
        LeanTween.move(dialog, new Vector2(0, Screen.height), 0.5f).setEase(LeanTweenType.easeOutBack);
        AdsManager.Instance.onRewardedVideoResult -= Instance_onRewardedVideoResult;
        Destroy(gameObject, 0.3f);
    }
}
