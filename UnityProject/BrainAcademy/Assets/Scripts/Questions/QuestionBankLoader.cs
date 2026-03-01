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
        public string passageId; // null for short questions
    }

    private class PassageData
    {
        public string id;
        public string title;
        public string type;
        public string text;
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

        JObject root = JObject.Parse(json.text);
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
                if (pd.id != null)
                    passages[pd.id] = pd;
            }
        }

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
                correctAnswer: bq.correctAnswer
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
                correctAnswer: bq.correctAnswer
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
                correctAnswer: bq.correctAnswer
            )
        );
    }

    public void Reset()
    {
        usedIds.Clear();
    }
}
