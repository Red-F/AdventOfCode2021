using System.ComponentModel;
using System.Text;

namespace AdventOfCode.Days
{
    [Day(2021, 18)]
    public class Day18 : BaseDay
    {
        public override string PartOne(string input)
        {
            var lines = input.Lines().ToArray();
            var sum = lines.First();
            foreach (var line in lines.Skip(1))
            {
                sum = Add(sum, line);
                sum = Reduce(sum);
            }

            var rc = Magnitude(ParseString(sum));
            return rc.ToString();
        }

        public override string PartTwo(string input)
        {
            var lines = input.Lines().ToArray();
            var sums = new List<long>();
            for (var i = 0; i < lines.Length; i++)
                sums.AddRange(lines.Where((_, j) => i != j)
                    .Select(t => Magnitude(ParseString(Reduce(Add(lines[i], t))))));
            return sums.Max().ToString();
        }

        private static long Magnitude(Element e) => e switch
        {
            Element.ANumber n => n.Value,
            Element.APair p => (3L * Magnitude(p.Value.Left)) + 2L * Magnitude(p.Value.Right),
            _ => throw new InvalidEnumArgumentException()
        };

        private static string Add(string s1, string s2)
        {
            var sb = new StringBuilder();
            return sb.Append('[').Append(s1).Append(',').Append(s2).Append(']').ToString();
        }

        private static string Reduce(string s)
        {
            var mustReduce = true;
            while (mustReduce)
            {
                (mustReduce, s) = Explode(s);
                if (mustReduce) continue;
                (mustReduce, s) = Split(s);
            }

            return s;
        }

        private static (bool exploded, string explodedString) Explode(string s)
        {
            var nest = 0;
            for (var i = 0; i < s.Length; i++)
            {
                switch (s[i])
                {
                    case '[':
                        nest++;
                        break;
                    case ']':
                        nest--;
                        break;
                }

                if (nest <= 4) continue;
                var explodingPair = s.Substring(i, s.IndexOf(']', i) - i + 1);
                var parts = explodingPair.Split(new[] {',', '[', ']'}, StringSplitOptions.RemoveEmptyEntries);
                s = s[..i] + "0" + s[(i + explodingPair.Length)..];
                for (var j = i + 1; j < s.Length; j++)
                {
                    if (!char.IsDigit(s[j])) continue;
                    var k = j;
                    while (char.IsDigit(s[k])) k++;
                    var newValue = int.Parse(s.Substring(j, k - j)) + int.Parse(parts[1]);
                    s = s[..j] + newValue + s[k..];
                    break;
                }

                for (var j = i - 1; j > 0; j--)
                {
                    if (!char.IsDigit(s[j])) continue;
                    var k = j;
                    while (char.IsDigit(s[k])) k--;
                    var newValue = int.Parse(s.Substring(k + 1, j - k)) + int.Parse(parts[0]);
                    s = s[..(k + 1)] + newValue + s[(j + 1)..];
                    break;
                }

                return (true, s);
            }

            return (false, s);
        }

        private static (bool wasSplit, string splitString) Split(string s)
        {
            for (var i = 0; i < s.Length; i++)
            {
                if (!char.IsDigit(s[i])) continue;
                var j = i;
                while (char.IsDigit(s[j])) j++;
                var value = int.Parse(s.Substring(i, j - i));
                if (value >= 10)
                {
                    var left = value / 2;
                    var right = value / 2 + (value % 2 == 0 ? 0 : 1);
                    var newPair = $"[{left},{right}]";
                    s = s[..i] + newPair + s[j..];
                    return (true, s);
                }

                i = j;
            }

            return (false, s);
        }

        private static Element ParseString(string s)
        {
            var tokenizer = new Tokenizer(s);
            if (tokenizer.GetToken() != '[')
                throw new InvalidOperationException(
                    $"Expected token '[' but found {tokenizer.PreviousToken()} at {tokenizer.LastPosition}");
            return new Element.APair(ParsePair(tokenizer));
        }

        private static Pair ParsePair(Tokenizer t)
        {
            var left = ParseElement(t);
            if (t.GetToken() != ',')
                throw new InvalidOperationException(
                    $"Expected token ',' but found {t.PreviousToken()} at {t.LastPosition}");
            var right = ParseElement(t);
            if (t.GetToken() != ']')
                throw new InvalidOperationException(
                    $"Expected token ']' but found {t.PreviousToken()} at {t.LastPosition}");
            return new Pair(left, right);
        }

        private static Element ParseElement(Tokenizer t)
        {
            Element rc;
            var token = t.GetToken();
            if (char.IsDigit(token))
            {
                var sb = new StringBuilder(char.ToString(token));
                while (char.IsDigit(t.PeekToken())) sb.Append(char.ToString(t.GetToken()));

                rc = new Element.ANumber(int.Parse(sb.ToString()));
                if (t.PeekToken() is not ',' and not ']')
                    throw new InvalidOperationException(
                        $"Expected token ',' or ']' but found {t.PreviousToken()} at {t.LastPosition}");
            }
            else
            {
                if (token != '[')
                    throw new InvalidOperationException(
                        $"Expected token '[' but found {t.PreviousToken()} at {t.LastPosition}");
                rc = new Element.APair(ParsePair(t));
            }

            return rc;
        }

        private record Element
        {
            public record ANumber(int Value) : Element;

            public record APair(Pair Value) : Element;
        }

        private record Pair(Element Left, Element Right);

        private class Tokenizer
        {
            private readonly string _s;
            private int _index;

            public Tokenizer(string s)
            {
                _s = s;
                _index = 0;
            }

            public char GetToken() => _index < _s.Length ? _s[_index++] : '\0';
            public char PeekToken() => _index < _s.Length ? _s[_index] : '\0';
            public char PreviousToken() => _s[LastPosition];
            public int LastPosition => _index - 1;
        }
    }
}