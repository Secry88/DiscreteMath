using DiplomProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiplomProject.Services;

public class PracticeGenerator
{
    private static readonly Random Rng = new();

    private static readonly List<(string Expr, Func<HashSet<int>, HashSet<int>, HashSet<int>, HashSet<int>, HashSet<int>> Compute, bool UsesC)> Templates = new()
    {
        // Простые (два множества)
        ("A ∪ B",           (a, b, c, u) => new HashSet<int>(a.Union(b)),                               false),
        ("A ∩ B",           (a, b, c, u) => new HashSet<int>(a.Intersect(b)),                           false),
        ("A \\ B",          (a, b, c, u) => new HashSet<int>(a.Except(b)),                              false),
        ("B \\ A",          (a, b, c, u) => new HashSet<int>(b.Except(a)),                              false),
        ("(A \\ B) ∪ (B \\ A)", (a, b, c, u) => new HashSet<int>(a.Except(b).Union(b.Except(a))),     false),

        // Средние (три множества)
        ("(A ∪ B) ∩ C",    (a, b, c, u) => new HashSet<int>(a.Union(b).Intersect(c)),                  true),
        ("(A ∩ B) ∪ C",    (a, b, c, u) => new HashSet<int>(a.Intersect(b).Union(c)),                  true),
        ("(A ∪ B) \\ C",   (a, b, c, u) => new HashSet<int>(a.Union(b).Except(c)),                     true),
        ("A \\ (B ∪ C)",   (a, b, c, u) => new HashSet<int>(a.Except(b.Union(c))),                     true),
        ("A ∪ (B \\ C)",   (a, b, c, u) => new HashSet<int>(a.Union(b.Except(c))),                     true),
        ("(A \\ B) ∪ C",   (a, b, c, u) => new HashSet<int>(a.Except(b).Union(c)),                     true),
        ("(A ∩ C) ∪ (B \\ C)", (a, b, c, u) => new HashSet<int>(a.Intersect(c).Union(b.Except(c))),   true),
        ("A \\ (B ∩ C)",   (a, b, c, u) => new HashSet<int>(a.Except(b.Intersect(c))),                 true),
        ("(A ∪ C) \\ B",   (a, b, c, u) => new HashSet<int>(a.Union(c).Except(b)),                     true),

        // Сложные (три множества, три операции)
        ("(A ∪ B) \\ (A ∩ C)",         (a, b, c, u) => new HashSet<int>(a.Union(b).Except(a.Intersect(c))),       true),
        ("(A \\ B) ∪ (B \\ C)",        (a, b, c, u) => new HashSet<int>(a.Except(b).Union(b.Except(c))),          true),
        ("(A ∩ B) ∪ (A ∩ C)",          (a, b, c, u) => new HashSet<int>(a.Intersect(b).Union(a.Intersect(c))),    true),
        ("(A ∪ B) ∩ (A ∪ C)",          (a, b, c, u) => new HashSet<int>(a.Union(b).Intersect(a.Union(c))),        true),
        ("A \\ (B ∪ C) ∪ (B ∩ C)",    (a, b, c, u) => new HashSet<int>(a.Except(b.Union(c)).Union(b.Intersect(c))), true),
        ("(A ∪ C) \\ (B ∪ C)",        (a, b, c, u) => new HashSet<int>(a.Union(c).Except(b.Union(c))),            true),
    };

    public GeneratedPracticeTask Generate()
    {
        var template = Templates[Rng.Next(Templates.Count)];

        // U = случайное подмножество {1..12}, 8–10 элементов
        var uSize = Rng.Next(8, 11);
        var universal = Enumerable.Range(1, 12).OrderBy(_ => Rng.Next()).Take(uSize).ToHashSet();
        var uList = universal.ToList();

        var a = RandomSubset(uList);
        var b = RandomSubset(uList);
        var c = RandomSubset(uList);

        var answer = template.Compute(a, b, c, universal);

        return new GeneratedPracticeTask
        {
            Expression    = template.Expr,
            SetADisplay   = FormatSet(a),
            SetBDisplay   = FormatSet(b),
            SetCDisplay   = template.UsesC ? FormatSet(c) : null,
            UniversalDisplay = FormatSet(universal),
            UsesC         = template.UsesC,
            CorrectAnswer = answer.Count == 0 ? "∅" : string.Join(", ", answer.OrderBy(x => x))
        };
    }

    private static HashSet<int> RandomSubset(List<int> universe)
    {
        var size = Rng.Next(1, Math.Max(2, universe.Count / 2 + 1));
        return universe.OrderBy(_ => Rng.Next()).Take(size).ToHashSet();
    }

    private static string FormatSet(HashSet<int> set) =>
        set.Count == 0 ? "∅" : "{ " + string.Join(", ", set.OrderBy(x => x)) + " }";
}
