using UnityEngine;
using TMPro;

public class LevelBadgeUI : MonoBehaviour
{
    [SerializeField] private TMP_Text label;

    public void SetLevel(int levelNumber)
    {
        if (label) label.text = "LV" + levelNumber;
    }
}
