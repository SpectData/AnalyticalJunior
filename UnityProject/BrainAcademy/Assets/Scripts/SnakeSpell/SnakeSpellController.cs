using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SnakeSpellController : MonoBehaviour
{
    // State (read by SnakeSpellUIController and BattlefieldRenderer)
    public DifficultyConfig Config { get; private set; }
    public BattlefieldState Battlefield { get; private set; }

    // Question state (wave phase)
    public GameQuestion.Standard CurrentQuestion { get; private set; }
    public bool Answered { get; private set; }
    public int SelectedAnswerIndex { get; private set; }
    public bool ShowCorrect { get; private set; }

    // Lightning bolt
    public bool HasLightningBolt => hasLightningBolt && Battlefield != null
        && Battlefield.phase == GamePhase.WavePhase;

    // Feedback
    public string FeedbackText { get; private set; }
    public bool FeedbackIsCorrect { get; private set; }
    public bool ShowFeedback { get; private set; }

    private QuestionProvider questionProvider;
    private AdaptiveDifficultyController adaptiveController;
    private int nextSnakeId;
    private int nextSpellId;
    private float spawnTimer;
    private int snakesSpawnedThisWave;
    private int totalSnakesThisWave;
    private bool navigatedToResults;

    // Phase state
    private List<ReviewItem> currentPhaseReviewItems = new List<ReviewItem>();
    private bool hasLightningBolt;
    private string currentPassageId;
    private GameQuestion.ReadingComprehension currentReadingQuestion;

    // UI controller references
    private ReadingPhaseUIController readingPhaseUI;
    private ReviewUIController reviewUI;

    void Start()
    {
        questionProvider = new QuestionProvider();
        questionProvider.Initialize();

        adaptiveController = new AdaptiveDifficultyController();
        Config = new DifficultyConfig(
            baseSnakeSpeed: 45f,
            baseSpawnInterval: 3.0f,
            startingLives: 4,
            availableSnakeTypes: new List<SnakeType>
                { SnakeType.Green, SnakeType.Yellow, SnakeType.Red, SnakeType.Purple },
            pointsPerKill: 25
        );

        readingPhaseUI = FindObjectOfType<ReadingPhaseUIController>();
        reviewUI = FindObjectOfType<ReviewUIController>();

        if (readingPhaseUI != null)
            readingPhaseUI.OnAnswerSubmitted += OnReadingAnswerSubmitted;

        StartGame();
    }

    private void StartGame()
    {
        nextSnakeId = 0;
        nextSpellId = 0;
        spawnTimer = 0f;
        snakesSpawnedThisWave = 0;
        navigatedToResults = false;
        questionProvider.ResetBank();
        currentPhaseReviewItems = new List<ReviewItem>();
        hasLightningBolt = false;
        currentPassageId = null;
        currentReadingQuestion = null;

        Battlefield = new BattlefieldState
        {
            status = GameStatus.Playing,
            phase = GamePhase.WavePhase,
            lives = Config.startingLives,
        };

        StartWave(1);
        GenerateNextQuestion();
    }

    void Update()
    {
        if (Battlefield == null) return;
        if (Battlefield.status == GameStatus.GameOver)
        {
            if (!navigatedToResults)
            {
                navigatedToResults = true;
                SaveResultsAndNavigate();
            }
            return;
        }

        if (Battlefield.status != GameStatus.Playing) return;

        float dt = Time.deltaTime;

        // Only WavePhase has per-frame updates; other phases are callback-driven
        if (Battlefield.phase == GamePhase.WavePhase)
            UpdateWavePhase(dt);
    }

    private void UpdateWavePhase(float dt)
    {
        // Move snakes inward (decreasing distance)
        foreach (var snake in Battlefield.snakes)
            snake.distance -= snake.speed * dt;

        // Move spells outward (increasing distance)
        foreach (var spell in Battlefield.spells)
            spell.distance += spell.speed * dt;

        // Check snakes reaching wizard
        var reached = Battlefield.snakes.Where(s => s.distance <= SnakeSpellConstants.WizardHitRadius).ToList();
        foreach (var snake in reached)
            Battlefield.snakes.Remove(snake);
        Battlefield.lives -= reached.Count;

        // Check spell-snake collisions
        var spellsToRemove = new HashSet<int>();
        foreach (var spell in Battlefield.spells)
        {
            var target = Battlefield.snakes.Find(s => s.id == spell.targetSnakeId);
            if (target != null && spell.distance >= target.distance)
            {
                spellsToRemove.Add(spell.id);
                target.hp -= 1;
                if (target.hp <= 0)
                {
                    Battlefield.snakes.Remove(target);
                    Battlefield.score += Config.pointsPerKill;
                    Battlefield.snakesKilled++;
                }
            }

            if (spell.distance > SnakeSpellConstants.SpellDespawnRadius)
                spellsToRemove.Add(spell.id);
        }
        Battlefield.spells.RemoveAll(s => spellsToRemove.Contains(s.id));

        // Spawn snakes
        spawnTimer -= dt;
        if (spawnTimer <= 0f && snakesSpawnedThisWave < totalSnakesThisWave)
        {
            float angleDeg = WaveGenerator.PickAngle();
            SnakeType type = WaveGenerator.PickSnakeType(Battlefield.currentWave, Config);
            float speed = WaveGenerator.GetSnakeSpeed(type, Battlefield.currentWave, Config);

            Battlefield.snakes.Add(new SnakeData(
                id: nextSnakeId++,
                angleDeg: angleDeg,
                type: type,
                distance: SnakeSpellConstants.FieldRadius,
                hp: SnakeTypeData.GetBaseHp(type),
                speed: speed
            ));

            snakesSpawnedThisWave++;
            spawnTimer = 1f / adaptiveController.GetSpawnRate();
        }

        // Check wave complete
        bool waveComplete = snakesSpawnedThisWave >= totalSnakesThisWave && Battlefield.snakes.Count == 0;

        if (Battlefield.lives <= 0)
        {
            Battlefield.lives = 0;
            Battlefield.status = GameStatus.GameOver;
        }
        else if (waveComplete)
        {
            EnterWaveReview();
        }
    }

    private void StartWave(int waveNumber)
    {
        totalSnakesThisWave = WaveGenerator.GetSnakeCountForWave(waveNumber);
        snakesSpawnedThisWave = 0;
        spawnTimer = 1.5f;
        Battlefield.currentWave = waveNumber;
    }

    // ── Phase Transitions ──

    private void EnterWaveReview()
    {
        // Lightning bolt expires at end of wave (use-it-or-lose-it)
        hasLightningBolt = false;
        Battlefield.hasLightningBolt = false;

        Battlefield.phase = GamePhase.WaveReview;

        if (reviewUI != null)
        {
            reviewUI.Show(currentPhaseReviewItems, () =>
            {
                currentPhaseReviewItems = new List<ReviewItem>();
                EnterInterWavePhase();
            });
        }
        else
        {
            currentPhaseReviewItems = new List<ReviewItem>();
            EnterInterWavePhase();
        }
    }

    private void EnterInterWavePhase()
    {
        Battlefield.phase = GamePhase.InterWavePhase;

        // Try to get a reading question for the current passage
        GameQuestion.ReadingComprehension readingQ = null;
        if (!string.IsNullOrEmpty(currentPassageId))
            readingQ = questionProvider.GetReadingQuestionForPassage(currentPassageId);

        // If passage exhausted or no current passage, get a new one
        if (readingQ == null)
            readingQ = questionProvider.GetReadingQuestion();

        if (readingQ != null && readingPhaseUI != null)
        {
            currentPassageId = readingQ.passageId;
            currentReadingQuestion = readingQ;
            readingPhaseUI.ShowPanel(readingQ);
        }
        else
        {
            // No reading questions available — skip to next wave
            currentPhaseReviewItems = new List<ReviewItem>();
            StartNextWave();
        }
    }

    private void OnReadingAnswerSubmitted(bool isCorrect, int selectedIndex)
    {
        hasLightningBolt = isCorrect;
        Battlefield.hasLightningBolt = isCorrect;
        adaptiveController.RecordAnswer(isCorrect);

        // Collect review item for wrong answers
        if (!isCorrect && currentReadingQuestion != null)
        {
            Question q = currentReadingQuestion.question;
            string studentAnswer = (selectedIndex >= 0 && selectedIndex < q.answers.Count)
                ? q.answers[selectedIndex]
                : "(unknown)";

            currentPhaseReviewItems.Add(new ReviewItem(
                questionText: q.questionText,
                studentAnswer: studentAnswer,
                correctAnswer: q.correctAnswer,
                explanation: q.explanation ?? ""
            ));
        }

        StartCoroutine(DelayedEnterInterWaveReview());
    }

    private IEnumerator DelayedEnterInterWaveReview()
    {
        yield return new WaitForSeconds(1.5f);
        if (readingPhaseUI != null)
            readingPhaseUI.HidePanel();
        EnterInterWaveReview();
    }

    private void EnterInterWaveReview()
    {
        Battlefield.phase = GamePhase.InterWaveReview;

        if (reviewUI != null)
        {
            reviewUI.Show(currentPhaseReviewItems, () =>
            {
                currentPhaseReviewItems = new List<ReviewItem>();
                StartNextWave();
            });
        }
        else
        {
            currentPhaseReviewItems = new List<ReviewItem>();
            StartNextWave();
        }
    }

    private void StartNextWave()
    {
        int nextWave = Battlefield.currentWave + 1;
        Battlefield.phase = GamePhase.WavePhase;
        Battlefield.hasLightningBolt = hasLightningBolt;
        StartWave(nextWave);
        GenerateNextQuestion();
    }

    // ── Question Handling (Wave Phase) ──

    private void GenerateNextQuestion()
    {
        CurrentQuestion = questionProvider.GetSnakeSpellQuestion();
        Answered = false;
        SelectedAnswerIndex = -1;
        ShowCorrect = false;
    }

    public void SubmitAnswer(int index)
    {
        if (Answered) return;
        if (CurrentQuestion == null) return;
        Question q = CurrentQuestion.question;

        Answered = true;
        SelectedAnswerIndex = index;
        ShowCorrect = true;

        bool isCorrect = q.answers[index] == q.correctAnswer;
        Battlefield.questionsAnswered++;
        if (isCorrect) Battlefield.questionsCorrect++;
        adaptiveController.RecordAnswer(isCorrect);

        // Collect review item for wrong answers
        if (!isCorrect)
        {
            currentPhaseReviewItems.Add(new ReviewItem(
                questionText: q.questionText,
                studentAnswer: q.answers[index],
                correctAnswer: q.correctAnswer,
                explanation: q.explanation ?? ""
            ));
        }

        if (isCorrect)
        {
            FireSpell();
            DisplayFeedback("Spell cast!", true);
            StartCoroutine(ScheduleNextQuestion(0.4f));
        }
        else
        {
            DisplayFeedback("Miss!", false);
            StartCoroutine(ScheduleNextQuestion(0.7f));
        }
    }

    private void FireSpell()
    {
        var nearestSnake = Battlefield.snakes
            .Where(s => s.hp > 0)
            .OrderBy(s => s.distance)
            .FirstOrDefault();

        if (nearestSnake == null) return;

        Battlefield.spells.Add(new SpellData(
            id: nextSpellId++,
            angleDeg: nearestSnake.angleDeg,
            distance: 0f,
            targetSnakeId: nearestSnake.id
        ));
    }

    private IEnumerator ScheduleNextQuestion(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (Battlefield.status == GameStatus.Playing && Battlefield.phase == GamePhase.WavePhase)
            GenerateNextQuestion();
    }

    // ── Lightning Bolt ──

    public void UseLightningBolt()
    {
        if (!hasLightningBolt) return;
        if (Battlefield.phase != GamePhase.WavePhase) return;

        hasLightningBolt = false;
        Battlefield.hasLightningBolt = false;

        var nearest = Battlefield.snakes
            .Where(s => s.hp > 0)
            .OrderBy(s => s.distance)
            .Take(SnakeSpellConstants.LightningBoltZapCount)
            .ToList();

        foreach (var snake in nearest)
        {
            Battlefield.snakes.Remove(snake);
            Battlefield.score += Config.pointsPerKill;
            Battlefield.snakesKilled++;
        }

        DisplayFeedback($"Lightning! {nearest.Count} zapped!", true);
    }

    // ── Feedback ──

    private void DisplayFeedback(string text, bool correct)
    {
        FeedbackText = text;
        FeedbackIsCorrect = correct;
        ShowFeedback = true;
        StartCoroutine(HideFeedback());
    }

    private IEnumerator HideFeedback()
    {
        yield return new WaitForSeconds(0.8f);
        ShowFeedback = false;
    }

    // ── Results ──

    private void SaveResultsAndNavigate()
    {
        GameManager gm = GameManager.Instance;
        gm.LastSnakeScore = Battlefield.score;
        gm.LastSnakeWave = Battlefield.currentWave - 1;
        gm.LastSnakeKills = Battlefield.snakesKilled;
        gm.LastSnakeQuestionsAnswered = Battlefield.questionsAnswered;
        gm.LastSnakeQuestionsCorrect = Battlefield.questionsCorrect;

        StartCoroutine(DelayedNavigateToResults());
    }

    private IEnumerator DelayedNavigateToResults()
    {
        yield return new WaitForSeconds(1.5f);
        SceneTransitionManager.Instance.LoadSnakeResults();
    }

    void OnDestroy()
    {
        if (readingPhaseUI != null)
            readingPhaseUI.OnAnswerSubmitted -= OnReadingAnswerSubmitted;
        StopAllCoroutines();
    }
}
