using UnityEngine;
using UnityEngine.UI;

public class ToggleLanguageHandler : MonoBehaviour
{
    public enum ToggleLanguage { English, Spanish, French }
    public ToggleLanguage toggleLanguage;

    [Header("Icons")]
    public Image onIcon;
    public Image offIcon;

    private Button button;

    private void Awake()
    {
        // Set default language if none exists
        if (PlayerPrefs.HasKey("LANGUAGE"))
        {
            string lang= PlayerPrefs.GetString("LANGUAGE");
            if(lang == "English")
                PlayerPrefs.SetInt(ToggleLanguage.English.ToString(), 1);
            else if(lang == "Spanish")
                PlayerPrefs.SetInt(ToggleLanguage.Spanish.ToString(), 1);
            else if(lang == "French")
                PlayerPrefs.SetInt(ToggleLanguage.French.ToString(), 1);

            PlayerPrefs.Save();
        }
    }

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnToggleButtonPressed);

        // Always update visuals after prefs are set
        UpdateVisual();
    }

    private void OnToggleButtonPressed()
    {
        string key = toggleLanguage.ToString();

        // Reset all other toggles to off
        ToggleLanguageHandler[] allToggles = FindObjectsOfType<ToggleLanguageHandler>();
        foreach (var toggle in allToggles)
        {
            PlayerPrefs.SetInt(toggle.toggleLanguage.ToString(), 0);
            toggle.UpdateVisual();
        }

        // Enable only the selected toggle
        PlayerPrefs.SetInt(key, 1);

        // Set language + index
        switch (toggleLanguage)
        {
            case ToggleLanguage.English:
                PlayerPrefs.SetString("LANGUAGE", "English");
                PlayerPrefs.SetInt("LANGUAGE_Index", 0);
                break;
            case ToggleLanguage.Spanish:
                PlayerPrefs.SetString("LANGUAGE", "Spanish");
                PlayerPrefs.SetInt("LANGUAGE_Index", 1);
                break;
            case ToggleLanguage.French:
                PlayerPrefs.SetString("LANGUAGE", "French");
                PlayerPrefs.SetInt("LANGUAGE_Index", 2);
                break;
        }

        PlayerPrefs.Save();
        UpdateVisual();

        // Reload localized text
        LocalizationManager.instance.LoadLocalizedText(
            LocalizationManager.instance.GetCurrentLanguage(),
            LocalizationManager.instance.GetCurrentLanguageIndex()
        );
    }

    private void UpdateVisual()
    {
        string key = toggleLanguage.ToString();
        bool isOn = PlayerPrefs.GetInt(key, 0) == 1;

        // Enable the correct icon state
        if (onIcon != null) onIcon.enabled = isOn;
        if (offIcon != null) offIcon.enabled = !isOn;
    }
}
