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
    private const string SpriteDir = "Assets/Sprites";

    public static GameObject AnswerButtonPrefab { get; private set; }
    public static GameObject MemoryCellPrefab { get; private set; }
    public static GameObject SnakePrefab { get; private set; }
    public static GameObject SpellPrefab { get; private set; }

    // Loaded sprites (null if files don't exist yet)
    public static Sprite WizardSprite { get; private set; }
    public static Sprite SnakeGreenSprite { get; private set; }
    public static Sprite SnakeYellowSprite { get; private set; }
    public static Sprite SnakeRedSprite { get; private set; }
    public static Sprite SnakePurpleSprite { get; private set; }
    public static Sprite SpellSprite { get; private set; }
    public static Sprite GrassLightSprite { get; private set; }
    public static Sprite GrassDarkSprite { get; private set; }

    public static void CreateAll()
    {
        if (!AssetDatabase.IsValidFolder(PrefabDir))
            AssetDatabase.CreateFolder("Assets", "Prefabs");

        LoadSprites();

        AnswerButtonPrefab = CreateAnswerButton();
        MemoryCellPrefab = CreateMemoryCell();
        SnakePrefab = CreateSnake();
        SpellPrefab = CreateSpell();

        AssetDatabase.SaveAssets();
        Debug.Log("[Setup] Prefabs created in Assets/Prefabs/");
    }

    private static void LoadSprites()
    {
        // Force Unity to detect any new files added outside the editor
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        WizardSprite = LoadSprite("wizard");
        SnakeGreenSprite = LoadSprite("snake_green");
        SnakeYellowSprite = LoadSprite("snake_yellow");
        SnakeRedSprite = LoadSprite("snake_red");
        SnakePurpleSprite = LoadSprite("snake_purple");
        SpellSprite = LoadSprite("spell");
        GrassLightSprite = LoadSprite("grass_light");
        GrassDarkSprite = LoadSprite("grass_dark");

        int loaded = 0;
        if (WizardSprite != null) loaded++;
        if (SnakeGreenSprite != null) loaded++;
        if (SnakeYellowSprite != null) loaded++;
        if (SnakeRedSprite != null) loaded++;
        if (SnakePurpleSprite != null) loaded++;
        if (SpellSprite != null) loaded++;
        if (GrassLightSprite != null) loaded++;
        if (GrassDarkSprite != null) loaded++;
        Debug.Log($"[Setup] Loaded {loaded}/8 sprites from {SpriteDir}/");
    }

    private static Sprite LoadSprite(string name)
    {
        string path = $"{SpriteDir}/{name}.png";

        // Ensure the texture is imported as Sprite type (Unity defaults to Texture2D)
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null && importer.textureType != TextureImporterType.Sprite)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.SaveAndReimport();
        }

        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
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
        rt.sizeDelta = new Vector2(80, 80);

        var img = root.GetComponent<Image>();
        if (SnakeGreenSprite != null)
        {
            img.sprite = SnakeGreenSprite;
            img.color = Color.white;
        }
        else
        {
            img.color = Color.green;
        }

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
        rt.sizeDelta = new Vector2(30, 30);

        var img = root.GetComponent<Image>();
        if (SpellSprite != null)
        {
            img.sprite = SpellSprite;
            img.color = Color.white;
        }
        else
        {
            img.color = spellGold;
        }

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
