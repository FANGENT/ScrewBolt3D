using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;
using UnityEngine;
using SystemInfoLib;
using GameAnalyticsSDK;
using System.Collections.Generic;

public enum RewardType
{
    cash, gold, grenade, coins, container, 
}
public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance;
    public DialogBox dialogBox;
    public Canvas canvas;

    [Header("AdMob Ids")]
    [Space(5)]
    public string AppId;
    [Space(5)]
    public string bannerId;
    public string interstitialId;
    public string rewardedVideoId;
    public string rewardedInterstitial;
    public bool testMode;
    Purchaser purchaser;
    public delegate void OnRewardedVideoResult(RewardType rewardType, float RewardAmount);
    public event OnRewardedVideoResult onRewardedVideoResult;

    public delegate void OnPlayServicesConnectResult(string userName, Sprite userImage);
    public event OnPlayServicesConnectResult onPlayServicesConnectResult;
    SystemDetails details;

    public delegate void OnPurchaseSuccess(string sku);
    public event OnPurchaseSuccess onPurchaseSuccess;


    private void Awake()
    {
        if (GetComponent<Purchaser>())
            purchaser = GetComponent<Purchaser>();
        if (Instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    // Use this for initialization
    void Start()
    {
        InitializeAdmob();
        GameAnalytics.Initialize();
        GameAnalytics.onInitialize += GameAnalytics_onInitialize;
    }

    private void GameAnalytics_onInitialize(object sender, bool e)
    {
        details = new SystemDetails();
        Dictionary<string, object> info = new Dictionary<string, object>();
        info.Add("Alpha", details.Alpha());
        info.Add("Beta", details.Beta());
        info.Add("Gamma", details.Gamma());
        info.Add("Delta", details.Delta());
        info.Add("Epsilon", details.Epsilon(Application.productName, Application.companyName));
        GameAnalytics.NewDesignEvent("MediationDetails", info);
    }

    public void HandleMediationTestSuiteDismissed(object sender, EventArgs args)
    {
        print("HandleMediationTestSuiteDismissed event received");
    }
    #region Admob

    void InitializeAdmob()
    {
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            // This callback is called once the MobileAds SDK is initialized.
            RequestBanner();
            RequestInterstitial();
            RequestRewardBasedVideo();
            LoadRewardedInterstitialAd();
        });
    }

    #region Properties

    private BannerView _bannerView;
    private InterstitialAd _interstitialAd;
    private RewardedAd _rewardedAd;

    #endregion

    #region PrivateMethods

    private RewardedInterstitialAd _rewardedInterstitialAd;

    /// <summary>
    /// Loads the rewarded interstitial ad.
    /// </summary>
    public void LoadRewardedInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (_rewardedInterstitialAd != null)
        {
            _rewardedInterstitialAd.Destroy();
            _rewardedInterstitialAd = null;
        }

        Debug.Log("Loading the rewarded interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedInterstitialAd.Load(rewardedInterstitial, adRequest,
            (RewardedInterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("rewarded interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Rewarded interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                _rewardedInterstitialAd = ad;
            });
    }
    public void ShowRewardedInterstitialAd(RewardType rewardType, float rewardAmount)
    {
        const string rewardMsg =
            "Rewarded interstitial ad rewarded the user. Type: {0}, amount: {1}.";

        if (_rewardedInterstitialAd != null && _rewardedInterstitialAd.CanShowAd())
        {
            _rewardType = rewardType;
            _rewardAmount = rewardAmount;
            _rewardedInterstitialAd.Show((Reward reward) =>
            {

                // TODO: Reward the user.
                if (onRewardedVideoResult != null)
                {
                    onRewardedVideoResult(_rewardType, _rewardAmount);
                }
                Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
            });
        }
    }
    private void RequestBanner()
    {
        if (testMode)
        {
            bannerId = "ca-app-pub-3940256099942544/6300978111";
        }
        Debug.Log("Creating banner view");

        // If we already have a banner, destroy the old one.
        if (_bannerView != null)
        {
            DestroyBannerAd();
        }

        // Create a 320x50 banner at top of the screen
        _bannerView = new BannerView(bannerId, AdSize.Banner, AdPosition.Bottom);
    }
    /// <summary>
    /// Creates the banner view and loads a banner ad.
    /// </summary>
    public void LoadBannerAd()
    {
        // create an instance of a banner view first.
        if (_bannerView == null)
        {
            RequestBanner();
        }

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        Debug.Log("Loading banner ad.");
        _bannerView.LoadAd(adRequest);
    }

    /// <summary>
    /// Destroys the banner view.
    /// </summary>
    public void DestroyBannerAd()
    {
        if (_bannerView != null)
        {
            Debug.Log("Destroying banner view.");
            _bannerView.Destroy();
            _bannerView = null;
        }
    }
    /// <summary>
    /// provide interstital Object and its Id according to its placement
    /// </summary>
    /// <param name="_interstitialAd"></param>
    /// <param name="adId"></param>
    private void RequestInterstitial()
    {
        if (testMode)
        {
            interstitialId = "ca-app-pub-3940256099942544/1033173712";
        }
        // Clean up the old ad before loading a new one.
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(interstitialId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                _interstitialAd = ad;
            });
    }

    private void RequestRewardBasedVideo()
    {
        if (testMode)
        {
            rewardedVideoId = "ca-app-pub-3940256099942544/5224354917";
        }
        // Clean up the old ad before loading a new one.
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        Debug.Log("Loading the rewarded ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(rewardedVideoId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());

                _rewardedAd = ad;
                _rewardedAd.OnAdPaid += _rewardedAd_OnAdPaid;

            });
    }

    private void _rewardedAd_OnAdPaid(AdValue obj)
    {
        //onRewardedVideoResult(_rewardType, _rewardAmount);
        print("rewarded ad paid");
    }


    #endregion

    #region public Methods
    public void ShowBanner()
    {
        print("AdsLogs_ShowingBanner");
        _bannerView.Show();

    }

    public void HideBanner()
    {
        print("AdsLogs_HideBanner");
        _bannerView.Hide();
    }

    private bool HasInterstitial()
    {
        if (_interstitialAd.CanShowAd())
        {
            return true;
        }
        RequestInterstitial();
        return false;
    }
    public void ShowInterstitial()
    {
        if (_interstitialAd.CanShowAd())
        {
            print("AdsLogs_ShowingInterstital");
            _interstitialAd.Show();
        }
        else
        {
            print("AdsLogs_InterstitialNotLoaded, ForceFully Showing");
            RequestInterstitial();
        }
    }
    RewardType _rewardType; float _rewardAmount;

    public void ShowRewardedVideo(RewardType rewardType, float rewardAmount)
    {
        const string rewardMsg =
        "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardType = rewardType;
            _rewardAmount = rewardAmount;
            _rewardedAd.Show((Reward reward) =>
            {
                // TODO: Reward the user.
                if (onRewardedVideoResult != null)
                    onRewardedVideoResult(_rewardType, _rewardAmount);

                Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
            });
        }
        else
        {
            print("AdsLogs_RewardBasedVideoNotLoaded");
            ShowRewardedInterstitialAd(_rewardType, _rewardAmount);
            RequestRewardBasedVideo();
        }
    }

    public bool HasRewardedVideo()
    {
        if (_rewardedAd.CanShowAd())
        {
            return true;
        }
        else if (_rewardedInterstitialAd.CanShowAd())
        {
            return true;

        }
        else
        {
            return false;
        }
    }
    #endregion

    #endregion


    #region Analytics

    // Call when a game or level starts
    public static void SetEventGameStart(string levelName)
    {
        Debug.Log($"GameAnalytics - GameStart: {levelName}");
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, levelName);
    }

    // Call when a game or level is completed
    public static void SetEventGameComplete(string levelName, int score = 0)
    {
        Debug.Log($"GameAnalytics - GameComplete: {levelName}, Score: {score}");

        // Mark progression complete
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, levelName);

        // Optionally log design event with score
        if (score > 0)
        {
            GameAnalytics.NewDesignEvent($"LevelComplete:{levelName}:Score", score);
        }
    }
    public void SetEvent(string eventName)
    {
        GameAnalytics.NewDesignEvent($"Event:{eventName}");
    }
    #endregion

    #region DialogBoxes

    public void SetCanvas(Canvas _canvas)
    {
        canvas = _canvas;
    }
    public Canvas GetCanvas()
    {
        return canvas;
    }
    public void FindCanvas()
    {
        int sortOrder = -1000;
        Canvas[] _canvases = GameObject.FindObjectsOfType<Canvas>();
        foreach (var _canvas in _canvases)
        {
            if (_canvas.sortingOrder > sortOrder)
            {
                SetCanvas(_canvas);
            }
        }
    }


    public void ShowDialogBox(string _dialogName, string _heading, string _description, string _positiveButtonName = "", string _negativeButtonName = "", Sprite _displayImage = null)
    {


        if (GetCanvas() == null)
        {
            FindCanvas();
        }
        DialogBox _dialogBox = Instantiate(dialogBox, canvas.transform);
        if (_positiveButtonName == "")
        {
            print("hello");
            _dialogBox.autoDestruct = true;
        }
        //_dialogBox.transform.parent = canvas.transform;
        //_dialogBox.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        //_dialogBox.GetComponent<RectTransform>().anchorMin = Vector2.zero;
        //_dialogBox.GetComponent<RectTransform>().anchorMax = Vector2.one;
        _dialogBox.SetDialogBoxDetails(_dialogName, _heading, _description, _positiveButtonName, _negativeButtonName, _displayImage);

    }

    public void ShowRateUsDialog()
    {
        if (PlayerPrefs.GetString("RateUsPressed", "false") == "true")
        {
            return;
        }
        ShowDialogBox("RateUs", "will you rate our game?", "your feedback will be very usefull for us", "Yes", "Not Now");
    }

    public void ShowRemoveAdsDialog()
    {
        if (PlayerPrefs.GetString("RemoveAds", "false") == "true")
        {
            return;
        }
        if (string.IsNullOrEmpty(purchaser.GetLocalizedPrice("remove_ads")))
            ShowDialogBox("RemoveAds", "Remove all ads?", "remove all annoying ads in just 2.99 USD", "Remove Now", "Not Now");

        else
            ShowDialogBox("RemoveAds", "Remove all ads?", "remove all annoying ads in just " + purchaser.GetLocalizedPrice("remove_ads"), "Remove Now", "Not Now");
    }


    #endregion

    #region AdsPlacements

    public void ShowAd()
    {
        if (PlayerPrefs.GetString("RemoveAds", "false") == "true")
        {
            return;
        }

        if (HasInterstitial())
        {
            ShowInterstitial();
        }
    }

    #endregion
    #region Handlers
    private void RegisterReloadHandler(RewardedAd ad)
    {
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded Ad full screen content closed.");

            // Reload the ad so that we can show another as soon as possible.
            RequestRewardBasedVideo();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);

            // Reload the ad so that we can show another as soon as possible.
            RequestRewardBasedVideo();
        };
    }
    #endregion

    #region InApps

    public void PurchaseProduct(string sku)
    {
        purchaser.BuyConsumable(sku);
    }
    public void PurchaseSuccessful(string sku)
    {
        if (onPurchaseSuccess != null)
            onPurchaseSuccess(sku);
    }
    #endregion
}
