using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizController : MonoBehaviour
{
    public const int TotalQuestions = 5;

    // Current game state (read by QuizUIController)
    public GameQuestion CurrentQuestion { get; private set; }
    public int CurrentQuestionIndex { get; private set; }
    public int GameScore { get; private set; }
    public int Streak { get; private set; }
    public int CorrectCount { get; private set; }
    public int TimeLeft { get; private set; }
    public bool Answered { get; private set; }
    public int SelectedAnswerIndex { get; private set; }
    public bool ShowCorrect { get; private set; }

    // Memory grid state
    public bool MemoryShowPhase { get; private set; }
    public HashSet<int> MemorySelectedCells { get; private set; } = new HashSet<int>();
    public int MemoryCorrectPicks { get; private set; }
    public bool MemoryDone { get; private set; }

    // Feedback
    public string FeedbackText { get; private set; }
    public bool FeedbackIsCorrect { get; private set; }
    public bool ShowFeedback { get; private set; }

    public bool IsGameOver => CurrentQuestionIndex >= TotalQuestions && Answered;

    private QuestionProvider questionProvider;
    private Coroutine timerCoroutine;
    private bool gameFinalized;

    void Start()
    {
        questionProvider = new QuestionProvider();
        questionProvider.Initialize();
        StartGame();
    }

    public void StartGame()
    {
        Difficulty difficulty = GameManager.Instance.SelectedDifficulty;
        CurrentQuestionIndex = 0;
        GameScore = 0;
        Streak = 0;
        CorrectCount = 0;
        gameFinalized = false;
        NextQuestion();
    }

    public void NextQuestion()
    {
        if (CurrentQuestionIndex >= TotalQuestions) return;

        Answered = false;
        SelectedAnswerIndex = -1;
        ShowCorrect = false;
        MemoryShowPhase = true;
        MemorySelectedCells = new HashSet<int>();
        MemoryCorrectPicks = 0;
        MemoryDone = false;
        CurrentQuestionIndex++;

        Category category = GameManager.Instance.SelectedCategory;
        Difficulty difficulty = GameManager.Instance.SelectedDifficulty;
        CurrentQuestion = questionProvider.GetQuizQuestion(category, difficulty);

        StartTimer();

        // Handle memory show phase
        if (CurrentQuestion is GameQuestion.Memory memQ)
        {
            StartCoroutine(MemoryShowCoroutine(memQ.memoryQuestion.showTimeSeconds));
        }
    }

    private IEnumerator MemoryShowCoroutine(float showTime)
    {
        yield return new WaitForSeconds(showTime);
        MemoryShowPhase = false;
    }

    private void StartTimer()
    {
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        TimeLeft = GameManager.Instance.SelectedDifficulty.GetTimerSeconds();
        timerCoroutine = StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        while (TimeLeft > 0)
        {
            yield return new WaitForSeconds(1f);
            TimeLeft--;
        }

        if (!Answered)
        {
            HandleTimeout();
        }
    }

    private void HandleTimeout()
    {
        Answered = true;
        Streak = 0;
        ShowCorrect = true;
        DisplayFeedback("Time's up!", false);
        StartCoroutine(ScheduleNextQuestion());
    }

    public void SelectAnswer(int index)
    {
        if (Answered) return;
        if (!(CurrentQuestion is GameQuestion.Standard stdQ)) return;
        Question question = stdQ.question;

        Answered = true;
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        SelectedAnswerIndex = index;
        ShowCorrect = true;

        Difficulty difficulty = GameManager.Instance.SelectedDifficulty;
        bool isCorrect = question.answers[index] == question.correctAnswer;

        if (isCorrect)
        {
            Streak++;
            CorrectCount++;
            int bonus = Streak > 1 ? difficulty.GetStreakBonus() : 0;
            int pts = difficulty.GetPoints() + bonus;
            GameScore += pts;
            DisplayFeedback(bonus > 0 ? $"+{pts} Streak!" : $"+{pts}", true);
        }
        else
        {
            Streak = 0;
            DisplayFeedback("Wrong!", false);
        }

        StartCoroutine(ScheduleNextQuestion());
    }

    public void SelectMemoryCell(int index)
    {
        if (!(CurrentQuestion is GameQuestion.Memory memQ)) return;
        MemoryQuestion mem = memQ.memoryQuestion;
        if (Answered || MemoryShowPhase || MemorySelectedCells.Contains(index)) return;
        if (MemorySelectedCells.Count >= mem.highlighted.Count) return;

        MemorySelectedCells.Add(index);
        if (mem.highlighted.Contains(index))
        {
            MemoryCorrectPicks++;
        }

        if (MemorySelectedCells.Count >= mem.highlighted.Count)
        {
            MemoryDone = true;
            Answered = true;
            if (timerCoroutine != null) StopCoroutine(timerCoroutine);

            Difficulty difficulty = GameManager.Instance.SelectedDifficulty;

            if (MemoryCorrectPicks == mem.highlighted.Count)
            {
                Streak++;
                CorrectCount++;
                int bonus = Streak > 1 ? difficulty.GetStreakBonus() : 0;
                int pts = difficulty.GetPoints() + bonus;
                GameScore += pts;
                DisplayFeedback(bonus > 0 ? $"+{pts} Streak!" : $"+{pts}", true);
            }
            else
            {
                Streak = 0;
                DisplayFeedback($"{MemoryCorrectPicks}/{mem.highlighted.Count}", false);
            }

            StartCoroutine(ScheduleNextQuestion());
        }
    }

    private void DisplayFeedback(string text, bool correct)
    {
        FeedbackText = text;
        FeedbackIsCorrect = correct;
        ShowFeedback = true;
        StartCoroutine(HideFeedbackCoroutine());
    }

    private IEnumerator HideFeedbackCoroutine()
    {
        yield return new WaitForSeconds(0.8f);
        ShowFeedback = false;
    }

    private IEnumerator ScheduleNextQuestion()
    {
        yield return new WaitForSeconds(1.2f);

        if (CurrentQuestionIndex >= TotalQuestions)
        {
            FinalizeGame();
        }
        else
        {
            NextQuestion();
        }
    }

    private void FinalizeGame()
    {
        if (gameFinalized) return;
        gameFinalized = true;

        if (timerCoroutine != null) StopCoroutine(timerCoroutine);

        GameManager gm = GameManager.Instance;
        gm.LastQuizScore = GameScore;
        gm.LastQuizCorrectCount = CorrectCount;
        gm.LastQuizTotalQuestions = TotalQuestions;
        gm.LastQuizStreak = Streak;
        gm.AddQuizResult(GameScore, Streak);

        SceneTransitionManager.Instance.LoadResults();
    }

    public int Accuracy => TotalQuestions > 0 ? (CorrectCount * 100) / TotalQuestions : 0;

    public int StarCount
    {
        get
        {
            int acc = Accuracy;
            if (acc >= 100) return 3;
            if (acc >= 60) return 2;
            if (acc >= 20) return 1;
            return 0;
        }
    }
}
