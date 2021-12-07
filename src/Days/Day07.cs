namespace AdventOfCode.Days
{
    [Day(2021, 7)]
    public class Day07 : BaseDay
    {
        public override string PartOne(string input)
        {
            var positions = input.Longs().ToList();
            var median = Median(positions);
            return FuelCost(positions, median).ToString();
        }

        public override string PartTwo(string input)
        {
            var positions = input.Longs().ToArray();
            var mean = Mean(positions);
            return CrabFuelCost(positions, mean).ToString();
        }

        private static long Mean(IEnumerable<long> positions)
        {
            var enumerable = positions as long[] ?? positions.ToArray();
            return enumerable.Sum() / enumerable.Length;
        }

        private static long Median(IReadOnlyCollection<long> positions)
        {
            var ordered = new List<long>(positions);
            ordered.Sort();
            return ordered[positions.Count / 2];
        }

        private static long FuelCost(IEnumerable<long> positions, long target) =>
            positions.Select(p => Math.Abs(p - target)).Sum();

        private static long CrabFuelCost(long[] positions, long target)
        {
            return positions.Select(p => (Math.Abs(p - target) * (Math.Abs(p - target) + 1)) / 2L).Sum();
        }
    }
}