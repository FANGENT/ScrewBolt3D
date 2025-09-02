using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    /*[Header("Values")]
*/

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


}