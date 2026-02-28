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
        public string topic;
        public string difficulty;
        public string question;
        public List<string> options;
        public string correctAnswer;
    }

    private readonly List<BankQuestion> allQuestions = new List<BankQuestion>();
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

        JArray math = (JArray)root["mathematical_reasoning"];
        if (math != null)
        {
            foreach (JToken item in math)
                allQuestions.Add(ParseBankQuestion(item));
        }

        JArray thinking = (JArray)root["thinking_skills"];
        if (thinking != null)
        {
            foreach (JToken item in thinking)
                allQuestions.Add(ParseBankQuestion(item));
        }

        Debug.Log($"QuestionBankLoader: Loaded {allQuestions.Count} questions");
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
            .Where(q => q.difficulty == diffStr && !usedIds.Contains(q.id))
            .ToList();

        if (available.Count == 0)
        {
            // All questions for this difficulty used - recycle
            usedIds.RemoveWhere(id =>
                allQuestions.Any(q => q.id == id && q.difficulty == diffStr));
            available = allQuestions.Where(q => q.difficulty == diffStr).ToList();
        }

        if (available.Count == 0) return null;

        BankQuestion bq = available[Random.Range(0, available.Count)];
        return ConvertToGameQuestion(bq);
    }

    private GameQuestion.Standard ConvertToGameQuestion(BankQuestion bq)
    {
        usedIds.Add(bq.id);

        TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;
        string label = textInfo.ToTitleCase(bq.topic.Replace("_", " "));

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
