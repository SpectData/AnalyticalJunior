using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Shared helpers for creating UI elements and wiring SerializeField references.
/// </summary>
public static class UIFactory
{
    // ── Canvas ──────────────────────────────────────────────────────────

    public static GameObject CreateCanvas(string name)
    {
        // Camera — required for Game view to render anything
        var camGo = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
        var cam = camGo.GetComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.94f, 0.96f, 0.97f); // #F0F4F8
        cam.orthographic = true;
        camGo.tag = "MainCamera";

        // EventSystem — required for button/touch input
        var es = new GameObject("EventSystem",
            typeof(UnityEngine.EventSystems.EventSystem),
            typeof(UnityEngine.EventSystems.StandaloneInputModule));

        var go = new GameObject(name, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));

        var canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = go.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        return go;
    }

    // ── RectTransform helpers ───────────────────────────────────────────

    public static RectTransform Stretch(GameObject go)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        return rt;
    }

    public static RectTransform SetRect(GameObject go,
        Vector2 anchorMin, Vector2 anchorMax,
        Vector2 pivot, Vector2 anchoredPos, Vector2 sizeDelta)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = pivot;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = sizeDelta;
        return rt;
    }

    // ── Background ──────────────────────────────────────────────────────

    public static GameObject CreateBackground(Transform parent, Color color)
    {
        var go = new GameObject("Background", typeof(Image));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = color;
        Stretch(go);
        return go;
    }

    // ── Image ───────────────────────────────────────────────────────────

    public static GameObject CreateImage(Transform parent, string name,
        Color color, Vector2 size)
    {
        var go = new GameObject(name, typeof(Image));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = color;
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = size;
        return go;
    }

    public static GameObject CreateFilledImage(Transform parent, string name,
        Color color, Image.FillMethod method = Image.FillMethod.Horizontal)
    {
        var go = new GameObject(name, typeof(Image));
        go.transform.SetParent(parent, false);
        var img = go.GetComponent<Image>();
        img.color = color;
        img.type = Image.Type.Filled;
        img.fillMethod = method;
        img.fillAmount = 1f;
        return go;
    }

    // ── Panel ───────────────────────────────────────────────────────────

    public static GameObject CreatePanel(Transform parent, string name,
        Color bgColor)
    {
        var go = new GameObject(name, typeof(Image));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = bgColor;
        return go;
    }

    // ── TextMeshPro ─────────────────────────────────────────────────────

    private static TMP_FontAsset _cachedFont;

    private static TMP_FontAsset FindDefaultFont()
    {
        if (_cachedFont != null) return _cachedFont;

        // Try TMP_Settings first (works in Editor GUI, throws in batch mode)
        try { _cachedFont = TMP_Settings.defaultFontAsset; }
        catch { /* TMP_Settings not initialized in batch mode */ }
        if (_cachedFont != null) return _cachedFont;

        // Fallback: search AssetDatabase for any TMP font
        var guids = AssetDatabase.FindAssets("t:TMP_FontAsset");
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            _cachedFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
            if (_cachedFont != null) return _cachedFont;
        }

        Debug.LogWarning("[Setup] No TMP font found — text will be invisible until TMP Essential Resources are imported");
        return null;
    }

    public static TextMeshProUGUI CreateTMP(Transform parent, string name,
        string text, int fontSize = 32,
        TextAlignmentOptions align = TextAlignmentOptions.Center,
        Color? color = null)
    {
        var go = new GameObject(name, typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        var tmp = go.GetComponent<TextMeshProUGUI>();
        var font = FindDefaultFont();
        if (font != null) tmp.font = font;
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = align;
        tmp.color = color ?? Color.black;
        return tmp;
    }

    // ── Button ──────────────────────────────────────────────────────────

    public static Button CreateButton(Transform parent, string name,
        string label, Color bgColor, int fontSize = 28)
    {
        var go = new GameObject(name, typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = bgColor;

        var tmpGo = new GameObject("Text", typeof(TextMeshProUGUI));
        tmpGo.transform.SetParent(go.transform, false);
        var tmp = tmpGo.GetComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        Stretch(tmpGo);

        return go.GetComponent<Button>();
    }

    // ── FeedbackOverlay ─────────────────────────────────────────────────

    public static (GameObject go, FeedbackOverlay comp, TextMeshProUGUI text)
        CreateFeedbackOverlay(Transform parent)
    {
        var go = new GameObject("FeedbackOverlay", typeof(RectTransform), typeof(CanvasGroup));
        go.transform.SetParent(parent, false);
        Stretch(go);
        go.GetComponent<CanvasGroup>().alpha = 0f;
        go.GetComponent<CanvasGroup>().blocksRaycasts = false;

        var feedbackComp = go.AddComponent<FeedbackOverlay>();

        var text = CreateTMP(go.transform, "FeedbackText", "",
            fontSize: 48, color: Color.white);
        Stretch(text.gameObject);

        Wire(feedbackComp, "feedbackText", text);
        Wire(feedbackComp, "canvasGroup", go.GetComponent<CanvasGroup>());

        return (go, feedbackComp, text);
    }

    // ── AnswerButton instances (from prefab) ────────────────────────────

    public static AnswerButtonUI[] CreateAnswerButtons(Transform parent,
        GameObject prefab, int count = 4)
    {
        var buttons = new AnswerButtonUI[count];
        for (int i = 0; i < count; i++)
        {
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            instance.name = $"AnswerButton{i}";
            buttons[i] = instance.GetComponent<AnswerButtonUI>();
        }
        return buttons;
    }

    // ── Wiring helpers (SerializedObject for private [SerializeField]) ──

    public static void Wire(Component target, string fieldName,
        Object value)
    {
        var so = new SerializedObject(target);
        var prop = so.FindProperty(fieldName);
        if (prop == null)
        {
            Debug.LogWarning($"Field '{fieldName}' not found on {target.GetType().Name}");
            return;
        }
        prop.objectReferenceValue = value;
        so.ApplyModifiedProperties();
    }

    public static void WireList(Component target, string fieldName,
        Object[] values)
    {
        var so = new SerializedObject(target);
        var prop = so.FindProperty(fieldName);
        if (prop == null)
        {
            Debug.LogWarning($"List '{fieldName}' not found on {target.GetType().Name}");
            return;
        }
        prop.arraySize = values.Length;
        for (int i = 0; i < values.Length; i++)
            prop.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
        so.ApplyModifiedProperties();
    }
}
