using UnityEngine;
using TMPro;

public class LocalizedText : MonoBehaviour
{
    public string key; // Key in JSON file
    private TextMeshProUGUI text;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        UpdateText();
    }

    public void UpdateText()
    {
        if (LocalizationManager.instance != null && LocalizationManager.instance.IsReady())
        {
            text.text = LocalizationManager.instance.GetLocalizedValue(key);
        }
    }
}
