namespace AdventOfCode.Days
{
    [Day(2021, 8)]
    public class Day08 : BaseDay
    {
        public override string PartOne(string input)
        {
            var notes = input.ParseStrings(ParseNote).ToArray();
            var rc = notes.Aggregate(0L,
                (current, note) => current + note.outputs.Count(s => s.Length is 2 or 3 or 4 or 7));
            return rc.ToString();
        }

        public override string PartTwo(string input)
        {
            var rc = input.ParseStrings(ParseNote).Select(Analyse).Sum();
            return rc.ToString();
        }

        private static long Analyse((List<string> patterns, List<string> outputs) note)
        {
            var patterns = note.patterns;
            var digits = new (int val, string s)[10];
            digits[1] = (1, patterns.First(s => s.Length == 2));
            digits[4] = (4, patterns.First(s => s.Length == 4));
            digits[7] = (7, patterns.First(s => s.Length == 3));
            digits[8] = (8, patterns.First(s => s.Length == 7));
            digits[3] = (3, patterns.Where(s1 => s1.Length == 5).First(s2 => s2.HasCharacters(digits[1].s)));
            digits[6] = (6, patterns.Where(s1 => s1.Length == 6).First(s2 => !s2.HasCharacters(digits[1].s)));
            digits[9] = (9, patterns.Where(s1 => s1.Length == 6).First(s2 => s2.HasCharacters(digits[3].s)));
            digits[0] = (0, patterns.Where(s1 => s1.Length == 6).First(s2 => s2 != digits[6].s && s2 != digits[9].s));
            digits[5] = (5, patterns.Where(s1 => s1.Length == 5).First(s2 => digits[6].s.HasCharacters(s2)));
            digits[2] = (2, patterns.Where(s1 => s1.Length == 5).First(s2 => s2 != digits[3].s && s2 != digits[5].s));

            var outputs = note.outputs;
            var rc = 0L;
            rc += digits.First(d => d.s == outputs[0]).val * 1000L;
            rc += digits.First(d => d.s == outputs[1]).val * 100L;
            rc += digits.First(d => d.s == outputs[2]).val * 10L;
            rc += digits.First(d => d.s == outputs[3]).val;
            return rc;
        }

        private static (List<string> patterns, List<string> outputs) ParseNote(string arg)
        {
            var parts = arg.Split('|');
            var patterns = parts[0].Trim().Split(' ').Select(p => string.Concat(p.OrderBy(c => c))).ToList();
            var outputs = parts[1].Trim().Split(' ').Select(p => string.Concat(p.OrderBy(c => c))).ToList();
            return (patterns, outputs);
        }
    }

    internal static class Day08Extensions
    {
        internal static bool HasCharacters(this string dst, string src) => src.All(c => dst.Any(c1 => c1 == c));
    }
}