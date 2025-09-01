using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LevelPreviewManager : MonoBehaviour
{
    [Header("References")]
    public Transform iconParent;          // Parent with HorizontalLayoutGroup
    public GameObject iconPrefab;         // Prefab for item icons
    public GameObject levelNodePrefab;    // Prefab for LV badge

    [Header("Sprites")]
    public List<Sprite> iconSprites;      // List of icons (cat, rose, fish, etc.)
    public Sprite checkmarkSprite;        // Tick/check sprite
    public Sprite emptySprite;            // Empty/default icon

    [Header("Progress Data")]
    public int currentLevel = 4;          // Example: player is at Level 4
    public int completedIcons = 3;        // Example: 3 items collected/completed

    private void Start()
    {
        GeneratePreview();
    }

    public void GeneratePreview()
    {
        // Clear old children
        foreach (Transform child in iconParent)
            Destroy(child.gameObject);

        // Spawn icons
        for (int i = 0; i < iconSprites.Count; i++)
        {
            GameObject newIcon = Instantiate(iconPrefab, iconParent);
            Image iconImage = newIcon.transform.Find("Icon").GetComponent<Image>();
            Image checkImage = newIcon.transform.Find("Check").GetComponent<Image>();

            iconImage.sprite = iconSprites[i];

            if (i < completedIcons)
                checkImage.enabled = true;  // Show check
            else
                checkImage.enabled = false; // Hide check
        }

        // Spawn level node (LV text)
        GameObject levelNode = Instantiate(levelNodePrefab, iconParent);
        TMP_Text levelText = levelNode.GetComponentInChildren<TMP_Text>();
        levelText.text = "LV" + currentLevel;
    }
}
