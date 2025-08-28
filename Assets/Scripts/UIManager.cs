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
    /*public GameObject loadingScreen;
    public Slider loadingSlider;
    public GameObject shopPanel;
    public GameObject removeAdsPanel;
    public GameObject notEnoughCoinsPanel;
    public GameObject watchAdPanel;*/

    [Header("------Values------------")]
    public TextMeshProUGUI coinsText;

    [Header("------Buttons------------")]
    public Button playBtn;
    public Button settingsBtn;
    public Button profileBtn;
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

    }

    void AddAllListeners()
    {
        playBtn.onClick.AddListener(Play);
        settingsBtn.onClick.AddListener(OnSettingPressed);
        profileBtn.onClick.AddListener(OnProfilePressed);
        //watchAdCoinsBtn.onClick.AddListener(WatchAdForCoins);
    }

    void Play()
    {
        SceneManager.LoadScene(2);
    }

    void OnSettingPressed()
    {
        settingsPanel.gameObject.SetActive(true);
        // Reset starting position to right side
        settingsPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(screenWidth, 0), 0f).SetEase(Ease.OutCubic);

        // Animate into view
        settingsPanel.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, 0.4f).SetEase(Ease.OutCubic);
    }

    public void OnSettingsClosed()
    {
        // Animate out of view to the right side
        settingsPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(screenWidth, 0), 0.4f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            settingsPanel.gameObject.SetActive(false);
        });
    }

    void OnProfilePressed()
    {  
        profilePanel.gameObject.SetActive(true);
    }
}