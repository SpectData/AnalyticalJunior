using System.Collections.Generic;

[System.Serializable]
public class Question
{
    public string label;
    public string questionText;
    public List<string> sequence;
    public List<string> answers;
    public string correctAnswer;
    public string explanation;

    public Question(string label, List<string> answers, string correctAnswer,
        string questionText = null, List<string> sequence = null,
        string explanation = null)
    {
        this.label = label;
        this.questionText = questionText;
        this.sequence = sequence;
        this.answers = answers;
        this.correctAnswer = correctAnswer;
        this.explanation = explanation;
    }
}

public abstract class GameQuestion
{
    public class Standard : GameQuestion
    {
        public Question question;
        public Standard(Question question) { this.question = question; }
    }

    public class ReadingComprehension : GameQuestion
    {
        public string passageId;
        public string passageTitle;
        public string passageText;
        public string passageType;
        public Question question;

        public ReadingComprehension(string passageId, string passageTitle,
            string passageText, string passageType, Question question)
        {
            this.passageId = passageId;
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
