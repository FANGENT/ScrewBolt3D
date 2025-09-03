using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LevelPreviewUI : MonoBehaviour
{
    [Header("Prefabs & Parent")]
    [SerializeField] private RectTransform parent;        // usually this same object
    [SerializeField] private StepUI stepPrefab;           // prefab from section 2A
    [SerializeField] private LevelBadgeUI levelBadgePrefab; // prefab from section 2B

    [Header("Visuals")]
    [SerializeField] private Color connectorDone = new Color32(92, 200, 98, 255); // green
    [SerializeField] private Color connectorTodo = new Color32(160, 160, 160, 255); // gray

    [Header("Data")]
    [SerializeField] private int stepsPerPreview = 5;     // fixed 5 like screenshot
    [SerializeField] private List<Sprite> stepIcons = new List<Sprite>(5); // assign 5
    [SerializeField] private int completedSteps = 3;      // 0..5
    [SerializeField] private int levelNumber = 4;         // LV4

    private void Reset()
    {
        parent = (RectTransform)transform;
    }

    

    private void Start()
    {
       levelNumber = LevelManager.Instance.currentLevel;
        completedSteps = levelNumber - 1;
        Build();
    }

    /// <summary>
    /// Call this whenever you need to refresh the preview (e.g., when progress changes).
    /// </summary>
    public void Build()
    {
        if (!parent) parent = (RectTransform)transform;

        // safety
        completedSteps = Mathf.Clamp(completedSteps, 0, stepsPerPreview);
        EnsureIconListSize();

        // clear previous
        for (int i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);

        // spawn steps
        for (int i = 0; i < stepsPerPreview; i++)
        {
            StepUI step = Instantiate(stepPrefab, parent);
            bool isCompleted = i < completedSteps;
            bool showConnector = i < stepsPerPreview/* - 1*/;               // last step has no connector
            bool connectorCompleted = i < completedSteps;               // connector after a completed node is green

            step.Setup(stepIcons[i], isCompleted, showConnector, connectorCompleted, connectorDone, connectorTodo);
        }

        // add LV badge at the end
        LevelBadgeUI badge = Instantiate(levelBadgePrefab, parent.GetChild(levelNumber-1));
        badge.SetLevel(levelNumber);
    }

    private void EnsureIconListSize()
    {
        // Ensure we have exactly 'stepsPerPreview' icons (fill with last known or null)
        if (stepIcons == null) stepIcons = new List<Sprite>(stepsPerPreview);
        while (stepIcons.Count < stepsPerPreview) stepIcons.Add(null);
        if (stepIcons.Count > stepsPerPreview) stepIcons.RemoveRange(stepsPerPreview, stepIcons.Count - stepsPerPreview);
    }

    // Convenience API for code-driven updates
    public void SetData(IList<Sprite> icons5, int completed0to5, int levelNo)
    {
        stepIcons = new List<Sprite>(icons5);
        completedSteps = Mathf.Clamp(completed0to5, 0, stepsPerPreview);
        levelNumber = levelNo;
        Build();
    }
}
