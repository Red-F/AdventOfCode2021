namespace AdventOfCode.Days
{
    [Day(2021, 3)]
    public class Day03 : BaseDay
    {
        public override string PartOne(string input)
        {
            var diagnosticReport = input.Lines().ToList();
            var (gammaRate, epsilonRate) = GammaEpsilonRates(diagnosticReport);
            return (Convert.ToInt64(gammaRate, 2) * Convert.ToInt64(epsilonRate, 2)).ToString();
        }

        public override string PartTwo(string input)
        {
            var diagnosticReport = input.Lines().ToList();
            var (oxygenGeneratorRating, co2ScrubberRating) = OxygenCo2Rates(diagnosticReport);
            return (Convert.ToInt64(oxygenGeneratorRating, 2) * Convert.ToInt64(co2ScrubberRating, 2)).ToString();
        }

        private static (string gammaRate, string epsilonRate) GammaEpsilonRates(IReadOnlyList<string> data)
        {
            var gamma = string.Empty;
            var epsilon = string.Empty;
            for (var i = 0; i < data[0].Length; i++)
            {
                var zeroCount = data.Count(t => t[i] == '0');
                if (zeroCount > data.Count / 2)
                {
                    gamma += '0';
                    epsilon += '1';
                }
                else
                {
                    gamma += '1';
                    epsilon += '0';
                }
            }

            return (gamma, epsilon);
        }

        private static (string oxygenGeneratorRating, string co2ScrubberRating) OxygenCo2Rates(IReadOnlyCollection<string> data)
        {
            var oxygen = new List<string>(data);
            var co2 = new List<string>(data);
            var index = 0;
            while (oxygen.Count > 1)
            {
                var (gammaRate, _) = GammaEpsilonRates(oxygen);
                oxygen = oxygen.Where(x => x[index] == gammaRate[index]).ToList();
                index++;
            }

            index = 0;
            while (co2.Count > 1)
            {
                var (gammaRate, _) = GammaEpsilonRates(co2);
                co2 = co2.Where(x => x[index] != gammaRate[index]).ToList();
                index++;
            }

            return (oxygen[0], co2[0]);
        }
    }
}