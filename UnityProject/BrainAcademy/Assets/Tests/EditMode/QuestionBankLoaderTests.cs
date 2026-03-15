using System.Collections.Generic;
using NUnit.Framework;

public class QuestionBankLoaderTests
{
    private const string MinimalV2Json = @"{
        ""meta"": { ""version"": ""2.0"" },
        ""mathematical_reasoning"": [],
        ""thinking_skills"": [],
        ""reading"": {
            ""passages"": [
                {
                    ""id"": ""COMP_1"",
                    ""title"": ""Test Passage"",
                    ""type"": ""comprehension"",
                    ""text"": ""A simple passage.""
                }
            ],
            ""questions"": [
                {
                    ""id"": ""MCQ_1"",
                    ""passage_id"": ""COMP_1"",
                    ""question"": ""What is the passage about?"",
                    ""options"": [""A"", ""B"", ""C"", ""D""],
                    ""correct_answer"": ""A"",
                    ""explanation"": ""Correct.""
                }
            ]
        }
    }";

    private const string ClozeJson = @"{
        ""meta"": { ""version"": ""2.0"" },
        ""mathematical_reasoning"": [],
        ""thinking_skills"": [],
        ""reading"": {
            ""passages"": [
                {
                    ""id"": ""CLOZE_1"",
                    ""title"": ""Cloze Test"",
                    ""type"": ""cloze"",
                    ""text"": ""He could barely ...[blank 1]... himself. She ...[blank 2]... away."",
                    ""blanks"": [
                        {
                            ""id"": ""C1_B1"",
                            ""blank_number"": 1,
                            ""options"": [""resist"", ""contain"", ""keep"", ""hold""],
                            ""correct_index"": 1,
                            ""correct_answer"": ""contain"",
                            ""reasoning"": ""Contain oneself is the correct idiom.""
                        },
                        {
                            ""id"": ""C1_B2"",
                            ""blank_number"": 2,
                            ""options"": [""ran"", ""walked"", ""drove"", ""flew""],
                            ""correct_index"": 1,
                            ""correct_answer"": ""walked"",
                            ""reasoning"": ""Walked fits the context.""
                        }
                    ]
                }
            ],
            ""questions"": []
        }
    }";

    private const string SentenceInsertionNumberJson = @"{
        ""meta"": { ""version"": ""2.0"" },
        ""mathematical_reasoning"": [],
        ""thinking_skills"": [],
        ""reading"": {
            ""passages"": [
                {
                    ""id"": ""SI_1"",
                    ""title"": ""Sentence Insertion Test"",
                    ""type"": ""sentence_insertion"",
                    ""text"": ""Australia is a land of birds. A ............ B ............"",
                    ""gaps"": [""A"", ""B""],
                    ""sentences"": [
                        { ""number"": 1, ""text"": ""The birds are shy."" },
                        { ""number"": 2, ""text"": ""They live in trees."" },
                        { ""number"": 3, ""text"": ""This is extra."" }
                    ],
                    ""correct_mapping"": { ""A"": 1, ""B"": 2 },
                    ""extra_sentence"": 3,
                    ""gap_reasoning"": {
                        ""A"": ""Gap A needs shy birds."",
                        ""B"": ""Gap B needs trees.""
                    }
                }
            ],
            ""questions"": []
        }
    }";

    private const string SentenceInsertionLabelJson = @"{
        ""meta"": { ""version"": ""2.0"" },
        ""mathematical_reasoning"": [],
        ""thinking_skills"": [],
        ""reading"": {
            ""passages"": [
                {
                    ""id"": ""SI_2"",
                    ""title"": ""Label Format Test"",
                    ""type"": ""sentence_insertion"",
                    ""text"": ""Text with gaps. 12 ............ 13 ............"",
                    ""gaps"": [""12"", ""13""],
                    ""sentences"": [
                        { ""label"": ""A"", ""text"": ""First option."" },
                        { ""label"": ""B"", ""text"": ""Second option."" },
                        { ""label"": ""C"", ""text"": ""Third option."" }
                    ],
                    ""correct_mapping"": { ""12"": ""A"", ""13"": ""C"" },
                    ""extra_sentence"": ""B"",
                    ""gap_reasoning"": {
                        ""12"": ""Reason for gap 12."",
                        ""13"": ""Reason for gap 13.""
                    }
                }
            ],
            ""questions"": []
        }
    }";

    [Test]
    public void LoadFromJson_McqOnly_LoadsCorrectly()
    {
        var loader = new QuestionBankLoader();
        loader.LoadFromJson(MinimalV2Json);

        Assert.AreEqual(1, loader.GetQuestionCount("long"));
        Assert.AreEqual(1, loader.GetPassageCount());
    }

    [Test]
    public void LoadFromJson_Cloze_GeneratesQuestionPerBlank()
    {
        var loader = new QuestionBankLoader();
        loader.LoadFromJson(ClozeJson);

        Assert.AreEqual(2, loader.GetQuestionCount("long"));
    }

    [Test]
    public void LoadFromJson_Cloze_CorrectAnswerMatches()
    {
        var loader = new QuestionBankLoader();
        loader.LoadFromJson(ClozeJson);

        // Get both cloze questions
        var q1 = loader.GetReadingQuestion();
        Assert.IsNotNull(q1);
        Assert.AreEqual("cloze", q1.passageType);

        // One of the two blanks should be returned
        bool isBlank1 = q1.question.correctAnswer == "contain";
        bool isBlank2 = q1.question.correctAnswer == "walked";
        Assert.IsTrue(isBlank1 || isBlank2, "Correct answer should match one of the blanks");

        // Get the second
        var q2 = loader.GetReadingQuestionForPassage("CLOZE_1");
        Assert.IsNotNull(q2);
        Assert.AreEqual("cloze", q2.passageType);
    }

    [Test]
    public void LoadFromJson_Cloze_QuestionTextIncludesBlankNumber()
    {
        var loader = new QuestionBankLoader();
        loader.LoadFromJson(ClozeJson);

        var q = loader.GetReadingQuestion();
        Assert.IsTrue(q.question.questionText.Contains("Fill in blank"),
            "Cloze question text should reference blank number");
    }

    [Test]
    public void LoadFromJson_Cloze_HasFourOptions()
    {
        var loader = new QuestionBankLoader();
        loader.LoadFromJson(ClozeJson);

        var q = loader.GetReadingQuestion();
        Assert.AreEqual(4, q.question.answers.Count);
    }

    [Test]
    public void LoadFromJson_SentenceInsertion_NumberFormat_GeneratesQuestionPerGap()
    {
        var loader = new QuestionBankLoader();
        loader.LoadFromJson(SentenceInsertionNumberJson);

        Assert.AreEqual(2, loader.GetQuestionCount("long"));
    }

    [Test]
    public void LoadFromJson_SentenceInsertion_NumberFormat_CorrectAnswerMatches()
    {
        var loader = new QuestionBankLoader();
        loader.LoadFromJson(SentenceInsertionNumberJson);

        var q = loader.GetReadingQuestion();
        Assert.IsNotNull(q);
        Assert.AreEqual("sentence_insertion", q.passageType);

        // Correct answer should be a formatted sentence option
        bool matchesSentence1 = q.question.correctAnswer.Contains("The birds are shy.");
        bool matchesSentence2 = q.question.correctAnswer.Contains("They live in trees.");
        Assert.IsTrue(matchesSentence1 || matchesSentence2,
            $"Correct answer '{q.question.correctAnswer}' should match a sentence");
    }

    [Test]
    public void LoadFromJson_SentenceInsertion_HasAllSentencesAsOptions()
    {
        var loader = new QuestionBankLoader();
        loader.LoadFromJson(SentenceInsertionNumberJson);

        var q = loader.GetReadingQuestion();
        Assert.AreEqual(3, q.question.answers.Count, "Should have all sentences as options");
    }

    [Test]
    public void LoadFromJson_SentenceInsertion_LabelFormat_GeneratesQuestions()
    {
        var loader = new QuestionBankLoader();
        loader.LoadFromJson(SentenceInsertionLabelJson);

        Assert.AreEqual(2, loader.GetQuestionCount("long"));
    }

    [Test]
    public void LoadFromJson_SentenceInsertion_LabelFormat_CorrectAnswerMatches()
    {
        var loader = new QuestionBankLoader();
        loader.LoadFromJson(SentenceInsertionLabelJson);

        var q = loader.GetReadingQuestion();
        Assert.IsNotNull(q);
        Assert.AreEqual("sentence_insertion", q.passageType);

        // One of the gap answers should match
        bool matchesA = q.question.correctAnswer.Contains("First option.");
        bool matchesC = q.question.correctAnswer.Contains("Third option.");
        Assert.IsTrue(matchesA || matchesC,
            $"Correct answer '{q.question.correctAnswer}' should match gap mapping");
    }

    [Test]
    public void LoadFromJson_SentenceInsertion_QuestionTextReferencesGap()
    {
        var loader = new QuestionBankLoader();
        loader.LoadFromJson(SentenceInsertionNumberJson);

        var q = loader.GetReadingQuestion();
        Assert.IsTrue(q.question.questionText.Contains("gap"),
            "Question should reference the gap");
    }

    [Test]
    public void LoadFromJson_SentenceInsertion_HasExplanation()
    {
        var loader = new QuestionBankLoader();
        loader.LoadFromJson(SentenceInsertionNumberJson);

        var q = loader.GetReadingQuestion();
        Assert.IsFalse(string.IsNullOrEmpty(q.question.explanation),
            "Sentence insertion questions should have reasoning as explanation");
    }

    [Test]
    public void LoadFromJson_Mixed_CountsAllQuestionTypes()
    {
        // Build JSON with both MCQ and cloze
        string mixedJson = @"{
            ""meta"": { ""version"": ""2.0"" },
            ""mathematical_reasoning"": [],
            ""thinking_skills"": [],
            ""reading"": {
                ""passages"": [
                    {
                        ""id"": ""COMP_1"",
                        ""title"": ""Comprehension"",
                        ""type"": ""comprehension"",
                        ""text"": ""A passage.""
                    },
                    {
                        ""id"": ""CLOZE_1"",
                        ""title"": ""Cloze"",
                        ""type"": ""cloze"",
                        ""text"": ""Fill ...[blank 1]..."",
                        ""blanks"": [
                            {
                                ""id"": ""B1"",
                                ""blank_number"": 1,
                                ""options"": [""a"", ""b"", ""c"", ""d""],
                                ""correct_answer"": ""b"",
                                ""reasoning"": ""Because.""
                            }
                        ]
                    }
                ],
                ""questions"": [
                    {
                        ""id"": ""Q1"",
                        ""passage_id"": ""COMP_1"",
                        ""question"": ""What?"",
                        ""options"": [""A"", ""B""],
                        ""correct_answer"": ""A"",
                        ""explanation"": ""Yes.""
                    }
                ]
            }
        }";

        var loader = new QuestionBankLoader();
        loader.LoadFromJson(mixedJson);

        // 1 MCQ + 1 cloze blank = 2 long questions
        Assert.AreEqual(2, loader.GetQuestionCount("long"));
        Assert.AreEqual(2, loader.GetPassageCount());
    }

    [Test]
    public void LoadFromJson_Cloze_PassageTextContainsBlanks()
    {
        var loader = new QuestionBankLoader();
        loader.LoadFromJson(ClozeJson);

        var q = loader.GetReadingQuestion();
        Assert.IsTrue(q.passageText.Contains("[blank"),
            "Cloze passage text should contain blank markers");
    }
}
