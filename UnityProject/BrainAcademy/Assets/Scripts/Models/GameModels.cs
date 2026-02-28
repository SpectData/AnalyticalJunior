using System.Collections.Generic;

[System.Serializable]
public class Question
{
    public string label;
    public string questionText;
    public List<string> sequence;
    public List<string> answers;
    public string correctAnswer;

    public Question(string label, List<string> answers, string correctAnswer,
        string questionText = null, List<string> sequence = null)
    {
        this.label = label;
        this.questionText = questionText;
        this.sequence = sequence;
        this.answers = answers;
        this.correctAnswer = correctAnswer;
    }
}

[System.Serializable]
public class MemoryQuestion
{
    public int gridSize;
    public List<int> highlighted;
    public float showTimeSeconds;

    public MemoryQuestion(int gridSize, List<int> highlighted, float showTimeSeconds)
    {
        this.gridSize = gridSize;
        this.highlighted = highlighted;
        this.showTimeSeconds = showTimeSeconds;
    }
}

public abstract class GameQuestion
{
    public class Standard : GameQuestion
    {
        public Question question;
        public Standard(Question question) { this.question = question; }
    }

    public class Memory : GameQuestion
    {
        public MemoryQuestion memoryQuestion;
        public Memory(MemoryQuestion memoryQuestion) { this.memoryQuestion = memoryQuestion; }
    }

    public class ReadingComprehension : GameQuestion
    {
        public string passageTitle;
        public string passageText;
        public string passageType;
        public Question question;

        public ReadingComprehension(string passageTitle, string passageText,
            string passageType, Question question)
        {
            this.passageTitle = passageTitle;
            this.passageText = passageText;
            this.passageType = passageType;
            this.question = question;
        }
    }
}

[System.Serializable]
public class GameStats
{
    public int totalScore;
    public int gamesPlayed;
    public int bestStreak;
}
