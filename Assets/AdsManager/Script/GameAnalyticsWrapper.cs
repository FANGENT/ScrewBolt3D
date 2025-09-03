using UnityEngine;
using GameAnalyticsSDK;

public class GameAnalyticsWrapper : MonoBehaviour
{
    private static GameAnalyticsWrapper instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // Persist across scenes
            InitializeGameAnalytics();
        }
        else
        {
            Destroy(gameObject);  // Ensure only one instance of the wrapper exists
        }
    }

    // Initialize GameAnalytics SDK
    private void InitializeGameAnalytics()
    {
        GameAnalytics.Initialize();
    }

    // Track custom events
    public static void TrackCustomEvent(string eventName, string key, string value)
    {
        GameAnalytics.NewDesignEvent($"{eventName}:{key}:{value}");
    }

    public static void TrackCustomEvent(string eventName, float eventValue)
    {
        GameAnalytics.NewDesignEvent(eventName, eventValue);
    }

    // Track level events (start, fail, complete)
    public static void TrackLevelStart(string levelName)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, levelName);
    }

    public static void TrackLevelFail(string levelName, string reason = "")
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, levelName, reason);
    }

    public static void TrackLevelComplete(string levelName)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, levelName);
    }

    // Track error events
    public static void TrackErrorEvent(GAErrorSeverity severity, string message)
    {
        GameAnalytics.NewErrorEvent(severity, message);
    }

    // Track resource events (for in-game currencies or resources)
    public static void TrackResourceEvent(GAResourceFlowType flowType, string currency, float amount, string itemType, string itemId)
    {
        GameAnalytics.NewResourceEvent(flowType, currency, amount, itemType, itemId);
    }

    // Debugging wrapper to ensure everything is sent properly
    public static void DebugLog(string message)
    {
        Debug.Log($"GameAnalytics Log: {message}");
    }
}
