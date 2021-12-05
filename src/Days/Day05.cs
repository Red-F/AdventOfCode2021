using System.Drawing;

namespace AdventOfCode.Days
{
    [Day(2021, 5)]
    public class Day05 : BaseDay
    {
        public override string PartOne(string input)
        {
            var lines = input.ParseStrings(ParseLine).ToArray();
            lines = lines.Where(p => p.from.X == p.to.X || p.from.Y == p.to.Y).ToArray();
            return CreateGrid(lines).CountGrid().ToString();
        }

        public override string PartTwo(string input)
        {
            var lines = input.ParseStrings(ParseLine).ToArray();
            return CreateGrid(lines).CountGrid().ToString();
        }

        private static (Point from, Point to) ParseLine(string arg)
        {
            var parts = arg.Split(' ');
            return (parts[0].ToPoint(), parts[2].ToPoint());
        }

        private static int[,] CreateGrid((Point from, Point to)[] lines)
        {
            var maxX = lines.Select(p => Math.Max(p.from.X, p.to.X)).Max() + 1;
            var maxY = lines.Select(p => Math.Max(p.from.Y, p.to.Y)).Max() + 1;
            var rc = new int[maxX, maxY];
            foreach (var line in lines)
            {
                var dX = line.from.X == line.to.X ? 0 : (line.to.X - line.from.X) / Math.Abs(line.to.X - line.from.X);
                var dY = line.from.Y == line.to.Y ? 0 : (line.to.Y - line.from.Y) / Math.Abs(line.to.Y - line.from.Y);
                var current = new Point(line.from.X - dX, line.from.Y - dY);
                do
                {
                    current.X += dX;
                    current.Y += dY;
                    rc[current.X, current.Y]++;
                } while (current != line.to);
            }

            return rc;
        }
    }

    internal static class Day05Extensions
    {
        internal static int CountGrid(this int[,] rc)
        {
            var count = 0;
            for (var x = 0; x < rc.GetLength(0); x++)
            {
                for (var y = 0; y < rc.GetLength(1); y++)
                {
                    if (rc[x, y] > 1) count++;
                }
            }
            return count;
        }
    }
}