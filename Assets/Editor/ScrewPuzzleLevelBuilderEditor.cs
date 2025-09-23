#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScrewPuzzleLevelBuilder))]
public class ScrewPuzzleLevelBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var builder = (ScrewPuzzleLevelBuilder)target;

        GUILayout.Space(8);
        if (GUILayout.Button("Build Level Hierarchy", GUILayout.Height(32)))
        {
            builder.BuildLevelHierarchy();
        }

        EditorGUILayout.HelpBox(
            "Attach this to the raw model root (e.g., Camp_GroupD). " +
            "Click the button to create PivotX/PivotY/Model, move all Board_* to Model/Parts, " +
            "move Screw* to Model/Screws, and add BlockedScrewsController to each screw.",
            MessageType.Info);
    }

    // Handy menu item: select a Camp_GroupD in the hierarchy and run this if you like.
    [MenuItem("Tools/Screw Puzzle/Build From Selected", priority = 0)]
    private static void BuildFromSelected()
    {
        var sel = Selection.activeTransform;
        if (!sel) { Debug.LogWarning("Select a root object first."); return; }
        var b = sel.GetComponent<ScrewPuzzleLevelBuilder>();
        if (!b) { b = sel.gameObject.AddComponent<ScrewPuzzleLevelBuilder>(); }
        b.BuildLevelHierarchy();
    }
}
#endif
