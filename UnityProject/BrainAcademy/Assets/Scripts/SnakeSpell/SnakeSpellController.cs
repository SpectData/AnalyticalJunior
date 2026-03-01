using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SnakeSpellController : MonoBehaviour
{
    // State (read by SnakeSpellUIController and BattlefieldRenderer)
    public DifficultyConfig Config { get; private set; }
    public BattlefieldState Battlefield { get; private set; }

    // Question state
    public GameQuestion.Standard CurrentQuestion { get; private set; }
    public int QuestionTimeLeft { get; private set; }
    public bool Answered { get; private set; }
    public int SelectedAnswerIndex { get; private set; }
    public bool ShowCorrect { get; private set; }

    // Feedback
    public string FeedbackText { get; private set; }
    public bool FeedbackIsCorrect { get; private set; }
    public bool ShowFeedback { get; private set; }

    private QuestionProvider questionProvider;
    private Difficulty difficulty;
    private int nextSnakeId;
    private int nextSpellId;
    private float spawnTimer;
    private int snakesSpawnedThisWave;
    private int totalSnakesThisWave;
    private float waveTransitionCountdown;
    private Coroutine questionTimerCoroutine;
    private bool navigatedToResults;

    void Start()
    {
        questionProvider = new QuestionProvider();
        questionProvider.Initialize();

        difficulty = GameManager.Instance.SelectedDifficulty;
        Config = difficulty.ToSnakeSpellConfig();
        StartGame();
    }

    private void StartGame()
    {
        nextSnakeId = 0;
        nextSpellId = 0;
        spawnTimer = 0f;
        snakesSpawnedThisWave = 0;
        waveTransitionCountdown = 0f;
        navigatedToResults = false;
        questionProvider.ResetBank();

        Battlefield = new BattlefieldState
        {
            status = GameStatus.Playing,
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

        float dt = Time.deltaTime;

        if (Battlefield.status == GameStatus.Playing)
            UpdatePlaying(dt);
        else if (Battlefield.status == GameStatus.WaveTransition)
            UpdateWaveTransition(dt);
    }

    private void UpdatePlaying(float dt)
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
            spawnTimer = WaveGenerator.GetSpawnInterval(Battlefield.currentWave, Config);
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
            waveTransitionCountdown = 3f;
            Battlefield.status = GameStatus.WaveTransition;
        }
    }

    private void UpdateWaveTransition(float dt)
    {
        waveTransitionCountdown -= dt;
        Battlefield.waveTransitionTimer = waveTransitionCountdown;

        if (waveTransitionCountdown <= 0f)
        {
            int nextWave = Battlefield.currentWave + 1;
            Battlefield.status = GameStatus.Playing;
            StartWave(nextWave);
        }
    }

    private void StartWave(int waveNumber)
    {
        totalSnakesThisWave = WaveGenerator.GetSnakeCountForWave(waveNumber);
        snakesSpawnedThisWave = 0;
        spawnTimer = 1.5f;
        Battlefield.currentWave = waveNumber;
    }

    // ── Question Handling ──

    private void GenerateNextQuestion()
    {
        CurrentQuestion = questionProvider.GetSnakeSpellQuestion(difficulty);
        Answered = false;
        SelectedAnswerIndex = -1;
        ShowCorrect = false;
        StartQuestionTimer();
    }

    private void StartQuestionTimer()
    {
        if (questionTimerCoroutine != null) StopCoroutine(questionTimerCoroutine);
        QuestionTimeLeft = Config.questionTimerSeconds;
        questionTimerCoroutine = StartCoroutine(QuestionTimerCoroutine());
    }

    private IEnumerator QuestionTimerCoroutine()
    {
        while (QuestionTimeLeft > 0)
        {
            yield return new WaitForSeconds(1f);
            QuestionTimeLeft--;
        }

        if (!Answered)
            HandleQuestionTimeout();
    }

    private void HandleQuestionTimeout()
    {
        Answered = true;
        ShowCorrect = true;
        Battlefield.questionsAnswered++;
        DisplayFeedback("Time's up!", false);
        StartCoroutine(ScheduleNextQuestion(0.8f));
    }

    public void SubmitAnswer(int index)
    {
        if (Answered) return;
        if (CurrentQuestion == null) return;
        Question q = CurrentQuestion.question;

        Answered = true;
        if (questionTimerCoroutine != null) StopCoroutine(questionTimerCoroutine);
        SelectedAnswerIndex = index;
        ShowCorrect = true;

        bool isCorrect = q.answers[index] == q.correctAnswer;
        Battlefield.questionsAnswered++;
        if (isCorrect) Battlefield.questionsCorrect++;

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
        if (Battlefield.status == GameStatus.Playing || Battlefield.status == GameStatus.WaveTransition)
            GenerateNextQuestion();
    }

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
        // Brief pause on game over screen
        yield return new WaitForSeconds(1.5f);
        SceneTransitionManager.Instance.LoadSnakeResults();
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }
}
