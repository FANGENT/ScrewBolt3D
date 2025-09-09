using DG.Tweening;
using System;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    [Header("Data")]
    public int coins;
    public int drills;
    public int hammers;
    public int wipers;

    private const string coinsKey = "Coins";
    private const string drillsKey = "Drills";
    private const string hammersKey = "Hammers";
    private const string wipersKey = "Wipers";

    [Header("Events")]
    public static Action onCoinsUpdated;
    public static Action onDrillsUpdated;
    public static Action onHammersUpdated;
    public static Action onWipersUpdated;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject); // Ensures only one instance exists
        }

        LoadData();
    }

    #region Coins
    public void AddCoins(int amount)
    {
        coins += amount;
        SaveData();
        onCoinsUpdated?.Invoke();
    }

    public void Purchase(int price)
    {
        int previousCoins = coins;
        coins -= price;
        SaveData();

        // Animate the counter from previousCoins to new coins
        DOTween.To(() => previousCoins, x =>
        {
            UpdateCoinText(x);
        }, coins, 0.5f).SetEase(Ease.OutQuad);

        onCoinsUpdated?.Invoke();
    }

    private void UpdateCoinText(int value)
    {
        UIManager.Instance.coinsText.text = value.ToString("N0");
    }

    public int GetCoins() => coins;
    #endregion

    #region Drills
    public void AddDrills(int amount)
    {
        drills += amount;
        SaveData();
        onDrillsUpdated?.Invoke();
    }

    public void UseDrill()
    {
        if (drills > 0)
        {
            drills--;
            SaveData();
            onDrillsUpdated?.Invoke();
        }
    }

    public int GetDrills() => drills;
    #endregion

    #region Hammers
    public void AddHammers(int amount)
    {
        hammers += amount;
        SaveData();
        onHammersUpdated?.Invoke();
    }

    public void UseHammer()
    {
        if (hammers > 0)
        {
            hammers--;
            SaveData();
            onHammersUpdated?.Invoke();
        }
    }

    public int GetHammers() => hammers;
    #endregion

    #region Wipers
    public void AddWipers(int amount)
    {
        wipers += amount;
        SaveData();
        onWipersUpdated?.Invoke();
    }

    public void UseWiper()
    {
        if (wipers > 0)
        {
            wipers--;
            SaveData();
            onWipersUpdated?.Invoke();
        }
    }

    public int GetWipers() => wipers;
    #endregion

    #region Save/Load
    private void LoadData()
    {
        coins = PlayerPrefs.GetInt(coinsKey, 0);
        drills = PlayerPrefs.GetInt(drillsKey, 0);
        hammers = PlayerPrefs.GetInt(hammersKey, 0);
        wipers = PlayerPrefs.GetInt(wipersKey, 0);
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt(coinsKey, coins);
        PlayerPrefs.SetInt(drillsKey, drills);
        PlayerPrefs.SetInt(hammersKey, hammers);
        PlayerPrefs.SetInt(wipersKey, wipers);
        PlayerPrefs.Save();
    }
    #endregion
}
