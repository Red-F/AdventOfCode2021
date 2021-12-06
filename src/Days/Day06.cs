namespace AdventOfCode.Days
{
    [Day(2021, 6)]
    public class Day06 : BaseDay
    {
        public override string PartOne(string input)
        {
            var fish = input.Integers().ToList();
            return GrowFishColonySimple(fish, 80).ToString();
        }

        public override string PartTwo(string input)
        {
            var fish = input.Integers().ToList();
            return GrowFishColony(fish, 256).ToString();
        }

        private static long GrowFishColonySimple(List<int> fish, int days)
        {
            for (var i = 0; i < days; i++)
            {
                var colonySize = fish.Count;
                for (var j = 0; j < colonySize; j++)
                {
                    if (--fish[j] >= 0) continue;
                    fish[j] = 6;
                    fish.Add(8);
                }
            }

            return fish.Count;
        }

        private static long GrowFishColony(List<int> fish, int days)
        {
            // fish timers can be 0..8
            var fishAges = new long[9];
            foreach (var f in fish) fishAges[f]++;
            var originalDay0 = 0L;
            for (var i = 0; i < days; i++)
            {
                for (var j = 0; j < fishAges.Length; j++)
                {
                    if (j == 0) originalDay0 = fishAges[j];
                    if (j == 8) fishAges[j] = originalDay0;
                    else if (j == 6) fishAges[j] = fishAges[j + 1] + originalDay0;
                    else fishAges[j] = fishAges[j + 1];
                }
            }

            return fishAges.Sum();
        }
    }
}
