using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Creates and saves the 4 prefabs required by the game scripts.
/// </summary>
public static class PrefabFactory
{
    private const string PrefabDir = "Assets/Prefabs";

    public static GameObject AnswerButtonPrefab { get; private set; }
    public static GameObject MemoryCellPrefab { get; private set; }
    public static GameObject SnakePrefab { get; private set; }
    public static GameObject SpellPrefab { get; private set; }

    public static void CreateAll()
    {
        if (!AssetDatabase.IsValidFolder(PrefabDir))
            AssetDatabase.CreateFolder("Assets", "Prefabs");

        AnswerButtonPrefab = CreateAnswerButton();
        MemoryCellPrefab = CreateMemoryCell();
        SnakePrefab = CreateSnake();
        SpellPrefab = CreateSpell();

        AssetDatabase.SaveAssets();
        Debug.Log("[Setup] Prefabs created in Assets/Prefabs/");
    }

    // ── AnswerButton ────────────────────────────────────────────────────

    private static GameObject CreateAnswerButton()
    {
        var root = new GameObject("AnswerButton", typeof(Image), typeof(Button));
        var rt = root.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(960, 80);

        root.GetComponent<Image>().color = Color.white;

        // Border image (behind background, slightly larger)
        var border = new GameObject("Border", typeof(Image));
        border.transform.SetParent(root.transform, false);
        border.GetComponent<Image>().color = new Color(0.85f, 0.85f, 0.85f);
        UIFactory.Stretch(border);

        // Answer text
        var textGo = new GameObject("AnswerText", typeof(TextMeshProUGUI));
        textGo.transform.SetParent(root.transform, false);
        var tmp = textGo.GetComponent<TextMeshProUGUI>();
        tmp.text = "Answer";
        tmp.fontSize = 28;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.black;
        UIFactory.Stretch(textGo);

        // Attach script and wire
        var comp = root.AddComponent<AnswerButtonUI>();
        UIFactory.Wire(comp, "button", root.GetComponent<Button>());
        UIFactory.Wire(comp, "backgroundImage", root.GetComponent<Image>());
        UIFactory.Wire(comp, "borderImage", border.GetComponent<Image>());
        UIFactory.Wire(comp, "answerText", tmp);

        return SavePrefab(root, "AnswerButton");
    }

    // ── MemoryCell ──────────────────────────────────────────────────────

    private static GameObject CreateMemoryCell()
    {
        var root = new GameObject("MemoryCell", typeof(Image), typeof(Button));
        var rt = root.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(80, 80);

        root.GetComponent<Image>().color = new Color(0.95f, 0.95f, 0.95f);

        // Border
        var border = new GameObject("Border", typeof(Image));
        border.transform.SetParent(root.transform, false);
        border.GetComponent<Image>().color = new Color(0.85f, 0.85f, 0.85f);
        UIFactory.Stretch(border);

        // Attach script and wire
        var comp = root.AddComponent<MemoryCellUI>();
        UIFactory.Wire(comp, "backgroundImage", root.GetComponent<Image>());
        UIFactory.Wire(comp, "borderImage", border.GetComponent<Image>());
        UIFactory.Wire(comp, "button", root.GetComponent<Button>());

        return SavePrefab(root, "MemoryCell");
    }

    // ── Snake ───────────────────────────────────────────────────────────

    private static GameObject CreateSnake()
    {
        var root = new GameObject("SnakePrefab", typeof(Image));
        var rt = root.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(40, 40);

        var img = root.GetComponent<Image>();
        img.color = Color.green;
        // Circle sprite will be assigned at runtime by BattlefieldRenderer

        // HP bar child
        var hpBar = new GameObject("HPBar", typeof(Image));
        hpBar.transform.SetParent(root.transform, false);
        var hpImg = hpBar.GetComponent<Image>();
        hpImg.color = Color.green;
        hpImg.type = Image.Type.Filled;
        hpImg.fillMethod = Image.FillMethod.Horizontal;
        hpImg.fillAmount = 1f;

        var hpRt = hpBar.GetComponent<RectTransform>();
        hpRt.anchorMin = new Vector2(0, 0);
        hpRt.anchorMax = new Vector2(1, 0);
        hpRt.pivot = new Vector2(0.5f, 0);
        hpRt.anchoredPosition = new Vector2(0, -5);
        hpRt.sizeDelta = new Vector2(0, 6);

        return SavePrefab(root, "SnakePrefab");
    }

    // ── Spell ───────────────────────────────────────────────────────────

    private static GameObject CreateSpell()
    {
        // Gold: #E8D44D
        Color spellGold = new Color(0.91f, 0.83f, 0.30f);

        var root = new GameObject("SpellPrefab", typeof(Image));
        var rt = root.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(20, 20);

        var img = root.GetComponent<Image>();
        img.color = spellGold;

        // Glow child (larger, semi-transparent)
        var glow = new GameObject("Glow", typeof(Image));
        glow.transform.SetParent(root.transform, false);
        var glowImg = glow.GetComponent<Image>();
        glowImg.color = new Color(spellGold.r, spellGold.g, spellGold.b, 0.3f);

        var glowRt = glow.GetComponent<RectTransform>();
        glowRt.anchorMin = new Vector2(0.5f, 0.5f);
        glowRt.anchorMax = new Vector2(0.5f, 0.5f);
        glowRt.sizeDelta = new Vector2(36, 36);

        return SavePrefab(root, "SpellPrefab");
    }

    // ── Save helper ─────────────────────────────────────────────────────

    private static GameObject SavePrefab(GameObject instance, string name)
    {
        string path = $"{PrefabDir}/{name}.prefab";
        var prefab = PrefabUtility.SaveAsPrefabAsset(instance, path);
        Object.DestroyImmediate(instance);
        Debug.Log($"  Created prefab: {path}");
        return prefab;
    }
}
