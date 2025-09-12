using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Level Settings")]
    public List<GameObject> models; // Prefabs for each level
    public Transform levelParent;   // Parent to hold instantiated level
    public int currentLevel;        // Current level index (0-based)
    public GameObject currentModel; // Current level model

    private GameObject currentLevelObj;

    private const string LEVEL_KEY = "CurrentLevel";
    private const string UNLOCKED_KEY = "UnlockedLevel";

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        LoadCurrentLevel();
    }

    /// <summary>
    /// Loads the current level prefab.
    /// </summary>
    public void LoadCurrentLevel()
    {
        currentLevel = PlayerPrefs.GetInt(LEVEL_KEY, 0); // Default = 0

        if (currentLevel >= models.Count)
            currentLevel = 0; // Reset if out of range

        if (currentLevelObj != null)
            Destroy(currentLevelObj);

        currentLevelObj = Instantiate(models[currentLevel], levelParent);
        currentLevelObj.SetActive(true);

        currentModel = currentLevelObj;
    }

    /// <summary>
    /// Unlocks the next level.
    /// </summary>
    public void UnlockNextLevel()
    {
        int unlockedLevel = PlayerPrefs.GetInt(UNLOCKED_KEY, 0);

        // Unlock next if not already
        if (currentLevel + 1 > unlockedLevel && currentLevel + 1 < models.Count)
        {
            PlayerPrefs.SetInt(UNLOCKED_KEY, currentLevel + 1);
        }

        // Advance CurrentLevel too
        PlayerPrefs.SetInt(LEVEL_KEY, currentLevel + 1);
        PlayerPrefs.Save();

        Debug.Log($"UnlockedLevel = {PlayerPrefs.GetInt(UNLOCKED_KEY)}, CurrentLevel = {PlayerPrefs.GetInt(LEVEL_KEY)}");
    }


    /// <summary>
    /// Loads the next level.
    /// </summary>
    public void LoadNextLevel()
    {
        currentLevel++;
        if (currentLevel >= models.Count)
            currentLevel = 0; // Loop back or stop at last level

        PlayerPrefs.SetInt(LEVEL_KEY, currentLevel);
        PlayerPrefs.Save();

        LoadCurrentLevel();
    }

    /// <summary>
    /// Reloads the current level.
    /// </summary>
    public void ReloadLevel()
    {
        LoadCurrentLevel();
    }

    /// <summary>
    /// Resets progress (optional).
    /// </summary>
    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey(LEVEL_KEY);
        PlayerPrefs.DeleteKey(UNLOCKED_KEY);
    }

    /// <summary>
    /// Returns highest unlocked level index.
    /// </summary>
    public int GetUnlockedLevel()
    {
        return PlayerPrefs.GetInt(UNLOCKED_KEY, 0);
    }

    public void CenterTheModel()
    {
        if (currentModel != null)
        {
            currentModel.transform.DOKill();
            currentModel.transform.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.OutQuad);
            currentModel.transform.DOLocalRotate(Vector3.zero, 0.5f).SetEase(Ease.OutQuad);
        }
    }
}
