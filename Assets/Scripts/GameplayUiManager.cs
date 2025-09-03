using DG.Tweening;
using System.Collections;
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
    /*public GameObject levelCompletePanel;
    public GameObject levelFailPanel;
    public GameObject gameOverPanel;*/

    [Header("Values")]
    public Image levelProgressImage;
    public TextMeshProUGUI levelProgressText;

    [Header("------Star Animation Effect------------")]
    public GameObject starPrefab; // UI coin image prefab
    public RectTransform starsTarget; // e.g. Canvas or dedicated CoinLayer
    public RectTransform spawnPoint; // Where coins start (e.g. coin counter)

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
        if(SoundManager.Instance)
        {
            SoundManager.Instance.PlaySFX("Level Complete");
        }
        levelCompletePanel.SetActive(true);
        levelCompletePanel.transform.GetChild(0).localScale = Vector3.zero;
        levelCompletePanel.transform.GetChild(0).DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
    }



    public void TestCoinBuy()
    {
        //DataManager.instance.AddCoins(1000);
        PlayCoinUp(settingPanel.GetComponent<RectTransform>());
    }

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