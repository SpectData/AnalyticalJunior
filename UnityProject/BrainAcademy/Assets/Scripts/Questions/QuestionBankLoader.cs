using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class QuestionBankLoader
{
    private class BankQuestion
    {
        public string id;
        public string questionType; // "short" or "long"
        public string topic;
        public string difficulty;
        public string question;
        public List<string> options;
        public string correctAnswer;
        public string explanation;
        public string passageId; // null for short questions
    }

    private class PassageData
    {
        public string id;
        public string title;
        public string type;
        public string text;
        public JArray blanks;              // cloze
        public JArray gaps;                // sentence_insertion: gap labels
        public JArray sentences;           // sentence_insertion: sentence options
        public JObject correctMapping;     // sentence_insertion: gap→sentence
        public JToken extraSentence;       // sentence_insertion: extra (unused) sentence
        public JObject gapReasoning;       // sentence_insertion: per-gap explanations
    }

    private readonly List<BankQuestion> allQuestions = new List<BankQuestion>();
    private readonly Dictionary<string, PassageData> passages = new Dictionary<string, PassageData>();
    private readonly HashSet<string> usedIds = new HashSet<string>();

    public void Load()
    {
        TextAsset json = Resources.Load<TextAsset>("question_bank");
        if (json == null)
        {
            Debug.LogWarning("question_bank.json not found in Resources folder");
            return;
        }

        LoadFromJson(json.text);
    }

    public void LoadFromJson(string jsonText)
    {
        JObject root = JObject.Parse(jsonText);
        string version = root["meta"]?["version"]?.ToString() ?? "1.0";

        if (version.StartsWith("2"))
            LoadV2(root);
        else
            LoadV1(root);

        Debug.Log($"QuestionBankLoader: Loaded {allQuestions.Count} questions, {passages.Count} passages");
    }

    private void LoadV1(JObject root)
    {
        LoadSection(root, "mathematical_reasoning", "short");
        LoadSection(root, "thinking_skills", "short");
    }

    private void LoadV2(JObject root)
    {
        LoadSection(root, "mathematical_reasoning", "short");
        LoadSection(root, "thinking_skills", "short");
        LoadReadingSection(root);
    }

    private void LoadSection(JObject root, string section, string questionType)
    {
        JArray arr = (JArray)root[section];
        if (arr == null) return;

        foreach (JToken item in arr)
        {
            BankQuestion bq = ParseBankQuestion(item);
            bq.questionType = questionType;
            allQuestions.Add(bq);
        }
    }

    private void LoadReadingSection(JObject root)
    {
        JObject reading = (JObject)root["reading"];
        if (reading == null) return;

        JArray passageArr = (JArray)reading["passages"];
        if (passageArr != null)
        {
            foreach (JToken p in passageArr)
            {
                var pd = new PassageData
                {
                    id = p["id"]?.ToString(),
                    title = p["title"]?.ToString(),
                    type = p["type"]?.ToString(),
                    text = p["text"]?.ToString()
                };

                if (pd.type == "cloze")
                {
                    pd.blanks = (JArray)p["blanks"];
                }
                else if (pd.type == "sentence_insertion")
                {
                    pd.gaps = (JArray)p["gaps"];
                    pd.sentences = (JArray)p["sentences"];
                    pd.correctMapping = (JObject)p["correct_mapping"];
                    pd.extraSentence = p["extra_sentence"];
                    pd.gapReasoning = (JObject)p["gap_reasoning"];
                }

                if (pd.id != null)
                    passages[pd.id] = pd;
            }
        }

        // Load MCQ questions (from comprehension/poem passages)
        JArray questionArr = (JArray)reading["questions"];
        if (questionArr != null)
        {
            foreach (JToken item in questionArr)
            {
                BankQuestion bq = ParseBankQuestion(item);
                bq.questionType = "long";
                bq.passageId = item["passage_id"]?.ToString();
                allQuestions.Add(bq);
            }
        }

        // Generate questions from cloze and sentence insertion passages
        GenerateClozeQuestions();
        GenerateSentenceInsertionQuestions();
    }

    private void GenerateClozeQuestions()
    {
        foreach (var pd in passages.Values)
        {
            if (pd.type != "cloze" || pd.blanks == null) continue;

            foreach (JToken blank in pd.blanks)
            {
                string blankId = blank["id"]?.ToString() ?? "";
                int blankNum = blank["blank_number"]?.Value<int>() ?? 0;

                var options = new List<string>();
                JArray optArr = (JArray)blank["options"];
                if (optArr != null)
                {
                    foreach (JToken opt in optArr)
                        options.Add(opt.ToString());
                }

                allQuestions.Add(new BankQuestion
                {
                    id = blankId,
                    questionType = "long",
                    passageId = pd.id,
                    question = $"Fill in blank {blankNum}: Which word best completes the sentence?",
                    options = options,
                    correctAnswer = blank["correct_answer"]?.ToString() ?? "",
                    explanation = blank["reasoning"]?.ToString() ?? "",
                });
            }
        }
    }

    private void GenerateSentenceInsertionQuestions()
    {
        foreach (var pd in passages.Values)
        {
            if (pd.type != "sentence_insertion" || pd.gaps == null
                || pd.sentences == null || pd.correctMapping == null)
                continue;

            // Build sentence lookup: key (number or label) → display text
            var sentenceTexts = new List<string>();
            var sentenceKeys = new List<string>();
            foreach (JToken s in pd.sentences)
            {
                string key = s["number"]?.ToString() ?? s["label"]?.ToString() ?? "";
                string text = s["text"]?.ToString() ?? "";
                sentenceKeys.Add(key);
                sentenceTexts.Add(text);
            }

            // Build options list with labels
            var options = new List<string>();
            for (int i = 0; i < sentenceKeys.Count; i++)
                options.Add($"{sentenceKeys[i]}. {sentenceTexts[i]}");

            foreach (JToken gapToken in pd.gaps)
            {
                string gap = gapToken.ToString();
                string questionId = $"{pd.id}_GAP_{gap}";

                // Find correct sentence for this gap
                JToken correctVal = pd.correctMapping[gap];
                string correctKey = correctVal?.ToString() ?? "";

                // Find the matching option text
                string correctOption = "";
                for (int i = 0; i < sentenceKeys.Count; i++)
                {
                    if (sentenceKeys[i] == correctKey)
                    {
                        correctOption = options[i];
                        break;
                    }
                }

                string reasoning = pd.gapReasoning?[gap]?.ToString() ?? "";

                allQuestions.Add(new BankQuestion
                {
                    id = questionId,
                    questionType = "long",
                    passageId = pd.id,
                    question = $"Which sentence best fits gap {gap}?",
                    options = new List<string>(options),
                    correctAnswer = correctOption,
                    explanation = reasoning,
                });
            }
        }
    }

    private BankQuestion ParseBankQuestion(JToken obj)
    {
        var options = new List<string>();
        JArray arr = (JArray)obj["options"];
        if (arr != null)
        {
            foreach (JToken opt in arr)
                options.Add(opt.ToString());
        }

        return new BankQuestion
        {
            id = obj["id"]?.ToString() ?? "",
            topic = obj["topic"]?.ToString() ?? "",
            difficulty = obj["difficulty"]?.ToString() ?? "",
            question = obj["question"]?.ToString() ?? "",
            options = options,
            correctAnswer = obj["correct_answer"]?.ToString() ?? "",
            explanation = obj["explanation"]?.ToString() ?? "",
        };
    }

    public GameQuestion.Standard GetQuestion(Difficulty difficulty)
    {
        string diffStr = difficulty.ToBankKey();

        var available = allQuestions
            .Where(q => q.questionType == "short"
                     && q.difficulty == diffStr
                     && !usedIds.Contains(q.id))
            .ToList();

        if (available.Count == 0)
        {
            // All questions for this difficulty used - recycle
            usedIds.RemoveWhere(id =>
                allQuestions.Any(q => q.id == id
                                  && q.questionType == "short"
                                  && q.difficulty == diffStr));
            available = allQuestions
                .Where(q => q.questionType == "short" && q.difficulty == diffStr)
                .ToList();
        }

        if (available.Count == 0) return null;

        BankQuestion bq = available[Random.Range(0, available.Count)];
        return ConvertToStandardQuestion(bq);
    }

    public GameQuestion.Standard GetShortQuestion()
    {
        var available = allQuestions
            .Where(q => q.questionType == "short" && !usedIds.Contains(q.id))
            .ToList();

        if (available.Count == 0)
        {
            usedIds.RemoveWhere(id =>
                allQuestions.Any(q => q.id == id && q.questionType == "short"));
            available = allQuestions
                .Where(q => q.questionType == "short")
                .ToList();
        }

        if (available.Count == 0) return null;

        BankQuestion bq = available[Random.Range(0, available.Count)];
        return ConvertToStandardQuestion(bq);
    }

    public GameQuestion.ReadingComprehension GetReadingQuestion()
    {
        var available = allQuestions
            .Where(q => q.questionType == "long" && !usedIds.Contains(q.id))
            .ToList();

        if (available.Count == 0)
        {
            usedIds.RemoveWhere(id =>
                allQuestions.Any(q => q.id == id && q.questionType == "long"));
            available = allQuestions
                .Where(q => q.questionType == "long")
                .ToList();
        }

        if (available.Count == 0) return null;

        BankQuestion bq = available[Random.Range(0, available.Count)];
        usedIds.Add(bq.id);

        PassageData passage = null;
        if (bq.passageId != null)
            passages.TryGetValue(bq.passageId, out passage);

        string label = passage?.title ?? "Reading";

        return new GameQuestion.ReadingComprehension(
            passageId: bq.passageId ?? "",
            passageTitle: passage?.title ?? "",
            passageText: passage?.text ?? "",
            passageType: passage?.type ?? "comprehension",
            question: new Question(
                label: label,
                questionText: bq.question,
                answers: bq.options,
                correctAnswer: bq.correctAnswer,
                explanation: bq.explanation
            )
        );
    }

    public GameQuestion.ReadingComprehension GetReadingQuestionForPassage(string passageId)
    {
        if (string.IsNullOrEmpty(passageId))
            return GetReadingQuestion();

        var available = allQuestions
            .Where(q => q.questionType == "long"
                     && q.passageId == passageId
                     && !usedIds.Contains(q.id))
            .ToList();

        if (available.Count == 0)
            return null; // Passage exhausted — caller should switch passages

        BankQuestion bq = available[Random.Range(0, available.Count)];
        usedIds.Add(bq.id);

        PassageData passage = null;
        if (bq.passageId != null)
            passages.TryGetValue(bq.passageId, out passage);

        string label = passage?.title ?? "Reading";

        return new GameQuestion.ReadingComprehension(
            passageId: bq.passageId ?? "",
            passageTitle: passage?.title ?? "",
            passageText: passage?.text ?? "",
            passageType: passage?.type ?? "comprehension",
            question: new Question(
                label: label,
                questionText: bq.question,
                answers: bq.options,
                correctAnswer: bq.correctAnswer,
                explanation: bq.explanation
            )
        );
    }

    private GameQuestion.Standard ConvertToStandardQuestion(BankQuestion bq)
    {
        usedIds.Add(bq.id);

        TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;
        string label = string.IsNullOrEmpty(bq.topic)
            ? "General"
            : textInfo.ToTitleCase(bq.topic.Replace("_", " "));

        return new GameQuestion.Standard(
            new Question(
                label: label,
                questionText: bq.question,
                answers: bq.options,
                correctAnswer: bq.correctAnswer,
                explanation: bq.explanation
            )
        );
    }

    public int GetQuestionCount(string questionType = null)
    {
        if (questionType == null)
            return allQuestions.Count;
        return allQuestions.Count(q => q.questionType == questionType);
    }

    public int GetPassageCount()
    {
        return passages.Count;
    }

    public void Reset()
    {
        usedIds.Clear();
    }
}
