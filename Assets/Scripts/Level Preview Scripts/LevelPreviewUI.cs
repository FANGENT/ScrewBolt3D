using System.Collections.Generic;
using UnityEngine;

public class LevelPreviewUI : MonoBehaviour
{
    [Header("Prefabs & Parent")]
    [SerializeField]
    private RectTransform parent; // usually this same object

    [SerializeField]
    private StepUI stepPrefab; // prefab from section 2A

    [SerializeField]
    private LevelBadgeUI levelBadgePrefab; // prefab from section 2B

    [Header("Visuals")]
    [SerializeField]
    private Color connectorDone = new Color32(92, 200, 98, 255); // green

    [SerializeField]
    private Color connectorTodo = new Color32(160, 160, 160, 255); // gray

    [Header("Data")]
    [SerializeField, Min(1)]
    private int stepsPerPreview = 5; // fixed window size = 5

    [SerializeField]
    private List<Sprite> allLevelIcons = new List<Sprite>(); // assign ALL level sprites here (e.g., 30)

    // cache
    private int _currentLevelIndex; // expected 0-based (LV1 = 0)
    private int _windowStart; // starting level index of the current 5-step window (0,5,10,...)
    private int _localIndex; // index of current level inside the 5-slot window (0..4)

    private void Reset()
    {
        parent = (RectTransform)transform;
    }

    private void Start()
    {
        if (!parent)
            parent = (RectTransform)transform;

        // NOTE: If your LevelManager is 1-based, uncomment the line with -1 and remove the other.
        _currentLevelIndex = Mathf.Max(0, LevelManager.Instance.currentLevel); // 0-based
        //_currentLevelIndex = Mathf.Max(0, LevelManager.Instance.currentLevel - 1);   // 1-based -> convert to 0-based

        Build();
    }

    /// <summary>
    /// Rebuilds the 5-step preview window according to the current level.
    /// </summary>
    public void Build()
    {
        if (!parent)
            parent = (RectTransform)transform;

        int totalLevels = Mathf.Max(allLevelIcons.Count, 0);
        if (totalLevels == 0)
        {
            Debug.LogWarning("[LevelPreviewUI] No level icons assigned.");
        }

        _currentLevelIndex = Mathf.Clamp(_currentLevelIndex, 0, Mathf.Max(0, totalLevels - 1));

        // Compute window start (0, 5, 10, 15, ...)
        _windowStart = (_currentLevelIndex / stepsPerPreview) * stepsPerPreview;
        _localIndex = _currentLevelIndex - _windowStart; // 0..4 inside the window

        // Clear previous children
        for (int i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);

        // Spawn steps in the window [windowStart .. windowStart+stepsPerPreview-1]
        for (int i = 0; i < stepsPerPreview; i++)
        {
            StepUI step = Instantiate(stepPrefab, parent);

            int globalIdx = _windowStart + i; // absolute level index
            Sprite icon =
                (globalIdx >= 0 && globalIdx < totalLevels) ? allLevelIcons[globalIdx] : null;

            // Completed = strictly before current level in global ordering
            bool isCompleted = (globalIdx < _currentLevelIndex);

            // Connector: show for all except last element in the 5-slot window
            bool showConnector = (i < stepsPerPreview - 1);

            // Connector color is green when the node BEFORE it is completed
            bool connectorCompleted = isCompleted;

            step.Setup(
                icon,
                isCompleted,
                showConnector,
                connectorCompleted,
                connectorDone,
                connectorTodo
            );
        }

        // Add LV badge on the *current* step of this window
        // Badge text is 1-based for display (LV1..LV30)
        int displayLevel = _currentLevelIndex + 1;
        LevelBadgeUI badge = Instantiate(
            levelBadgePrefab,
            parent.GetChild(Mathf.Clamp(_localIndex, 0, stepsPerPreview - 1))
        );
        badge.SetLevel(displayLevel);
    }

    /// <summary>
    /// Call this when the player's level changes; value should be 0-based.
    /// If your external value is 1-based, pass (newLevel - 1) here.
    /// </summary>
    public void RefreshForLevel(int newLevelIndex0Based)
    {
        _currentLevelIndex = Mathf.Max(0, newLevelIndex0Based);
        Build();
    }

    /// <summary>
    /// Optional helper if you only know the level in 1-based form (LV1=1).
    /// </summary>
    public void RefreshForLevelOneBased(int newLevelNumber1Based)
    {
        RefreshForLevel(Mathf.Max(1, newLevelNumber1Based) - 1);
    }
}
