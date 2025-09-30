using DG.Tweening;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameplayUiManager : MonoBehaviour
{
    public static GameplayUiManager Instance;


    [Header("UI Panels")]
    public GameObject gamePlayPanel;
    public GameObject pausePanel;
    public GameObject settingPanel;
    public GameObject levelCompletePanel;
    public GameObject levelFailPanel;
    public GameObject watchAdPanel;
    public GameObject adNotAvailablePanel;

    [Header("Values")]
    public Image levelProgressImage;
    public TextMeshProUGUI levelProgressText;

    [Header("------Star Animation Effect------------")]
    public GameObject starPrefab; // UI coin image prefab
    public RectTransform starsTarget; // e.g. Canvas or dedicated CoinLayer
    public RectTransform spawnPoint; // Where coins start (e.g. coin counter)

    [Header("Watch Ad related Data for rewards Extra Container")]
    public int maxExtraContainersAllowed = 2;
    public List<EmptyContainer> extraContainerEmptyButtons = new List<EmptyContainer>();
    public int extraContainerIndex= -1;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }


    private void Start()
    {
        Time.timeScale = 1f; // Ensure time scale is set to normal

        Invoke(nameof(FindAllEmptyContainers), 2f); // slight delay to ensure all EmptyContainer instances are initialized
    }
    public void FindAllEmptyContainers()
    {
        extraContainerEmptyButtons.Clear(); // clear previous entries

        EmptyContainer[] found = FindObjectsByType<EmptyContainer>(FindObjectsSortMode.None); // true = include inactive
        foreach (var empty in found)
        {
            extraContainerEmptyButtons.Add(empty);
        }
    }


    public void OnHome()
    {
        Time.timeScale = 1f; // Resume time scale
        SceneManager.LoadScene(1);
    }

    public void OnReplay()
    {
        Time.timeScale = 1f; // Resume time scale
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnPause()
    {
        pausePanel.SetActive(true);
        pausePanel.transform.GetChild(0).localScale = Vector3.zero;
        pausePanel.transform.GetChild(0).DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).OnComplete(() => Time.timeScale = 0);
    }

    public void OnResume()
    {
        Time.timeScale = 1f; // Resume time scale
        pausePanel.transform.GetChild(0).DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
        {
            pausePanel.SetActive(false);
        });
    }

    public void OnLevelComplete()
    {
        StartCoroutine(DelayLevelComplete());
    }
    IEnumerator DelayLevelComplete()
    {
        yield return new WaitForSeconds(1.5f);
        if (SoundManager.Instance)
        {
            SoundManager.Instance.PlaySFX("Level Complete");
        }
        levelCompletePanel.SetActive(true);
        levelCompletePanel.transform.GetChild(0).localScale = Vector3.zero;
        levelCompletePanel.transform.GetChild(0).DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);

        LevelManager.Instance.UnlockNextLevel();
    }

    public void OnContinue()
    {
        if (SoundManager.Instance)
        {
            SoundManager.Instance.PlaySFX("Button Click");
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void OnLevelFailed()
    {
        if (SoundManager.Instance)
        {
            SoundManager.Instance.PlaySFX("Level Failed");
        }
        levelFailPanel.SetActive(true);
        levelFailPanel.transform.GetChild(0).localScale = Vector3.zero;
        levelFailPanel.transform.GetChild(0).DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
    }
    public void CloseLevelFail()
    {
        levelFailPanel
            .transform.GetChild(0).DOScale(Vector3.zero, 0.2f)
            .SetEase(Ease.InBack)
            .OnComplete(() => levelFailPanel.SetActive(false));
    }

    public void OnAdNotAvailable()
    {
        adNotAvailablePanel.SetActive(true);
        adNotAvailablePanel.transform.GetChild(0).localScale = Vector3.zero;
        adNotAvailablePanel.transform.GetChild(0).DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }
    public void OnCloseAdNotAvailablePanel()
    {
        if (SoundManager.Instance)
        {
            SoundManager.Instance.PlaySFX("Button Click");
        }
        adNotAvailablePanel
            .transform.GetChild(0).DOScale(Vector3.zero, 0.2f)
            .SetEase(Ease.InBack)
            .OnComplete(() => adNotAvailablePanel.SetActive(false));
    }

    #region Extra Container Related Methods

    public void OnEmptyContainerButtonClicked(int placementIdx)
    {
        if (SoundManager.Instance)
        {
            SoundManager.Instance.PlaySFX("Button Click");
        }

        watchAdPanel.SetActive(true);
        watchAdPanel.transform.GetChild(0).localScale = Vector3.zero;
        watchAdPanel.transform.GetChild(0).DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);

        /*//Store placementIdx and emptyBtn to use after ad is watched successfully
        extraContainerIndex = placementIdx;
        extraContainerEmptyButton = emptyBtn;*/
    }

    public void OnCloseWatchAdPanel()
    {
        if (SoundManager.Instance)
        {
            SoundManager.Instance.PlaySFX("Button Click");
        }

        watchAdPanel
            .transform.GetChild(0).DOScale(Vector3.zero, 0.2f)
            .SetEase(Ease.InBack)
            .OnComplete(() => watchAdPanel.SetActive(false));
    }


    public void OnWatchingExtraContainerAd()
    {
        TryShowExtraContainerAd(OnCloseWatchAdPanel);
    }

    public void WatchAdToGetExtraContainerOnLevelFail()
    {
        TryShowExtraContainerAd(CloseLevelFail);
    }

    private void TryShowExtraContainerAd(System.Action onFailAction)
    {
        if (AdsManager.Instance == null)
        {
            onFailAction?.Invoke();
            OnAdNotAvailable();
            return;
        }

        if (AdsManager.Instance.HasRewardedVideo())
        {
            AdsManager.Instance.onRewardedVideoResult += OnSuccessfullyWatchingExtraContainerAd;
            AdsManager.Instance.ShowRewardedVideo(RewardType.container, 1);
        }
        else
        {
            onFailAction?.Invoke();
            OnAdNotAvailable();
            Debug.Log("No rewarded video available at the moment.");
        }
    }

    private void OnSuccessfullyWatchingExtraContainerAd(RewardType rewardType, float amount)
    {
        if (rewardType != RewardType.container)
            return;

        AdsManager.Instance.onRewardedVideoResult -= OnSuccessfullyWatchingExtraContainerAd;
        OnCloseWatchAdPanel();
        CloseLevelFail();

        // decide extra container index
        if (maxExtraContainersAllowed == 2)
        {
            if (extraContainerEmptyButtons[0].PlacementIndex == 2)
            {
                extraContainerEmptyButtons[0].gameObject.SetActive(false);
            }
            extraContainerIndex = 2;
        }
        else if (maxExtraContainersAllowed == 1)
        {
            if (extraContainerEmptyButtons[1].PlacementIndex == 3)
            {
                extraContainerEmptyButtons[1].gameObject.SetActive(false);
            }
            extraContainerIndex = 3;
        }
            

        maxExtraContainersAllowed--;
        BoltContainerManager.Instance.MakeNewContainerWhereUnscrewedBoltsCanBePlaced(extraContainerIndex);

        Debug.Log("Extra container index: " + extraContainerIndex);

        AdsManager.Instance.SetEvent("GotExtraContainerByAd");
    }

    public void GetExtraContainerWithCoins(int coinCost)
    {
        if (DataManager.instance.GetCoins() < coinCost)
        {
            Debug.Log("Not enough coins to get extra container.");
            return;
        }

        // decide extra container index
        if (maxExtraContainersAllowed == 2)
        {
            if (extraContainerEmptyButtons[0].PlacementIndex == 2)
            {
                extraContainerEmptyButtons[0].gameObject.SetActive(false);
            }
            extraContainerIndex = 2;
        }
        else if (maxExtraContainersAllowed == 1)
        {
            if (extraContainerEmptyButtons[1].PlacementIndex == 3)
            {
                extraContainerEmptyButtons[1].gameObject.SetActive(false);
            }
            extraContainerIndex = 3;
        }

        maxExtraContainersAllowed--;

        DataManager.instance.AddCoins(-coinCost);
        PlayCoinUp(levelFailPanel.GetComponent<RectTransform>());
        BoltContainerManager.Instance.MakeNewContainerWhereUnscrewedBoltsCanBePlaced(extraContainerIndex);
        CloseLevelFail();
        AdsManager.Instance.SetEvent("GotExtraContainerByCoins");
    }

    #endregion

    public void PLayStarAnimationFromPosition(RectTransform spawnPosition)
    {
        PlayCoinUp(spawnPosition);
    }


    #region Coin Animation

    private int displayedCoins = 0; // Keep track of the last shown value

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

            GameObject coin = Instantiate(starPrefab, starsTarget); // Should be under same canvas
            RectTransform rt = coin.GetComponent<RectTransform>();

            // Convert world spawnPoint to local canvas space
            Vector2 startAnchoredPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                starsTarget,
                RectTransformUtility.WorldToScreenPoint(null, spawnPoint.position),
                null,
                out startAnchoredPos
            );
            rt.anchoredPosition = startAnchoredPos;

            // Convert target UI to anchored canvas position
            Vector2 targetAnchoredPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                starsTarget,
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

            GameObject coin = Instantiate(starPrefab, starsTarget); // Make sure it's under same canvas
            RectTransform rt = coin.GetComponent<RectTransform>();

            // Convert world position to local position within the parent (UI canvas)
            Vector2 startPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                starsTarget,
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



}