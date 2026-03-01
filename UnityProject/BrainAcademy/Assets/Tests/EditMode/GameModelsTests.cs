using System.Collections.Generic;
using NUnit.Framework;

public class QuestionTests
{
    [Test]
    public void Constructor_SetsRequiredFields()
    {
        var answers = new List<string> { "A", "B", "C", "D" };
        var q = new Question(label: "Quick Calc", answers: answers, correctAnswer: "B");

        Assert.AreEqual("Quick Calc", q.label);
        Assert.AreEqual(4, q.answers.Count);
        Assert.AreEqual("B", q.correctAnswer);
        Assert.IsNull(q.questionText);
        Assert.IsNull(q.sequence);
    }

    [Test]
    public void Constructor_SetsOptionalFields()
    {
        var answers = new List<string> { "5", "6", "7", "8" };
        var seq = new List<string> { "1", "2", "3", "4", "?" };
        var q = new Question(
            label: "Sequence",
            answers: answers,
            correctAnswer: "5",
            questionText: "What comes next?",
            sequence: seq
        );

        Assert.AreEqual("What comes next?", q.questionText);
        Assert.AreEqual(5, q.sequence.Count);
    }

    [Test]
    public void Constructor_SetsExplanation()
    {
        var answers = new List<string> { "A", "B" };
        var q = new Question(
            label: "Test",
            answers: answers,
            correctAnswer: "A",
            explanation: "Because A is correct"
        );

        Assert.AreEqual("Because A is correct", q.explanation);
    }

    [Test]
    public void Constructor_ExplanationDefaultsToNull()
    {
        var answers = new List<string> { "A", "B" };
        var q = new Question(label: "Test", answers: answers, correctAnswer: "A");

        Assert.IsNull(q.explanation);
    }
}

public class GameQuestionTests
{
    [Test]
    public void Standard_WrapsQuestion()
    {
        var q = new Question("Test", new List<string> { "A" }, "A");
        var gq = new GameQuestion.Standard(q);
        Assert.AreEqual(q, gq.question);
    }

    [Test]
    public void ReadingComprehension_SetsAllFields()
    {
        var q = new Question("Reading", new List<string> { "A", "B", "C", "D" }, "B",
            questionText: "What is the main idea?");
        var rc = new GameQuestion.ReadingComprehension(
            passageId: "passage_01",
            passageTitle: "The Story of Frogs",
            passageText: "Frogs are amphibians...",
            passageType: "comprehension",
            question: q
        );

        Assert.AreEqual("passage_01", rc.passageId);
        Assert.AreEqual("The Story of Frogs", rc.passageTitle);
        Assert.AreEqual("Frogs are amphibians...", rc.passageText);
        Assert.AreEqual("comprehension", rc.passageType);
        Assert.AreEqual(q, rc.question);
    }

    [Test]
    public void ReadingComprehension_PassageIdCanBeEmpty()
    {
        var q = new Question("Reading", new List<string> { "A" }, "A");
        var rc = new GameQuestion.ReadingComprehension("", "Title", "Text", "poem", q);

        Assert.AreEqual("", rc.passageId);
    }
}

public class ReviewItemTests
{
    [Test]
    public void Constructor_SetsAllFields()
    {
        var item = new ReviewItem(
            questionText: "What is 2+2?",
            studentAnswer: "5",
            correctAnswer: "4",
            explanation: "2+2 equals 4"
        );

        Assert.AreEqual("What is 2+2?", item.questionText);
        Assert.AreEqual("5", item.studentAnswer);
        Assert.AreEqual("4", item.correctAnswer);
        Assert.AreEqual("2+2 equals 4", item.explanation);
    }
}

public class GamePhaseTests
{
    [Test]
    public void GamePhase_HasFourValues()
    {
        var values = System.Enum.GetValues(typeof(GamePhase));
        Assert.AreEqual(4, values.Length);
    }

    [Test]
    public void GamePhase_ContainsExpectedValues()
    {
        Assert.IsTrue(System.Enum.IsDefined(typeof(GamePhase), GamePhase.WavePhase));
        Assert.IsTrue(System.Enum.IsDefined(typeof(GamePhase), GamePhase.WaveReview));
        Assert.IsTrue(System.Enum.IsDefined(typeof(GamePhase), GamePhase.InterWavePhase));
        Assert.IsTrue(System.Enum.IsDefined(typeof(GamePhase), GamePhase.InterWaveReview));
    }
}

public class SnakeSpellConstantsTests
{
    [Test]
    public void FieldRadius_Is500()
    {
        Assert.AreEqual(500f, SnakeSpellConstants.FieldRadius);
    }

    [Test]
    public void WizardHitRadius_IsPositive()
    {
        Assert.Greater(SnakeSpellConstants.WizardHitRadius, 0f);
    }

    [Test]
    public void SpellDespawnRadius_IsGreaterThanFieldRadius()
    {
        Assert.Greater(SnakeSpellConstants.SpellDespawnRadius, SnakeSpellConstants.FieldRadius);
    }

    [Test]
    public void LightningBoltZapCount_Is3()
    {
        Assert.AreEqual(3, SnakeSpellConstants.LightningBoltZapCount);
    }
}
