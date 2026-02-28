using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LogicQuestionGenerator
{
    private static readonly string[] Shapes = { "\u25CF", "\u25A0", "\u25B2", "\u2666", "\u2605" };

    public static GameQuestion.Standard GeneratePatternMatch(Difficulty difficulty)
    {
        string answer;
        List<string> pattern;

        switch (difficulty)
        {
            case Difficulty.Easy:
            {
                string a = PickRandomShape();
                string b;
                do { b = PickRandomShape(); } while (b == a);
                pattern = new List<string> { a, b, a, b, "?" };
                answer = a;
                break;
            }
            case Difficulty.Medium:
            {
                List<string> picks = ShuffledShapes(3);
                pattern = new List<string> { picks[0], picks[1], picks[2], picks[0], picks[1], "?" };
                answer = picks[2];
                break;
            }
            case Difficulty.Hard:
            {
                string a = PickRandomShape();
                string b;
                do { b = PickRandomShape(); } while (b == a);
                pattern = new List<string> { a, a, b, b, a, a, b, "?" };
                answer = b;
                break;
            }
            default: // SuperHard
            {
                List<string> picks = ShuffledShapes(4);
                pattern = new List<string>
                {
                    picks[0], picks[1], picks[2], picks[3],
                    picks[0], picks[1], picks[2], "?"
                };
                answer = picks[3];
                break;
            }
        }

        List<string> wrongShapes = Shapes.Where(s => s != answer).ToList();
        List<string> distractors = wrongShapes.OrderBy(_ => Random.value).Take(3).ToList();
        List<string> answers = new List<string> { answer }
            .Concat(distractors)
            .OrderBy(_ => Random.value)
            .ToList();

        return new GameQuestion.Standard(
            new Question(
                label: "Pattern Match",
                answers: answers,
                correctAnswer: answer,
                sequence: pattern
            )
        );
    }

    public static GameQuestion.Standard GenerateOddOneOut(Difficulty difficulty)
    {
        OddOneOutConfig config = PickOddOneOutConfig(difficulty);
        List<int> picks = config.group.OrderBy(_ => Random.value).Take(3).ToList();
        int odd = config.outliers[Random.Range(0, config.outliers.Count)];
        List<int> items = picks.Concat(new[] { odd }).OrderBy(_ => Random.value).ToList();

        return new GameQuestion.Standard(
            new Question(
                label: "Odd One Out",
                questionText: config.question,
                answers: items.Select(x => x.ToString()).ToList(),
                correctAnswer: odd.ToString()
            )
        );
    }

    private struct OddOneOutConfig
    {
        public List<int> group;
        public List<int> outliers;
        public string question;
    }

    private static OddOneOutConfig PickOddOneOutConfig(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                if (Random.value > 0.5f)
                    return new OddOneOutConfig
                    {
                        group = new List<int> { 2, 4, 6, 8, 10, 12, 14 },
                        outliers = new List<int> { 1, 3, 5, 7, 9, 11, 13 },
                        question = "Which number does NOT belong?"
                    };
                else
                    return new OddOneOutConfig
                    {
                        group = new List<int> { 1, 3, 5, 7, 9, 11, 13 },
                        outliers = new List<int> { 2, 4, 6, 8, 10, 12, 14 },
                        question = "Which number does NOT belong?"
                    };

            case Difficulty.Medium:
                if (Random.value > 0.5f)
                    return new OddOneOutConfig
                    {
                        group = new List<int> { 2, 3, 5, 7, 11, 13, 17, 19, 23 },
                        outliers = new List<int> { 4, 6, 8, 9, 10, 12, 14, 15 },
                        question = "Which is NOT a prime number?"
                    };
                else
                    return new OddOneOutConfig
                    {
                        group = new List<int> { 3, 6, 9, 12, 15, 18, 21 },
                        outliers = new List<int> { 4, 5, 7, 8, 10, 11, 13 },
                        question = "Which is NOT a multiple of 3?"
                    };

            case Difficulty.Hard:
                if (Random.value > 0.5f)
                    return new OddOneOutConfig
                    {
                        group = new List<int> { 1, 4, 9, 16, 25, 36, 49, 64 },
                        outliers = new List<int> { 2, 3, 5, 6, 7, 8, 10, 11, 12, 13, 15 },
                        question = "Which is NOT a perfect square?"
                    };
                else
                    return new OddOneOutConfig
                    {
                        group = new List<int> { 1, 2, 3, 5, 8, 13, 21, 34 },
                        outliers = new List<int> { 4, 6, 7, 9, 10, 11, 12, 14 },
                        question = "Which is NOT a Fibonacci number?"
                    };

            default: // SuperHard
                int variant = Random.Range(0, 3);
                if (variant == 0)
                    return new OddOneOutConfig
                    {
                        group = new List<int> { 1, 8, 27, 64, 125 },
                        outliers = new List<int> { 2, 4, 9, 16, 25, 32, 36, 50 },
                        question = "Which is NOT a perfect cube?"
                    };
                else if (variant == 1)
                    return new OddOneOutConfig
                    {
                        group = new List<int> { 2, 4, 8, 16, 32, 64, 128 },
                        outliers = new List<int> { 3, 6, 10, 12, 24, 48, 96 },
                        question = "Which is NOT a power of 2?"
                    };
                else
                    return new OddOneOutConfig
                    {
                        group = new List<int> { 1, 3, 6, 10, 15, 21, 28, 36 },
                        outliers = new List<int> { 2, 4, 5, 7, 8, 9, 11, 12, 14 },
                        question = "Which is NOT a triangular number?"
                    };
        }
    }

    private static string PickRandomShape()
    {
        return Shapes[Random.Range(0, Shapes.Length)];
    }

    private static List<string> ShuffledShapes(int count)
    {
        return Shapes.OrderBy(_ => Random.value).Take(count).ToList();
    }
}
