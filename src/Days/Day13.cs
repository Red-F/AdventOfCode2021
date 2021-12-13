using System.Drawing;
using System.Text;

namespace AdventOfCode.Days
{
    [Day(2021, 13)]
    public class Day13 : BaseDay
    {
        public override string PartOne(string input)
        {
            var (points, folds) = Parse(input.Lines(true).ToArray());
            var grid = CreateGrid(points);
            return Fold(grid, new[] {folds[0]}).GetPoints('#').Count().ToString();
        }

        public override string PartTwo(string input)
        {
            var (points, folds) = Parse(input.Lines(true).ToArray());
            var grid = CreateGrid(points);
            grid = Fold(grid, folds);
            return PrintGrid(grid);
        }

        private static string PrintGrid(char[,] grid)
        {
            var rc = new StringBuilder();
            rc.Append(Environment.NewLine);
            for (var y = 0; y < grid.GetLength(1); y++)
            {
                for (var x = 0; x < grid.GetLength(0); x++) rc.Append(grid[x, y] == '#' ? '#' : ' ');
                rc.Append(Environment.NewLine);
            }

            return rc.ToString();
        }

        private static char[,] Fold(char[,] grid, string[] folds)
        {
            for (var i = 0; i < folds.Length; i++)
            {
                var instruction = folds[i].Split('=');
                grid = instruction[0][instruction[0].Length - 1] == 'y'
                    ? FoldY(grid, int.Parse(instruction[1]))
                    : FoldX(grid, int.Parse(instruction[1]));
            }

            return grid;
        }

        private static char[,] FoldX(char[,] grid, int foldLine)
        {
            var maxX = grid.GetLength(0);
            var rc = new char[maxX - foldLine - 1, grid.GetLength(1)];
            for (var x = foldLine + 1; 2 * foldLine - x >= 0; x++)
            for (var y = 0; y < rc.GetLength(1); y++)
                rc[2 * foldLine - x, y] = grid[2 * foldLine - x, y] == '#' || grid[x, y] == '#' ? '#' : ' ';

            return rc;
        }

        private static char[,] FoldY(char[,] grid, int foldLine)
        {
            var maxY = grid.GetLength(1);
            var rc = new char[grid.GetLength(0), maxY - foldLine - 1];
            for (var y = foldLine + 1; 2 * foldLine - y >= 0; y++)
            for (var x = 0; x < rc.GetLength(0); x++)
                rc[x, 2 * foldLine - y] = grid[x, 2 * foldLine - y] == '#' || grid[x, y] == '#' ? '#' : ' ';

            return rc;
        }

        private static (Point[] points, string[] folds) Parse(IReadOnlyList<string> lines)
        {
            var i = 0;
            var points = new List<Point>();
            while (lines[i] != string.Empty)
            {
                points.Add(lines[i++].ToPoint());
            }

            var folds = new List<string>();

            while (i < lines.Count)
            {
                if (lines[i] != string.Empty) folds.Add(lines[i]);
                i++;
            }

            return (points.ToArray(), folds.ToArray());
        }

        private static char[,] CreateGrid(Point[] points)
        {
            var maxX = points.MaxBy(p => p.X).X + 1;
            var maxY = points.MaxBy(p => p.Y).Y + 1;
            var grid = new char[maxX + 1, maxY + 1];
            foreach (var point in points) grid[point.X, point.Y] = '#';
            return grid;
        }
    }
}