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

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnToggleButtonPressed);
        UpdateVisual();
    }

    private void OnToggleButtonPressed()
    {
        string key = toggleLanguage.ToString();

        //Reset all other toggles to off
        ToggleLanguageHandler[] allToggles = FindObjectsOfType<ToggleLanguageHandler>();
        foreach (var toggle in allToggles)
        {
            PlayerPrefs.SetInt(toggle.toggleLanguage.ToString(), 0);
            toggle.UpdateVisual();
        }

        //Enable only the selected toggle
        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save();

        //Set language + index
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

        UpdateVisual();

        //Reload localized text
        LocalizationManager.instance.LoadLocalizedText(
            LocalizationManager.instance.GetCurrentLanguage(),
            LocalizationManager.instance.GetCurrentLanguageIndex()
        );
    }

    private void UpdateVisual()
    {
        string key = toggleLanguage.ToString();
        bool isOn = PlayerPrefs.GetInt(key, 0) == 1;

        onIcon.enabled = isOn;
        offIcon.enabled = !isOn;
    }
}
