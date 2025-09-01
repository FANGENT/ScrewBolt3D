using DG.Tweening;
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
    /*public GameObject loadingScreen;
    public Slider loadingSlider;
    public GameObject shopPanel;
    
    public GameObject notEnoughCoinsPanel;
    public GameObject watchAdPanel;*/

    [Header("------Values------------")]
    public TextMeshProUGUI coinsText;

    [Header("------Buttons------------")]
    public Button playBtn;
    public Button settingsBtn;
    public Button profileBtn;
    public Button removeAdsBtn;
    public Button levelGiftBtn;
    /*public Button watchAdCoinsBtn;
    public RectTransform startCoinPosition; // For the coin icon in the UI
    public List<Button> shopCoinsBtns = new List<Button>();*/

    /*[Header("------Coin Drop Effect------------")]
    public GameObject coinPrefab; // UI coin image prefab
    public RectTransform coinParent; // e.g. Canvas or dedicated CoinLayer
    public RectTransform spawnPoint; // Where coins start (e.g. coin counter)
    public TextMeshProUGUI levelNumber;*/


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
}