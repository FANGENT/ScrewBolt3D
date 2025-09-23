using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class ScrewPuzzleLevelBuilder : MonoBehaviour
{
    [Header("Naming (case-insensitive)")]
    public string boardPrefix = "Board_";
    public string screwPrefix = "Screw";

    [Header("Materials (assign in Inspector)")]
    public Material greenMat;
    public Material blueMat;
    public Material yellowMat;
    public Material redMat;

    [Header("Options")]
    public Transform searchRootOverride;
    [Tooltip("Layer index applied to ALL screws under Model/Screws.")]
    public int coloredScrewLayer = 10;

    private static readonly string[] ColorNames = { "red", "green", "blue", "yellow" };

    public void BuildLevelHierarchy()
    {
        Transform root = transform;
        Transform pivotX = EnsureChild("PivotX", root);
        Transform pivotY = EnsureChild("PivotY", pivotX);
        Transform model = EnsureChild("Model", pivotY);
        Transform parts = EnsureChild("Parts", model);
        Transform screwsParent = EnsureChild("Screws", model);

#if UNITY_EDITOR
        UnityEditor.Undo.RegisterFullObjectHierarchyUndo(root.gameObject, "Build Screw Puzzle Level");
#endif

        Transform searchRoot = searchRootOverride ? searchRootOverride : root;

        // All boards
        var boards = searchRoot.GetComponentsInChildren<Transform>(true)
            .Where(t => t && t != parts && t != screwsParent &&
                        t.name.StartsWith(boardPrefix, StringComparison.OrdinalIgnoreCase))
            .Distinct().ToList();

        var allScrews = new List<Transform>();

        foreach (var board in boards)
        {
            if (board.parent != parts) board.SetParent(parts, true);
            SetupBoardComponents(board.gameObject);

            // --- DIRECT children of the board that look like screws
            var boardChildScrews = new List<Transform>();
            for (int i = 0; i < board.childCount; i++)
            {
                var c = board.GetChild(i);
                if (IsScrewName(c.name))
                    boardChildScrews.Add(c);
            }

            // >>> Assign references of these screws into AllBlockingBolts BEFORE moving them
            AssignBoltsList(board.gameObject, boardChildScrews);

            // Now move those screws to the global Screws parent and prep them
            foreach (var s in boardChildScrews)
            {
                s.SetParent(screwsParent, true);
                allScrews.Add(s);

                if (!s.TryGetComponent(out BlockedScrewsController _))
                {
#if UNITY_EDITOR
                    UnityEditor.Undo.AddComponent<BlockedScrewsController>(s.gameObject);
#else
                    s.gameObject.AddComponent<BlockedScrewsController>();
#endif
                }

                SetupSphereCollider(s.gameObject);
                SetLayerRecursively(s, coloredScrewLayer);

#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(s.gameObject);
#endif
            }

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(board.gameObject);
#endif
        }

        // Color/name screws in triplets (multiple of 3 per color; leftovers => green)
        AssignColorsInTriplets(allScrews);

        // Safety sweep: any stray screws anywhere -> move to Screws + layer 10
        SweepAndEnforceScrewLayer(root, screwsParent);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(root.gameObject);
#endif

        Debug.Log($"[ScrewPuzzleLevelBuilder] Boards: {boards.Count}, Screws processed: {allScrews.Count}.");
    }

    // ---------------- Helpers ----------------
    private static Transform EnsureChild(string name, Transform parent)
    {
        var t = parent.Find(name);
        if (!t)
        {
            var go = new GameObject(name);
            t = go.transform;
            t.SetParent(parent, false);
        }
        return t;
    }

    private static bool IsScrewName(string n)
    {
        if (string.IsNullOrEmpty(n)) return false;
        n = n.Trim();
        if (n.StartsWith("Screw", StringComparison.OrdinalIgnoreCase)) return true;
        string lower = n.ToLowerInvariant();
        foreach (var cn in ColorNames)
        {
            if (lower == cn || lower.StartsWith(cn + " ") || lower.StartsWith(cn + "_") || lower.StartsWith(cn + "("))
                return true;
        }
        return false;
    }

    private void SweepAndEnforceScrewLayer(Transform root, Transform screwsParent)
    {
        var stray = root.GetComponentsInChildren<Transform>(true)
            .Where(t => t && t != screwsParent && t.parent != screwsParent && IsScrewName(t.name))
            .ToList();

        foreach (var t in stray)
        {
            t.SetParent(screwsParent, true);

            if (!t.TryGetComponent(out BlockedScrewsController _))
            {
#if UNITY_EDITOR
                UnityEditor.Undo.AddComponent<BlockedScrewsController>(t.gameObject);
#else
                t.gameObject.AddComponent<BlockedScrewsController>();
#endif
            }
            SetupSphereCollider(t.gameObject);
            SetLayerRecursively(t, coloredScrewLayer);

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(t.gameObject);
#endif
        }

        for (int i = 0; i < screwsParent.childCount; i++)
            SetLayerRecursively(screwsParent.GetChild(i), coloredScrewLayer);
    }

    private void SetupBoardComponents(GameObject board)
    {
        var mc = board.GetComponent<MeshCollider>();
        if (!mc) mc = board.AddComponent<MeshCollider>();
        mc.convex = true;
        mc.isTrigger = true;
        mc.sharedMesh = board.GetComponent<MeshFilter>()?.sharedMesh;

        if (!board.TryGetComponent(out BlockedMeshedController _))
        {
#if UNITY_EDITOR
            UnityEditor.Undo.AddComponent<BlockedMeshedController>(board);
#else
            board.AddComponent<BlockedMeshedController>();
#endif
        }

        var rb = board.GetComponent<Rigidbody>();
        if (!rb) rb = board.AddComponent<Rigidbody>();
        rb.mass = 1f;
        rb.linearDamping = 0f;
        rb.angularDamping = 0.05f;
        rb.useGravity = true;
        rb.isKinematic = true; // per request
        rb.interpolation = RigidbodyInterpolation.None;
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
    }

    private void SetupSphereCollider(GameObject go)
    {
        var sc = go.GetComponent<SphereCollider>();
        if (!sc) sc = go.AddComponent<SphereCollider>();
        sc.isTrigger = true;
        sc.center = Vector3.zero;
        sc.radius = 0.1f;

        try
        {
            var col = (Collider)sc;
            col.providesContacts = false;
            col.layerOverridePriority = 0;
            col.includeLayers = 0;
            col.excludeLayers = 0;
        }
        catch { }
    }

    private static void SetLayerRecursively(Transform t, int layer)
    {
        t.gameObject.layer = layer;
        for (int i = 0; i < t.childCount; i++)
            SetLayerRecursively(t.GetChild(i), layer);
    }

    // ---------- Assign the bolts LIST/ARRAY on BlockedMeshedController ----------
    // Supports List/Array of BlockedScrewsController, Transform, GameObject, or any Component.
    private void AssignBoltsList(GameObject board, List<Transform> screws)
    {
        var ctrl = board.GetComponent<BlockedMeshedController>();
        if (!ctrl) return;

        // Determine element type from the field or property, if possible.
        MemberInfo member;
        var elementType = GetBoltsElementType(ctrl.GetType(), out member);

        // Convert each screw Transform to the expected UnityEngine.Object
        var objects = new List<UnityEngine.Object>();
        foreach (var s in screws)
        {
            UnityEngine.Object obj = ConvertForElementType(s, elementType);
            if (obj != null) objects.Add(obj);
        }

        // Try reflection-first (works in playmode too)
        if (TryAssignViaReflection(ctrl, member, elementType, objects))
            return;

#if UNITY_EDITOR
        // Fallback: SerializedObject path (editor-only, more tolerant)
        var so = new UnityEditor.SerializedObject(ctrl);
        var prop = so.FindProperty("AllBlockingBolts") ??
                   so.FindProperty("allBlockingBolts") ??
                   so.FindProperty("m_AllBlockingBolts");

        if (prop != null && prop.isArray)
        {
            prop.arraySize = objects.Count;
            for (int i = 0; i < objects.Count; i++)
            {
                var el = prop.GetArrayElementAtIndex(i);
                el.objectReferenceValue = objects[i];
            }
            so.ApplyModifiedProperties();
            UnityEditor.EditorUtility.SetDirty(ctrl);
            return;
        }

        Debug.LogWarning($"[ScrewPuzzleLevelBuilder] Could not locate a serialized 'AllBlockingBolts' list/array on {ctrl.name}.", ctrl);
#endif
    }

    // Determine generic element type of AllBlockingBolts, if we can discover it.
    private static Type GetBoltsElementType(Type ctrlType, out MemberInfo member)
    {
        var names = new[] { "AllBlockingBolts", "allBlockingBolts", "m_AllBlockingBolts" };
        foreach (var n in names)
        {
            var f = ctrlType.GetField(n, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (f != null)
            {
                member = f;
                if (f.FieldType.IsArray) return f.FieldType.GetElementType();
                if (f.FieldType.IsGenericType) return f.FieldType.GetGenericArguments()[0];
                // Non-generic IList or something else: fall back to Component
                return typeof(Component);
            }

            var p = ctrlType.GetProperty(n, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (p != null)
            {
                member = p;
                var t = p.PropertyType;
                if (t.IsArray) return t.GetElementType();
                if (t.IsGenericType) return t.GetGenericArguments()[0];
                return typeof(Component);
            }
        }
        member = null;
        return typeof(Component); // best-effort default
    }

    private static UnityEngine.Object ConvertForElementType(Transform screw, Type elementType)
    {
        if (elementType == null) elementType = typeof(Component);

        // Exact/common types first
        if (typeof(BlockedScrewsController).IsAssignableFrom(elementType))
            return screw.GetComponent<BlockedScrewsController>();
        if (typeof(Transform).IsAssignableFrom(elementType))
            return screw;
        if (typeof(GameObject).IsAssignableFrom(elementType))
            return screw.gameObject;

        // Any other Component type
        if (typeof(Component).IsAssignableFrom(elementType))
            return screw.GetComponent(elementType);

        return null;
    }

    private static bool TryAssignViaReflection(object ctrl, MemberInfo member, Type elementType, List<UnityEngine.Object> objs)
    {
        if (member == null) return false;

        try
        {
            if (member is FieldInfo fi)
            {
                var ft = fi.FieldType;
                if (ft.IsArray)
                {
                    var arr = Array.CreateInstance(elementType, objs.Count);
                    for (int i = 0; i < objs.Count; i++) arr.SetValue(objs[i], i);
                    fi.SetValue(ctrl, arr);
                    return true;
                }

                // List<T> or other IList<T>
                if (ft.IsGenericType)
                {
                    var list = Activator.CreateInstance(ft);
                    var add = ft.GetMethod("Add");
                    foreach (var o in objs) add.Invoke(list, new object[] { o });
                    fi.SetValue(ctrl, list);
                    return true;
                }
            }
            else if (member is PropertyInfo pi && pi.CanWrite)
            {
                var pt = pi.PropertyType;
                if (pt.IsArray)
                {
                    var arr = Array.CreateInstance(elementType, objs.Count);
                    for (int i = 0; i < objs.Count; i++) arr.SetValue(objs[i], i);
                    pi.SetValue(ctrl, arr, null);
                    return true;
                }
                if (pt.IsGenericType)
                {
                    var list = Activator.CreateInstance(pt);
                    var add = pt.GetMethod("Add");
                    foreach (var o in objs) add.Invoke(list, new object[] { o });
                    pi.SetValue(ctrl, list, null);
                    return true;
                }
            }
        }
        catch { /* fall back below */ }

        return false;
    }

    // ---------- Coloring ----------
    private enum ScrewColor { Green, Blue, Yellow, Red }

    private void AssignColorsInTriplets(List<Transform> screws)
    {
        if (screws == null || screws.Count == 0) return;

        // Shuffle
        for (int i = 0; i < screws.Count; i++)
        {
            int j = UnityEngine.Random.Range(i, screws.Count);
            (screws[i], screws[j]) = (screws[j], screws[i]);
        }

        int total = screws.Count;
        int triplets = total / 3;
        int remainder = total % 3;
        if (remainder != 0)
            Debug.LogWarning($"[ScrewPuzzleLevelBuilder] Total screws {total} not divisible by 3; {remainder} leftover -> green.");

        int idx = 0;
        for (int t = 0; t < triplets; t++)
        {
            var color = (ScrewColor)UnityEngine.Random.Range(0, 4);
            for (int k = 0; k < 3; k++)
                ApplyColorAndName(screws[idx++].gameObject, color);
        }
        for (; idx < total; idx++)
            ApplyColorAndName(screws[idx].gameObject, ScrewColor.Green);
    }

    private void ApplyColorAndName(GameObject screw, ScrewColor col)
    {
        screw.name = col.ToString().ToLowerInvariant();

        var mr = screw.GetComponent<MeshRenderer>();
        if (!mr) return;

        Material m = null;
        switch (col)
        {
            case ScrewColor.Green: m = greenMat; break;
            case ScrewColor.Blue: m = blueMat; break;
            case ScrewColor.Yellow: m = yellowMat; break;
            case ScrewColor.Red: m = redMat; break;
        }
        if (m) mr.sharedMaterial = m;
    }
}
