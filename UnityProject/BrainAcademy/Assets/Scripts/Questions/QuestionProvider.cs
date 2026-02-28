using UnityEngine;

public class QuestionProvider
{
    private QuestionBankLoader bankLoader;

    public void Initialize()
    {
        bankLoader = new QuestionBankLoader();
        bankLoader.Load();
    }

    public GameQuestion.Standard GetSnakeSpellQuestion(Difficulty difficulty)
    {
        // Primary: question bank. Fallback: procedural generation.
        GameQuestion.Standard bankQuestion = bankLoader?.GetQuestion(difficulty);
        if (bankQuestion != null) return bankQuestion;
        return GenerateProceduralQuestion(difficulty);
    }

    private GameQuestion.Standard GenerateProceduralQuestion(Difficulty difficulty)
    {
        int choice = Random.Range(0, 5);
        switch (choice)
        {
            case 0: return MathQuestionGenerator.GenerateQuickCalc(difficulty);
            case 1: return MathQuestionGenerator.GenerateSequence(difficulty);
            case 2: return MathQuestionGenerator.GenerateCompare(difficulty);
            case 3: return LogicQuestionGenerator.GeneratePatternMatch(difficulty);
            default: return LogicQuestionGenerator.GenerateOddOneOut(difficulty);
        }
    }

    public void ResetBank()
    {
        bankLoader?.Reset();
    }
}
