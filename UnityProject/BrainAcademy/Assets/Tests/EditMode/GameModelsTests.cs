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

public class SnakeSpellConstantsTests
{
    [Test]
    public void FieldWidth_Is1000()
    {
        Assert.AreEqual(1000f, SnakeSpellConstants.FieldWidth);
    }

    [Test]
    public void NumLanes_Is3()
    {
        Assert.AreEqual(3, SnakeSpellConstants.NumLanes);
    }

    [Test]
    public void WizardX_IsPositive()
    {
        Assert.Greater(SnakeSpellConstants.WizardX, 0f);
    }
}
