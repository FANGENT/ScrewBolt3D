using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;


public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager instance;

    private Dictionary<string, string> localizedText;
    public string currentLanguage
    {
        get
        {
            return PlayerPrefs.GetString("LANGUAGE", "English");  // Default language
        }
        set
        {
            PlayerPrefs.SetString("LANGUAGE", value);
        }
    }
    public int currentLanguageIndex
    {
        get
        {
            return PlayerPrefs.GetInt("LANGUAGE_Index", 0);  // Default language
        }
        set
        {
            PlayerPrefs.SetInt("LANGUAGE_Index", value);
        }
    }
    private bool isReady = false;
    private string missingTextString = "Text not found";
    public static readonly Dictionary<string, string> LanguageCodes = new Dictionary<string, string>
{
    {"English", "en"},
    {"Korean", "ko"},
    {"Chinese", "zh"},
    {"Russian", "ru"},
    {"Dutch", "nl"},
    {"Taiwan", "zh-TW"},
    {"Spanish", "es"},
    {"Japanese", "ja"},
    {"Portuguese", "pt"},
    {"Greece", "el"},
    {"Swedish", "sv"},
    {"Ukrainian", "uk"},
    {"Netherlands", "nl-NL"},
    {"Italian", "it"},
    {"Turkish", "tr"},
    {"Poland", "pl"},
    {"French", "fr"},
    {"Thailand", "th"},
    {"Romania", "ro"},
    {"Indonesian", "id"}
};
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        LoadLocalizedText(currentLanguage, currentLanguageIndex);
    }
    // Call this method to load a language file
    public void LoadLocalizedText(string language, int index)
    {
        currentLanguage = language;
        currentLanguageIndex = index;
        string fileName = "Localization/" + language; // Assuming JSON files are in Resources/Localization/
        TextAsset jsonText = Resources.Load<TextAsset>(fileName);

        if (jsonText != null)
        {
            string dataAsJson = jsonText.text;
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

            localizedText = new Dictionary<string, string>();
            foreach (var item in loadedData.items)
            {
                localizedText.Add(item.key, item.value);
            }
            isReady = true;
        }
        else
        {
            Debug.LogError("Cannot find file: " + fileName);
        }

        //string filePath = Path.Combine(Application.streamingAssetsPath, language + ".json");

        //if (File.Exists(filePath))
        //{
        //    string dataAsJson = File.ReadAllText(filePath);
        //    LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

        //    localizedText = new Dictionary<string, string>();
        //    foreach (var item in loadedData.items)
        //    {
        //        localizedText.Add(item.key, item.value);
        //    }
        //    isReady = true;
        //}
        //else
        //{
        //    Debug.LogError("Cannot find file: " + filePath);
        //}

        // Update UI 
        /*if (GameController.Instance)
            GameController.Instance.uiController.UpdateLanguage();*/
        UpdateAllLocalizedText();


    }

    public string GetLocalizedValue(string key)
    {
        if (localizedText.ContainsKey(key))
        {
            return localizedText[key];
        }
        return missingTextString;
    }

    // This method will be called to update all the UI text elements when language is changed
    public void UpdateAllLocalizedText()
    {
        LocalizedText[] texts = FindObjectsOfType<LocalizedText>();
        foreach (LocalizedText text in texts)
        {
            text.UpdateText();
        }
    }

    public bool IsReady()
    {
        return isReady;
    }

    public string GetCurrentLanguage()
    {
        return currentLanguage;
    }
    public int GetCurrentLanguageIndex()
    {
        return currentLanguageIndex;
    }
}

[System.Serializable]
public class LocalizationData
{
    public LocalizationItem[] items;
}

[System.Serializable]
public class LocalizationItem
{
    public string key;
    public string value;
}
