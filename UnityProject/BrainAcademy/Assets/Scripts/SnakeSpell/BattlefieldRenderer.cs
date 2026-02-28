using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlefieldRenderer : MonoBehaviour
{
    [Header("Battlefield Panel")]
    [SerializeField] private RectTransform battlefieldPanel;

    [Header("Lane Backgrounds")]
    [SerializeField] private List<Image> laneBackgrounds;
    [SerializeField] private List<Image> dirtRoads;

    [Header("Wizards")]
    [SerializeField] private List<RectTransform> wizardObjects;

    [Header("Prefabs")]
    [SerializeField] private GameObject snakePrefab;
    [SerializeField] private GameObject spellPrefab;

    [Header("Overlays")]
    [SerializeField] private GameObject waveTransitionOverlay;
    [SerializeField] private TMPro.TextMeshProUGUI waveTransitionText;
    [SerializeField] private GameObject gameOverOverlay;

    // Object pools
    private Dictionary<int, RectTransform> activeSnakeObjects = new Dictionary<int, RectTransform>();
    private Dictionary<int, RectTransform> activeSpellObjects = new Dictionary<int, RectTransform>();
    private Queue<RectTransform> snakePool = new Queue<RectTransform>();
    private Queue<RectTransform> spellPool = new Queue<RectTransform>();

    private SnakeSpellController controller;
    private static Sprite circleSprite;

    void Start()
    {
        controller = FindObjectOfType<SnakeSpellController>();
        GenerateCircleSprite();

        // Set lane colors
        for (int i = 0; i < laneBackgrounds.Count && i < SnakeSpellConstants.NumLanes; i++)
        {
            laneBackgrounds[i].color = (i % 2 == 0) ? AppColors.GrassLight : AppColors.GrassDark;
        }

        // Set dirt road colors
        foreach (var road in dirtRoads)
            road.color = AppColors.DirtRoad;
    }

    void Update()
    {
        if (controller == null || controller.Battlefield == null) return;

        BattlefieldState bf = controller.Battlefield;
        RenderSnakes(bf);
        RenderSpells(bf);
        UpdateOverlays(bf);
    }

    private void RenderSnakes(BattlefieldState bf)
    {
        // Track which snake IDs are still alive
        HashSet<int> aliveIds = new HashSet<int>();

        foreach (var snake in bf.snakes)
        {
            aliveIds.Add(snake.id);

            RectTransform snakeRT;
            if (!activeSnakeObjects.TryGetValue(snake.id, out snakeRT))
            {
                snakeRT = GetFromPool(snakePool, snakePrefab);
                activeSnakeObjects[snake.id] = snakeRT;
            }

            // Position snake
            Vector2 pos = GameToScreenPosition(snake.xPosition, snake.lane);
            snakeRT.anchoredPosition = pos;

            // Set color based on snake type
            Image snakeImage = snakeRT.GetComponent<Image>();
            if (snakeImage != null)
                snakeImage.color = SnakeTypeData.GetBodyColor(snake.type);

            // Update HP bar if present
            Image hpBar = snakeRT.Find("HPBar")?.GetComponent<Image>();
            if (hpBar != null)
            {
                int maxHp = SnakeTypeData.GetBaseHp(snake.type);
                hpBar.fillAmount = (float)snake.hp / maxHp;
                float ratio = (float)snake.hp / maxHp;
                hpBar.color = ratio > 0.6f ? AppColors.EasyGreen :
                              ratio > 0.3f ? AppColors.MediumYellow : AppColors.HardRed;
            }

            snakeRT.gameObject.SetActive(true);
        }

        // Remove dead snakes
        var toRemove = new List<int>();
        foreach (var kvp in activeSnakeObjects)
        {
            if (!aliveIds.Contains(kvp.Key))
            {
                ReturnToPool(snakePool, kvp.Value);
                toRemove.Add(kvp.Key);
            }
        }
        foreach (int id in toRemove)
            activeSnakeObjects.Remove(id);
    }

    private void RenderSpells(BattlefieldState bf)
    {
        HashSet<int> activeIds = new HashSet<int>();

        foreach (var spell in bf.spells)
        {
            activeIds.Add(spell.id);

            RectTransform spellRT;
            if (!activeSpellObjects.TryGetValue(spell.id, out spellRT))
            {
                spellRT = GetFromPool(spellPool, spellPrefab);
                activeSpellObjects[spell.id] = spellRT;
            }

            Vector2 pos = GameToScreenPosition(spell.xPosition, spell.lane);
            spellRT.anchoredPosition = pos;

            Image spellImage = spellRT.GetComponent<Image>();
            if (spellImage != null)
                spellImage.color = AppColors.SpellGold;

            spellRT.gameObject.SetActive(true);
        }

        var toRemove = new List<int>();
        foreach (var kvp in activeSpellObjects)
        {
            if (!activeIds.Contains(kvp.Key))
            {
                ReturnToPool(spellPool, kvp.Value);
                toRemove.Add(kvp.Key);
            }
        }
        foreach (int id in toRemove)
            activeSpellObjects.Remove(id);
    }

    private void UpdateOverlays(BattlefieldState bf)
    {
        if (waveTransitionOverlay != null)
        {
            bool showTransition = bf.status == GameStatus.WaveTransition;
            waveTransitionOverlay.SetActive(showTransition);
            if (showTransition && waveTransitionText != null)
                waveTransitionText.text = $"Wave {bf.currentWave} Complete!";
        }

        if (gameOverOverlay != null)
            gameOverOverlay.SetActive(bf.status == GameStatus.GameOver);
    }

    private Vector2 GameToScreenPosition(float gameX, int lane)
    {
        if (battlefieldPanel == null) return Vector2.zero;

        float panelWidth = battlefieldPanel.rect.width;
        float panelHeight = battlefieldPanel.rect.height;

        float scaleX = panelWidth / SnakeSpellConstants.FieldWidth;
        float laneHeight = panelHeight / SnakeSpellConstants.NumLanes;

        float screenX = gameX * scaleX;
        float screenY = -(lane * laneHeight + laneHeight / 2f);

        return new Vector2(screenX, screenY);
    }

    // ── Object Pooling ──

    private RectTransform GetFromPool(Queue<RectTransform> pool, GameObject prefab)
    {
        if (pool.Count > 0)
        {
            var obj = pool.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }

        if (prefab == null) return null;
        GameObject newObj = Instantiate(prefab, battlefieldPanel);
        return newObj.GetComponent<RectTransform>();
    }

    private void ReturnToPool(Queue<RectTransform> pool, RectTransform obj)
    {
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }

    // ── Sprite Generation ──

    public static Sprite GetCircleSprite()
    {
        if (circleSprite == null) GenerateCircleSprite();
        return circleSprite;
    }

    private static void GenerateCircleSprite()
    {
        if (circleSprite != null) return;

        int diameter = 64;
        Texture2D tex = new Texture2D(diameter, diameter, TextureFormat.RGBA32, false);
        float radius = diameter / 2f;
        Color clear = new Color(0, 0, 0, 0);

        for (int x = 0; x < diameter; x++)
        {
            for (int y = 0; y < diameter; y++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(radius, radius));
                tex.SetPixel(x, y, dist <= radius ? Color.white : clear);
            }
        }

        tex.Apply();
        circleSprite = Sprite.Create(tex, new Rect(0, 0, diameter, diameter), Vector2.one * 0.5f);
    }
}
