using System;
using System.Collections.Generic;
using AppLovinMax; // MAX Unity plugin
using GameAnalyticsSDK;
using SystemInfoLib;
using UnityEngine;

public enum RewardType
{
    cash,
    gold,
    grenade,
    coins,
    container,
}

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance;
    public DialogBox dialogBox;
    public Canvas canvas;

    [Header("AppLovin MAX Ids")]
    [Space(5)]
    [Tooltip("MAX SDK Key")]
    public string AppId;

    [Space(5)]
    [Tooltip("MAX Banner Ad Unit Id")]
    public string bannerId;

    [Tooltip("MAX Interstitial Ad Unit Id")]
    public string interstitialId;

    [Tooltip("MAX Rewarded Ad Unit Id")]
    public string rewardedVideoId;

    [Tooltip("MAX Rewarded Interstitial Ad Unit Id")]
    public string rewardedInterstitial;

    [Tooltip(
        "Optional: used only to enable extra logging, does NOT change ad unit ids like AdMob test ids"
    )]
    public string tenjinSDKKey;
    public bool testMode;

    Purchaser purchaser;

    public delegate void OnRewardedVideoResult(RewardType rewardType, float RewardAmount);
    public event OnRewardedVideoResult onRewardedVideoResult;

    public delegate void OnPlayServicesConnectResult(string userName, Sprite userImage);
    public event OnPlayServicesConnectResult onPlayServicesConnectResult;
    SystemDetails details;

    public delegate void OnPurchaseSuccess(string sku);
    public event OnPurchaseSuccess onPurchaseSuccess;

    // Internal state for rewards
    private RewardType _rewardType;
    private float _rewardAmount;

    // Retry counters (optional, simple exponential backoff)
    private int interstitialRetryAttempt;
    private int rewardedRetryAttempt;
    private int rewardedInterstitialRetryAttempt;

    // Simple guard flags so we don't register callbacks multiple times
    private bool bannerCallbacksRegistered;
    private bool interstitialCallbacksRegistered;
    private bool rewardedCallbacksRegistered;
    private bool rewardedInterstitialCallbacksRegistered;
    private bool bannerCreated;

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

    void Start()
    {
        InitializeAdmob(); // now initializes MAX
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

    #region AppLovin MAX (replacing AdMob)

    void InitializeAdmob()
    {
        if (string.IsNullOrEmpty(AppId))
        {
            Debug.LogError("MAX SDK Key (AppId) is empty. Please assign it in AdsManager.");
        }

        if (testMode)
        {
            // Just more logs – does not create test ad ids like AdMob
            MaxSdk.SetVerboseLogging(true);
        }
        BaseTenjin instance = Tenjin.getInstance(tenjinSDKKey);
        string analyticsID = instance.GetAnalyticsInstallationId();
        MaxSdk.SetUserId(analyticsID);
        // Initialize MAX
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfig) =>
        {
            Debug.Log("MAX SDK Initialized");

            // After MAX is ready, set up and load all formats
            RequestBanner();
            RequestInterstitial();
            RequestRewardBasedVideo();
            LoadRewardedInterstitialAd();
        };

        MaxSdk.SetSdkKey(AppId);
        MaxSdk.InitializeSdk();
    }

    #region Banner

    private void RequestBanner()
    {
        if (bannerCallbacksRegistered)
            return;

        if (string.IsNullOrEmpty(bannerId))
        {
            Debug.LogWarning("MAX Banner adUnitId is empty.");
            return;
        }

        bannerCallbacksRegistered = true;

        // Attach callbacks
        MaxSdkCallbacks.Banner.OnAdLoadedEvent += (string adUnitId, MaxSdkBase.AdInfo adInfo) =>
        {
            Debug.Log("MAX Banner loaded: " + adUnitId);
        };

        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += (
            string adUnitId,
            MaxSdkBase.ErrorInfo errorInfo
        ) =>
        {
            Debug.LogWarning("MAX Banner failed to load: " + errorInfo.Message);
        };

        MaxSdkCallbacks.Banner.OnAdClickedEvent += (string adUnitId, MaxSdkBase.AdInfo adInfo) =>
        {
            Debug.Log("MAX Banner clicked: " + adUnitId);
        };

        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += (
            string adUnitId,
            MaxSdkBase.AdInfo adInfo
        ) => {
            // You can forward revenue info to analytics here if needed
            // Debug.Log($"MAX Banner revenue: {adInfo.Revenue}");
        };

        // Create banner at bottom (similar to AdMob AdPosition.Bottom)
        Debug.Log("Creating MAX banner");
        MaxSdk.CreateBanner(bannerId, MaxSdkBase.BannerPosition.BottomCenter);
        // Start hidden; you control visibility via ShowBanner / HideBanner
        MaxSdk.HideBanner(bannerId);

        bannerCreated = true;
    }

    /// <summary>
    /// In AdMob this explicitly loaded the banner.
    /// For MAX, creating the banner already begins loading, so here we just ensure it's created.
    /// </summary>
    public void LoadBannerAd()
    {
        if (!bannerCreated)
        {
            RequestBanner();
        }
        // No-op otherwise; banner auto-loads via MAX
    }

    public void DestroyBannerAd()
    {
        if (!string.IsNullOrEmpty(bannerId))
        {
            Debug.Log("Destroying MAX banner");
            MaxSdk.DestroyBanner(bannerId);
            bannerCreated = false;
            bannerCallbacksRegistered = false;
        }
    }

    #endregion

    #region Interstitial

    private void RequestInterstitial()
    {
        if (interstitialCallbacksRegistered)
            return;

        if (string.IsNullOrEmpty(interstitialId))
        {
            Debug.LogWarning("MAX Interstitial adUnitId is empty.");
            return;
        }

        interstitialCallbacksRegistered = true;

        // Attach callbacks
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialFailedToDisplayEvent;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialRevenuePaidEvent;

        Debug.Log("Loading MAX interstitial");
        MaxSdk.LoadInterstitial(interstitialId);
    }

    private void LoadInterstitial()
    {
        if (string.IsNullOrEmpty(interstitialId))
            return;

        MaxSdk.LoadInterstitial(interstitialId);
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("MAX Interstitial loaded");
        interstitialRetryAttempt = 0;
    }

    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        Debug.LogWarning("MAX Interstitial failed to load: " + errorInfo.Message);

        interstitialRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, interstitialRetryAttempt));
        Invoke(nameof(LoadInterstitial), (float)retryDelay);
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("MAX Interstitial displayed");
    }

    private void OnInterstitialFailedToDisplayEvent(
        string adUnitId,
        MaxSdkBase.ErrorInfo errorInfo,
        MaxSdkBase.AdInfo adInfo
    )
    {
        Debug.LogWarning("MAX Interstitial failed to display: " + errorInfo.Message);
        LoadInterstitial();
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("MAX Interstitial clicked");
    }

    private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("MAX Interstitial hidden, pre-loading next");
        LoadInterstitial();
    }

    private void OnInterstitialRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Use for ILRD if you want
        // Debug.Log($"MAX Interstitial revenue: {adInfo.Revenue}");
    }

    #endregion

    #region Rewarded

    private void RequestRewardBasedVideo()
    {
        if (rewardedCallbacksRegistered)
            return;

        if (string.IsNullOrEmpty(rewardedVideoId))
        {
            Debug.LogWarning("MAX Rewarded adUnitId is empty.");
            return;
        }

        rewardedCallbacksRegistered = true;

        // Attach callbacks
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

        Debug.Log("Loading MAX rewarded ad");
        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
        if (string.IsNullOrEmpty(rewardedVideoId))
            return;

        MaxSdk.LoadRewardedAd(rewardedVideoId);
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("MAX Rewarded loaded");
        rewardedRetryAttempt = 0;
    }

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        Debug.LogWarning("MAX Rewarded failed to load: " + errorInfo.Message);

        rewardedRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, rewardedRetryAttempt));
        Invoke(nameof(LoadRewardedAd), (float)retryDelay);
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("MAX Rewarded displayed");
    }

    private void OnRewardedAdFailedToDisplayEvent(
        string adUnitId,
        MaxSdkBase.ErrorInfo errorInfo,
        MaxSdkBase.AdInfo adInfo
    )
    {
        Debug.LogWarning("MAX Rewarded failed to display: " + errorInfo.Message);
        LoadRewardedAd();
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("MAX Rewarded clicked");
    }

    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("MAX Rewarded hidden, pre-loading next");
        LoadRewardedAd();
    }

    private void OnRewardedAdReceivedRewardEvent(
        string adUnitId,
        MaxSdk.Reward reward,
        MaxSdkBase.AdInfo adInfo
    )
    {
        Debug.Log($"MAX Rewarded user: {reward.Amount} {reward.Label}");

        if (onRewardedVideoResult != null)
            onRewardedVideoResult(_rewardType, _rewardAmount);
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Use for ILRD if you need
        // Debug.Log($"MAX Rewarded revenue: {adInfo.Revenue}");
    }

    #endregion

    #region Rewarded Interstitial

    /// <summary>
    /// Loads the rewarded interstitial ad.
    /// </summary>
    public void LoadRewardedInterstitialAd()
    {
        if (rewardedInterstitialCallbacksRegistered)
            return;

        if (string.IsNullOrEmpty(rewardedInterstitial))
        {
            Debug.LogWarning("MAX Rewarded Interstitial adUnitId is empty.");
            return;
        }

        rewardedInterstitialCallbacksRegistered = true;

        // Attach callbacks
        // MaxSdkCallbacks.RewardedInterstitial.OnAdLoadedEvent += OnRewardedInterstitialLoadedEvent;
        // MaxSdkCallbacks.RewardedInterstitial.OnAdLoadFailedEvent += OnRewardedInterstitialLoadFailedEvent;
        // MaxSdkCallbacks.RewardedInterstitial.OnAdDisplayedEvent += OnRewardedInterstitialDisplayedEvent;
        // MaxSdkCallbacks.RewardedInterstitial.OnAdClickedEvent += OnRewardedInterstitialClickedEvent;
        // MaxSdkCallbacks.RewardedInterstitial.OnAdHiddenEvent += OnRewardedInterstitialHiddenEvent;
        // MaxSdkCallbacks.RewardedInterstitial.OnAdDisplayFailedEvent += OnRewardedInterstitialFailedToDisplayEvent;
        // MaxSdkCallbacks.RewardedInterstitial.OnAdReceivedRewardEvent += OnRewardedInterstitialReceivedRewardEvent;
        // MaxSdkCallbacks.RewardedInterstitial.OnAdRevenuePaidEvent += OnRewardedInterstitialRevenuePaidEvent;

        Debug.Log("Loading MAX rewarded interstitial ad");
        LoadRewardedInterstitial();
    }

    private void LoadRewardedInterstitial()
    {
        if (string.IsNullOrEmpty(rewardedInterstitial))
            return;

        //MaxSdk.LoadRewardedInterstitialAd(rewardedInterstitial);
    }

    private void OnRewardedInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("MAX Rewarded Interstitial loaded");
        rewardedInterstitialRetryAttempt = 0;
    }

    private void OnRewardedInterstitialLoadFailedEvent(
        string adUnitId,
        MaxSdkBase.ErrorInfo errorInfo
    )
    {
        Debug.LogWarning("MAX Rewarded Interstitial failed to load: " + errorInfo.Message);

        rewardedInterstitialRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, rewardedInterstitialRetryAttempt));
        Invoke(nameof(LoadRewardedInterstitial), (float)retryDelay);
    }

    private void OnRewardedInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("MAX Rewarded Interstitial displayed");
    }

    private void OnRewardedInterstitialFailedToDisplayEvent(
        string adUnitId,
        MaxSdkBase.ErrorInfo errorInfo,
        MaxSdkBase.AdInfo adInfo
    )
    {
        Debug.LogWarning("MAX Rewarded Interstitial failed to display: " + errorInfo.Message);
        LoadRewardedInterstitial();
    }

    private void OnRewardedInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("MAX Rewarded Interstitial clicked");
    }

    private void OnRewardedInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("MAX Rewarded Interstitial hidden, pre-loading next");
        LoadRewardedInterstitial();
    }

    private void OnRewardedInterstitialReceivedRewardEvent(
        string adUnitId,
        MaxSdk.Reward reward,
        MaxSdkBase.AdInfo adInfo
    )
    {
        Debug.Log($"MAX Rewarded Interstitial user: {reward.Amount} {reward.Label}");

        if (onRewardedVideoResult != null)
            onRewardedVideoResult(_rewardType, _rewardAmount);
    }

    private void OnRewardedInterstitialRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // ILRD if needed
    }

    #endregion

    #region Public Methods (same interface as before)

    public void ShowBanner()
    {
        print("AdsLogs_ShowingBanner (MAX)");
        if (!bannerCreated)
        {
            RequestBanner();
        }

        if (!string.IsNullOrEmpty(bannerId))
        {
            MaxSdk.ShowBanner(bannerId);
        }
    }

    public void HideBanner()
    {
        print("AdsLogs_HideBanner (MAX)");
        if (!string.IsNullOrEmpty(bannerId))
        {
            MaxSdk.HideBanner(bannerId);
        }
    }

    private bool HasInterstitial()
    {
        if (!string.IsNullOrEmpty(interstitialId) && MaxSdk.IsInterstitialReady(interstitialId))
        {
            return true;
        }

        LoadInterstitial();
        return false;
    }

    public void ShowInterstitial()
    {
        if (!string.IsNullOrEmpty(interstitialId) && MaxSdk.IsInterstitialReady(interstitialId))
        {
            print("AdsLogs_ShowingInterstital (MAX)");
            MaxSdk.ShowInterstitial(interstitialId);
        }
        else
        {
            print("AdsLogs_InterstitialNotLoaded, loading with MAX");
            LoadInterstitial();
        }
    }

    public void ShowRewardedInterstitialAd(RewardType rewardType, float rewardAmount)
    {
        _rewardType = rewardType;
        _rewardAmount = rewardAmount;

        //         if (!string.IsNullOrEmpty(rewardedInterstitial)
        //             && MaxSdk.IsRewardedInterstitialAdReady(rewardedInterstitial)
        // )
        //         {
        //             Debug.Log("Showing MAX Rewarded Interstitial");
        //             MaxSdk.ShowRewardedInterstitialAd(rewardedInterstitial);
        //         }
        //         else
        //         {
        //             Debug.LogWarning("MAX Rewarded Interstitial not ready, loading now");
        //             LoadRewardedInterstitial();
        //         }
    }

    public void ShowRewardedVideo(RewardType rewardType, float rewardAmount)
    {
        _rewardType = rewardType;
        _rewardAmount = rewardAmount;

        if (!string.IsNullOrEmpty(rewardedVideoId) && MaxSdk.IsRewardedAdReady(rewardedVideoId))
        {
            print("AdsLogs_ShowingRewarded (MAX)");
            MaxSdk.ShowRewardedAd(rewardedVideoId);
        }
        else
        {
            print("AdsLogs_RewardedNotLoaded, trying RewardedInterstitial (MAX)");
            // fallback to rewarded interstitial as before
            ShowRewardedInterstitialAd(_rewardType, _rewardAmount);
            LoadRewardedAd();
        }
    }

    public bool HasRewardedVideo()
    {
        if (!string.IsNullOrEmpty(rewardedVideoId) && MaxSdk.IsRewardedAdReady(rewardedVideoId))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    #endregion // MAX region

    #region Analytics

    public static void SetEventGameStart(string levelName)
    {
        Debug.Log($"GameAnalytics - GameStart: {levelName}");
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, levelName);
    }

    public static void SetEventGameComplete(string levelName, int score = 0)
    {
        Debug.Log($"GameAnalytics - GameComplete: {levelName}, Score: {score}");

        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, levelName);

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

    public void ShowDialogBox(
        string _dialogName,
        string _heading,
        string _description,
        string _positiveButtonName = "",
        string _negativeButtonName = "",
        Sprite _displayImage = null
    )
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
        _dialogBox.SetDialogBoxDetails(
            _dialogName,
            _heading,
            _description,
            _positiveButtonName,
            _negativeButtonName,
            _displayImage
        );
    }

    public void ShowRateUsDialog()
    {
        if (PlayerPrefs.GetString("RateUsPressed", "false") == "true")
        {
            return;
        }
        ShowDialogBox(
            "RateUs",
            "will you rate our game?",
            "your feedback will be very usefull for us",
            "Yes",
            "Not Now"
        );
    }

    public void ShowRemoveAdsDialog()
    {
        if (PlayerPrefs.GetString("RemoveAds", "false") == "true")
        {
            return;
        }
        if (string.IsNullOrEmpty(purchaser.GetLocalizedPrice("remove_ads")))
            ShowDialogBox(
                "RemoveAds",
                "Remove all ads?",
                "remove all annoying ads in just 2.99 USD",
                "Remove Now",
                "Not Now"
            );
        else
            ShowDialogBox(
                "RemoveAds",
                "Remove all ads?",
                "remove all annoying ads in just " + purchaser.GetLocalizedPrice("remove_ads"),
                "Remove Now",
                "Not Now"
            );
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
