using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("------UI Panels------------")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject profilePanel;
    public GameObject enterNamePanel;
    public GameObject removeAdsPanel;
    public GameObject levelGiftPanel;
    public GameObject purchaseCancelPanel;
    public GameObject aboutPanel;
    /*public GameObject loadingScreen;
    public Slider loadingSlider;
    public GameObject shopPanel;
    
    public GameObject notEnoughCoinsPanel;
    public GameObject watchAdPanel;*/

    [Header("------Values------------")]
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI levelNumber;

    [Header("------Buttons------------")]
    public Button playBtn;
    public Button settingsBtn;
    public Button profileBtn;
    public Button removeAdsBtn;
    public Button levelGiftBtn;
    /*public Button watchAdCoinsBtn;
    public RectTransform startCoinPosition; // For the coin icon in the UI
    public List<Button> shopCoinsBtns = new List<Button>();*/

    [Header("------Coin Drop Effect------------")]
    public GameObject coinPrefab; // UI coin image prefab
    public RectTransform coinTarget; // e.g. Canvas or dedicated CoinLayer
    public RectTransform spawnPoint; // Where coins start (e.g. coin counter)
    


    private float screenWidth;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        screenWidth = Screen.width;
        AddAllListeners();

        if (SoundManager.Instance)
        {
            SoundManager.Instance.PlayMusic("Main Menu BGM");
        }

        DataManager.onCoinsUpdated += CoinsUpdateCallback;
        CoinsUpdateCallback(); // Initialize coin display
    }

    void AddAllListeners()
    {
        playBtn.onClick.AddListener(Play);
        settingsBtn.onClick.AddListener(OnSettingPressed);
        profileBtn.onClick.AddListener(OnProfilePressed);
        removeAdsBtn.onClick.AddListener(OnRemoveAds);
        levelGiftBtn.onClick.AddListener(OnLevelGiftPressed);
        //watchAdCoinsBtn.onClick.AddListener(WatchAdForCoins);
    }

    void Play()
    {
        if(SoundManager.Instance)
        {
            SoundManager.Instance.PlaySFX("Button Click");
        }
        SceneManager.LoadScene(2);
    }

    void OnSettingPressed()
    {
        if (SoundManager.Instance)
        {
            SoundManager.Instance.PlaySFX("Button Click");
        }

        settingsPanel.gameObject.SetActive(true);
        // Reset starting position to right side
        settingsPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(screenWidth, 0), 0f).SetEase(Ease.OutCubic);

        // Animate into view
        settingsPanel.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, 0.4f).SetEase(Ease.OutCubic);
    }

    public void OnSettingsClosed()
    {
        if (SoundManager.Instance)
        {
            SoundManager.Instance.PlaySFX("Button Click");
        }

        // Animate out of view to the right side
        settingsPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(screenWidth, 0), 0.4f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            settingsPanel.gameObject.SetActive(false);
        });
    }

    void OnProfilePressed()
    {
        if (SoundManager.Instance)
        {
            SoundManager.Instance.PlaySFX("Button Click");
        }

        if (PlayerPrefs.GetString("PlayerName") == "")
        {
            enterNamePanel.gameObject.SetActive(true);
            return;
        }
        profilePanel.gameObject.SetActive(true);
    }

    void OnRemoveAds()
    {
        if (SoundManager.Instance)
        {
            SoundManager.Instance.PlaySFX("Button Click");
        }

        removeAdsPanel.gameObject.SetActive(true);
        removeAdsPanel.transform.GetChild(0).localScale = Vector3.zero;
        removeAdsPanel.transform.GetChild(0).DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }
    public void OnRemoveAdsClosed()
    {
        if (SoundManager.Instance)
        {
            SoundManager.Instance.PlaySFX("Button Click");
        }
        removeAdsPanel.transform.GetChild(0).DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
        {
            removeAdsPanel.gameObject.SetActive(false);
        });
    }

    void OnLevelGiftPressed()
    {
        if (SoundManager.Instance)
        {
            SoundManager.Instance.PlaySFX("Button Click");
        }

        levelGiftPanel.gameObject.SetActive(true);
        levelGiftPanel.transform.GetChild(0).localScale = Vector3.zero;
        levelGiftPanel.transform.GetChild(0).DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }
    public void OnLevelGiftClosed()
    {
        if (SoundManager.Instance)
        {
            SoundManager.Instance.PlaySFX("Button Click");
        }
        levelGiftPanel.transform.GetChild(0).DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
        {
            levelGiftPanel.gameObject.SetActive(false);
        });


        TestCoinBuy();
    }

    public void OnAboutPressed()
    {
        if (SoundManager.Instance)
        {
            SoundManager.Instance.PlaySFX("Button Click");
        }
        aboutPanel.SetActive(true);
        // Reset starting position to right side
        aboutPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(screenWidth, 0), 0f).SetEase(Ease.OutCubic);

        // Animate into view
        aboutPanel.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, 0.4f).SetEase(Ease.OutCubic);
    }

    public void OnAboutClosed()
    {
        if (SoundManager.Instance)
        {
            SoundManager.Instance.PlaySFX("Button Click");
        }
        // Animate out of view to the right side
        aboutPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(screenWidth, 0), 0.4f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            aboutPanel.SetActive(false);
        });
    }

    #region Shop

    public void PurchaseShopItem(string itemID)
    {
        if (SoundManager.Instance)
        {
            SoundManager.Instance.PlaySFX("Button Click");
        }
        // Implement purchase logic here IAP

    }

    public void ShowPurchaseCancel()
    {
        purchaseCancelPanel.gameObject.SetActive(true);
        purchaseCancelPanel.transform.GetChild(0).localScale = Vector3.zero;
        purchaseCancelPanel.transform.GetChild(0).DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }
    public void OnPurchaseCancelClosed()
    {
        if (SoundManager.Instance)
        {
            SoundManager.Instance.PlaySFX("Button Click");
        }
        purchaseCancelPanel.transform.GetChild(0).DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
        {
            purchaseCancelPanel.gameObject.SetActive(false);
        });
    }

    #endregion

    #region Coin Animation

    private int displayedCoins = 0; // Keep track of the last shown value

    private void CoinsUpdateCallback()
    {
        int targetCoins = DataManager.instance.coins;

        DOTween.Kill("CoinsTween"); // Kill any previous coin tween

        DOTween
            .To(
                () => displayedCoins,
                x =>
                {
                    displayedCoins = x;
                    coinsText.text = x.ToString();
                },
                targetCoins,
                0.5f
            ) // 0.5 seconds animation duration
            .SetEase(Ease.OutQuad)
            .SetId("CoinsTween");
    }

    public void PlayCoinDrop(RectTransform targetButton, int coinCount = 10, float duration = 0.5f)
    {
        StartCoroutine(SpawnCoins(targetButton, coinCount, duration));
    }

    public void PlayCoinUp(RectTransform targetButton, int coinCount = 10, float duration = 0.5f)
    {
        StartCoroutine(SpawnCoinsUp(targetButton, coinCount, duration));
    }

    //For Buy effects, coins will drop from the coin icon to the target button
    private IEnumerator SpawnCoins(RectTransform target, int count, float duration)
    {
        for (int i = 0; i < count; i++)
        {
            if (SoundManager.Instance)
                SoundManager.Instance.PlaySFX("Coin Sound");

            GameObject coin = Instantiate(coinPrefab, coinTarget); // Should be under same canvas
            RectTransform rt = coin.GetComponent<RectTransform>();

            // Convert world spawnPoint to local canvas space
            Vector2 startAnchoredPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                coinTarget,
                RectTransformUtility.WorldToScreenPoint(null, spawnPoint.position),
                null,
                out startAnchoredPos
            );
            rt.anchoredPosition = startAnchoredPos;

            // Convert target UI to anchored canvas position
            Vector2 targetAnchoredPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                coinTarget,
                RectTransformUtility.WorldToScreenPoint(null, target.position),
                null,
                out targetAnchoredPos
            );

            // Add random arc offset
            Vector2 randomOffset = new Vector2(
                UnityEngine.Random.Range(-30f, 30f),
                UnityEngine.Random.Range(30f, 60f)
            );

            rt.DOAnchorPos(targetAnchoredPos + randomOffset, duration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => Destroy(coin));

            rt.DOScale(Vector3.one * 0.6f, 0.2f).From(0.2f).SetEase(Ease.OutBack);

            yield return new WaitForSeconds(0.05f);
        }
    }


    //coins will drop from the coin icon to the target button
    private IEnumerator SpawnCoinsUp(RectTransform target, int count, float duration)
    {
        for (int i = 0; i < count; i++)
        {
            if (SoundManager.Instance)
                SoundManager.Instance.PlaySFX("Coin Sound");

            GameObject coin = Instantiate(coinPrefab, coinTarget); // Make sure it's under same canvas
            RectTransform rt = coin.GetComponent<RectTransform>();

            // Convert world position to local position within the parent (UI canvas)
            Vector2 startPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                coinTarget,
                RectTransformUtility.WorldToScreenPoint(null, target.position),
                null,
                out startPos
            );
            rt.anchoredPosition = startPos;

            Vector2 endPos = Vector2.zero; // assuming center of coinTarget

            // Add random offset for arc
            Vector2 randomOffset = new Vector2(
                UnityEngine.Random.Range(-30f, 30f),
                UnityEngine.Random.Range(30f, 60f)
            );

            rt.DOAnchorPos(endPos + randomOffset, duration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => Destroy(coin));

            rt.DOScale(Vector3.one * 0.6f, 0.2f).From(0.2f).SetEase(Ease.OutBack);

            yield return new WaitForSeconds(0.05f);
        }
    }
    #endregion


    public void TestCoinBuy()
    {
        DataManager.instance.AddCoins(50);
        PlayCoinUp(levelGiftPanel.GetComponent<RectTransform>());
    }

    #region Purchase Shop Items

    public void PurchaseCoins(string sku)
    {
        if (AdsManager.Instance)
        {
            AdsManager.Instance.PurchaseProduct(sku);
        }
    }

    public void OnPurhcaseSuccesful(string sku)
    {
        switch (sku)
        {
            case "coinspack1":
                {
                    //DataManager.instance.AddCoins(1000);
                    //PlayCoinUp(shopCoinsBtns[0].GetComponent<RectTransform>());
                    break;
                }
            case "coinspack2":
                {
                    //DataManager.instance.AddCoins(2000);
                    //PlayCoinUp(shopCoinsBtns[0].GetComponent<RectTransform>());
                    break;
                }
            case "coinspack3":
                {
                    //DataManager.instance.AddCoins(3000);
                    //PlayCoinUp(shopCoinsBtns[0].GetComponent<RectTransform>());
                    break;
                }
            case "coinspack4":
                {
                    //DataManager.instance.AddCoins(4000);
                    //PlayCoinUp(shopCoinsBtns[0].GetComponent<RectTransform>());
                    break;
                }
            case "coinspack5":
                {
                    //DataManager.instance.AddCoins(5000);
                    //PlayCoinUp(shopCoinsBtns[0].GetComponent<RectTransform>());
                    break;
                }
            case "coinspack6":
                {
                    //DataManager.instance.AddCoins(10000);
                    //PlayCoinUp(shopCoinsBtns[0].GetComponent<RectTransform>());
                    break;
                }
        }
    }
    #endregion

  /*  #region Watch Ad for Coins

    public void OnWatchAdToGetFreeCoins()
    {
        if (SoundManager.Instance)
            SoundManager.Instance.PlaySFX("Button Click");
        watchAdPanel.SetActive(true);
        watchAdPanel.transform.localScale = Vector3.zero;
        watchAdPanel.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }

    public void OnCloseWatchAdPanel()
    {
        if (SoundManager.Instance)
            SoundManager.Instance.PlaySFX("Button Click");
        watchAdPanel
            .transform.DOScale(Vector3.zero, 0.2f)
            .SetEase(Ease.InBack)
            .OnComplete(() => watchAdPanel.SetActive(false));
    }

    public void OnWatchingExtraCoinsAd()
    {
        if (AdsManager.Instance)
        {
            if (AdsManager.Instance.HasRewardedVideo())
            {
                AdsManager.Instance.onRewardedVideoResult += OnSuccessfullyWatchingExtraCoinsAd;
                AdsManager.Instance.ShowRewardedVideo(RewardType.coins, 100);
            }
            else
            {
                Debug.Log("No rewarded video available at the moment.");
            }
        }
    }

    private void OnSuccessfullyWatchingExtraCoinsAd(RewardType rewardType, float amount)
    {
        if (rewardType == RewardType.coins)
        {
            AdsManager.Instance.onRewardedVideoResult -= OnSuccessfullyWatchingExtraCoinsAd;
            watchAdPanel
                .transform.DOScale(Vector3.zero, 0.2f)
                .SetEase(Ease.InBack)
                .OnComplete(() => watchAdPanel.SetActive(false));
            DataManager.instance.AddCoins(100);
            PlayCoinUp(startCoinPosition);

            AdsManager.Instance.SetEvent("FreeCoins");
        }
    }
    #endregion*/
}