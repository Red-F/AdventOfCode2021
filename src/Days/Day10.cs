namespace AdventOfCode.Days
{
    [Day(2021, 10)]
    public class Day10 : BaseDay
    {
        public override string PartOne(string input) =>
            Analyse(input.Lines()).Where(x => x.corrupt).Select(x => x.val).Sum().ToString();

        public override string PartTwo(string input)
        {
            var scores = Analyse(input.Lines()).Where(x => !x.corrupt).Select(p => p.val).OrderBy(l => l).ToList();
            return scores[(scores.Count / 2)].ToString();
        }

        private static IEnumerable<(bool corrupt, long val)> Analyse(IEnumerable<string> lines) =>
            lines.Select(CheckLine).ToList();

        private static (bool corrupt, long val) CheckLine(string line)
        {
            var openings = new Stack<char>();
            foreach (var t in line)
            {
                switch (t)
                {
                    case '(': case '[': case '{': case '<':
                        openings.Push(t);
                        break;
                    case ')':
                        if (openings.Pop() != '(') return (true, 3L);
                        break;
                    case ']': 
                        if (openings.Pop() != '[') return (true, 57L);
                        break;
                    case '}': 
                        if (openings.Pop() != '{') return (true, 1197L);
                        break;
                    case '>':
                        if (openings.Pop() != '<') return (true, 25137L);
                        break;
                }
            }

            var score = string.Concat(openings.Select(c => c)).Aggregate(0L,
                (acc, elem) => (5L * acc) + elem switch
                {
                    '(' => 1, '[' => 2, '{' => 3, '<' => 4, _ => throw new InvalidOperationException()
                });
            return (false, score);
        }
    }
}
