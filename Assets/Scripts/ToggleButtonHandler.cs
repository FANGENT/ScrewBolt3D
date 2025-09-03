using UnityEngine;
using UnityEngine.UI;

public class ToggleButtonHandler : MonoBehaviour
{
    public enum ToggleType { Sound, Music, Vibration }
    public ToggleType toggleType;

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
        string key = toggleType.ToString();
        bool currentState = PlayerPrefs.GetInt(key, 1) == 1;
        bool newState = !currentState;

        PlayerPrefs.SetInt(key, newState ? 1 : 0);
        PlayerPrefs.Save();

        //--A uncomment this if you want to play/stop music on toggle
        if (key == "Music")
        {
            if(SoundManager.Instance)
            {
                SoundManager.Instance.PlayMusic("Main Menu BGM");
            }
        }

        if (toggleType == ToggleType.Vibration && newState)
            Handheld.Vibrate();

        UpdateVisual();
    }

    private void UpdateVisual()
    {
        string key = toggleType.ToString();
        bool isOn = PlayerPrefs.GetInt(key, 1) == 1;

        onIcon.enabled = isOn;
        offIcon.enabled = !isOn;
    }

    // Optional static methods for other classes to check toggle states
    public static bool IsSoundOn() => PlayerPrefs.GetInt("Sound", 1) == 1;
    public static bool IsMusicOn() => PlayerPrefs.GetInt("Music", 1) == 1;
    public static bool IsVibrationOn() => PlayerPrefs.GetInt("Vibration", 1) == 1;
}
