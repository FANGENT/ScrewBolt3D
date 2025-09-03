using DG.Tweening;
using System;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    [Header("Data")]
    public int coins;
    private const string coinsKey = "Coins";

    [Header("Events")]
    public static Action onCoinsUpdated;

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

    public void AddCoins(int amount)
    {
        coins += amount;
        SaveData();
        onCoinsUpdated?.Invoke();
    }

    public void Purchase(int price)
    {
        /*coins -= price;
        SaveData();
        onCoinsUpdated?.Invoke();*/

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

    public int GetCoins()
    {
        return coins;
    }

    private void LoadData()
    {
        coins = PlayerPrefs.GetInt(coinsKey);
        //onCoinsUpdated?.Invoke();
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt(coinsKey, coins);
    }
}
