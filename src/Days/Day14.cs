namespace AdventOfCode.Days
{
    [Day(2021, 14)]
    public class Day14 : BaseDay
    {
        public override string PartOne(string input)
        {
            var (polymerTemplate, insertionRules) = Parse(input);
            return SolvePart1(polymerTemplate, insertionRules).ToString();
        }

        public override string PartTwo(string input)
        {
            var (polymerTemplate, insertionRules) = Parse(input);
            return SolvePart2(polymerTemplate, insertionRules).ToString();
        }

        private static int SolvePart1(char[] polymerTemplate, IReadOnlyDictionary<string, char> insertionRules)
        {
            var polymer = polymerTemplate;
            for (var i = 0; i < 10; i++) polymer = Polymerize(polymer, insertionRules);
            var elementOccurrence = polymer.Distinct()
                .Select(e => (element: e, count: polymer.Count(c => c == e))).ToArray();
            return elementOccurrence.MaxBy(p => p.count).count - elementOccurrence.MinBy(p => p.count).count;
        }

        private static (char[] polymerTemplate, Dictionary<string, char> insertionRules) Parse(string input)
        {
            var template = input.Lines().First().ToArray();
            var rules = input.Lines().Skip(1).Select(s => s.Split(' '))
                .ToDictionary(x => x[0], x => x[2][0]);
            return (template, rules);
        }

        private static char[] Polymerize(IReadOnlyList<char> template, IReadOnlyDictionary<string, char> rules)
        {
            var rc = new char[2 * template.Count - 1];
            for (var i = 0; i < template.Count - 1; i++)
            {
                var key = new string(new[] {template[i], template[i + 1]});
                rc[2 * i] = key[0];
                rc[2 * i + 1] = rules[key];
            }

            rc[^1] = template[^1];
            return rc;
        }

        private static long SolvePart2(char[] polymerTemplate, IReadOnlyDictionary<string, char> insertionRules)
        {
            const int stepCount = 40;
            var pairLoopData = new Dictionary<string, Dictionary<int, Dictionary<char, long>>>();
            foreach (var s in insertionRules.Keys) CalculatePairLoops(s, insertionRules, stepCount, pairLoopData);

            var charCounts = new Dictionary<char, long>();
            // we add the first character of the original template...
            charCounts.SafeIncrement(polymerTemplate[0]);
            polymerTemplate.Window(2).Select(c => new string(c.ToArray())).ForEach(window =>
            {
                foreach (var (key, value) in pairLoopData[window][stepCount]) charCounts.SafeIncrement(key, value);
                // .. and decrement for the first character of the window, as it is the last one of the previous window
                charCounts.SafeDecrement(window[0]);
            });
            return charCounts.Max(x => x.Value) - charCounts.Min(x => x.Value);
        }

        private static void CalculatePairLoops(string pair, IReadOnlyDictionary<string, char> rules, int steps,
            IDictionary<string, Dictionary<int, Dictionary<char, long>>> pairLoopData)
        {
            if (pairLoopData.ContainsKey(pair) && pairLoopData[pair].ContainsKey(steps)) return;

            var polymerized = Polymerize(pair, rules);
            var charCounts = new Dictionary<char, long>();
            if (steps == 1)
                foreach (var c in polymerized)
                    charCounts.SafeIncrement(c);
            else
            {
                var left = string.Concat(pair[0], rules[pair]);
                var right = string.Concat(rules[pair], pair[1]);
                if (!pairLoopData.ContainsKey(left) || !pairLoopData[left].ContainsKey(steps - 1))
                    CalculatePairLoops(left, rules, steps - 1, pairLoopData);
                if (!pairLoopData.ContainsKey(right) || !pairLoopData[right].ContainsKey(steps - 1))
                    CalculatePairLoops(right, rules, steps - 1, pairLoopData);
                foreach (var (key, value) in pairLoopData[left][steps - 1]) charCounts.SafeIncrement(key, value);
                foreach (var (key, value) in pairLoopData[right][steps - 1]) charCounts.SafeIncrement(key, value);
                // the middle char gets added for both left and right, compensate
                charCounts.SafeDecrement(polymerized[1]);
            }

            if (!pairLoopData.ContainsKey(pair)) pairLoopData[pair] = new Dictionary<int, Dictionary<char, long>>();
            pairLoopData[pair].Add(steps, charCounts);
        }

        private static string Polymerize(string pair, IReadOnlyDictionary<string, char> rules) =>
            string.Concat(pair[0], rules[pair], pair[1]);
    }

    internal static class Day14Extensions
    {
        public static void SafeIncrement<TKey>(this Dictionary<TKey, long> dict, TKey key)
        {
            if (dict.ContainsKey(key)) dict[key]++;
            else dict.Add(key, 1L);
        }

        public static void SafeIncrement<TKey>(this Dictionary<TKey, long> dict, TKey key, long value)
        {
            if (dict.ContainsKey(key)) dict[key] += value;
            else dict.Add(key, value);
        }

        public static void SafeDecrement<TKey>(this Dictionary<TKey, long> dict, TKey key)
        {
            if (dict.ContainsKey(key)) dict[key]--;
            else dict.Add(key, -1L);
        }
    }
}