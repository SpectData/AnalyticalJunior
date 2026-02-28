using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MathQuestionGenerator
{
    public static GameQuestion.Standard GenerateQuickCalc(Difficulty difficulty)
    {
        int a, b;
        string op;

        switch (difficulty)
        {
            case Difficulty.Easy:
                a = Rand(1, 10);
                b = Rand(1, 10);
                op = PickRandom(new[] { "+", "-" });
                break;
            case Difficulty.Medium:
                op = PickRandom(new[] { "+", "-", "\u00D7" });
                if (op == "\u00D7")
                {
                    a = Rand(2, 12);
                    b = Rand(2, 12);
                }
                else
                {
                    a = Rand(5, 30);
                    b = Rand(5, 30);
                }
                break;
            case Difficulty.Hard:
                op = PickRandom(new[] { "+", "-", "\u00D7" });
                if (op == "\u00D7")
                {
                    a = Rand(5, 15);
                    b = Rand(5, 15);
                }
                else
                {
                    a = Rand(20, 100);
                    b = Rand(20, 100);
                }
                break;
            default: // SuperHard
                op = PickRandom(new[] { "+", "-", "\u00D7", "\u00F7" });
                if (op == "\u00D7")
                {
                    a = Rand(10, 25);
                    b = Rand(10, 25);
                }
                else if (op == "\u00F7")
                {
                    b = Rand(2, 12);
                    a = b * Rand(2, 15);
                }
                else
                {
                    a = Rand(50, 200);
                    b = Rand(50, 200);
                }
                break;
        }

        int answer;
        switch (op)
        {
            case "+":
                answer = a + b;
                break;
            case "-":
                if (b > a) { int tmp = a; a = b; b = tmp; }
                answer = a - b;
                break;
            case "\u00F7":
                answer = a / b;
                break;
            default: // multiply
                answer = a * b;
                break;
        }

        List<int> distractors = GenerateDistractors(answer, 3);
        List<string> answers = new List<int> { answer }
            .Concat(distractors)
            .OrderBy(_ => Random.value)
            .Select(x => x.ToString())
            .ToList();

        return new GameQuestion.Standard(
            new Question(
                label: "Quick Calc",
                questionText: $"{a} {op} {b} = ?",
                answers: answers,
                correctAnswer: answer.ToString()
            )
        );
    }

    public static GameQuestion.Standard GenerateSequence(Difficulty difficulty)
    {
        List<int> seq;

        switch (difficulty)
        {
            case Difficulty.Easy:
            {
                int start = Rand(1, 10);
                int step = Rand(1, 5);
                seq = Enumerable.Range(0, 5).Select(i => start + step * i).ToList();
                break;
            }
            case Difficulty.Medium:
            {
                int start = Rand(1, 5);
                int mult = Rand(2, 3);
                seq = Enumerable.Range(0, 5)
                    .Select(i => start * (int)Mathf.Pow(mult, i))
                    .ToList();
                break;
            }
            case Difficulty.Hard:
            {
                int a = Rand(1, 3);
                int b = Rand(1, 4);
                int c = Rand(1, 3);
                seq = Enumerable.Range(0, 5).Select(i => a * i * i + b * i + c).ToList();
                break;
            }
            default: // SuperHard
            {
                int a = Rand(2, 5);
                int b = Rand(2, 6);
                int c = Rand(1, 5);
                seq = Enumerable.Range(0, 5).Select(i => a * i * i + b * i + c).ToList();
                break;
            }
        }

        int answer = seq[4];
        List<string> display = seq.Take(4).Select(x => x.ToString()).ToList();
        display.Add("?");

        List<int> distractors = GenerateDistractors(answer, 3);
        List<string> answers = new List<int> { answer }
            .Concat(distractors)
            .OrderBy(_ => Random.value)
            .Select(x => x.ToString())
            .ToList();

        return new GameQuestion.Standard(
            new Question(
                label: "Number Sequence",
                answers: answers,
                correctAnswer: answer.ToString(),
                sequence: display
            )
        );
    }

    public static GameQuestion.Standard GenerateCompare(Difficulty difficulty)
    {
        string exprA, exprB;
        int valA, valB;

        switch (difficulty)
        {
            case Difficulty.Easy:
            {
                int a1 = Rand(1, 10), a2 = Rand(1, 10);
                int b1 = Rand(1, 10), b2 = Rand(1, 10);
                valA = a1 + a2;
                valB = b1 + b2;
                exprA = $"{a1} + {a2}";
                exprB = $"{b1} + {b2}";
                break;
            }
            case Difficulty.Medium:
            {
                int a1 = Rand(2, 8), a2 = Rand(2, 8);
                int b1 = Rand(2, 8), b2 = Rand(2, 8);
                valA = a1 * a2;
                valB = b1 * b2;
                exprA = $"{a1} \u00D7 {a2}";
                exprB = $"{b1} \u00D7 {b2}";
                break;
            }
            case Difficulty.Hard:
            {
                int a1 = Rand(5, 15), a2 = Rand(2, 8), a3 = Rand(1, 10);
                int b1 = Rand(5, 15), b2 = Rand(2, 8), b3 = Rand(1, 10);
                valA = a1 * a2 + a3;
                valB = b1 * b2 + b3;
                exprA = $"{a1} \u00D7 {a2} + {a3}";
                exprB = $"{b1} \u00D7 {b2} + {b3}";
                break;
            }
            default: // SuperHard
            {
                int a1 = Rand(10, 25), a2 = Rand(3, 12), a3 = Rand(10, 50);
                int b1 = Rand(10, 25), b2 = Rand(3, 12), b3 = Rand(10, 50);
                valA = a1 * a2 + a3;
                valB = b1 * b2 + b3;
                exprA = $"{a1} \u00D7 {a2} + {a3}";
                exprB = $"{b1} \u00D7 {b2} + {b3}";
                break;
            }
        }

        if (valA == valB) valB += 1;

        string correct = valA > valB ? exprA : exprB;
        List<string> answers = new List<string> { exprA, exprB }
            .OrderBy(_ => Random.value)
            .ToList();

        return new GameQuestion.Standard(
            new Question(
                label: "Compare",
                questionText: "Which is larger?",
                answers: answers,
                correctAnswer: correct
            )
        );
    }

    private static int Rand(int min, int max)
    {
        return Random.Range(min, max + 1);
    }

    private static string PickRandom(string[] options)
    {
        return options[Random.Range(0, options.Length)];
    }

    private static List<int> GenerateDistractors(int correct, int count)
    {
        HashSet<int> set = new HashSet<int>();
        int attempts = 0;
        while (set.Count < count && attempts < 100)
        {
            attempts++;
            float r = Random.value;
            int d;
            if (r < 0.3f) d = correct + Rand(1, 5);
            else if (r < 0.6f) d = correct - Rand(1, 5);
            else if (r < 0.8f) d = correct + Rand(5, 15);
            else d = correct - Rand(5, 15);

            if (d != correct && d >= 0) set.Add(d);
        }
        return set.ToList();
    }
}
