using System.Drawing;

namespace AdventOfCode.Days
{
    [Day(2021, 9)]
    public class Day09 : BaseDay
    {
        public override string PartOne(string input)
        {
            var grid = input.CreateCharGrid();
            return GetLowPoints(grid).Sum(x => x.value + 1).ToString();
        }

        public override string PartTwo(string input)
        {
            var grid = input.CreateCharGrid();
            var basins = GetBasins(grid).OrderByDescending(b => b.size).Take(3).ToList();
            return basins.Select(b => b.size).Multiply().ToString();
        }

        private static IEnumerable<(Point position, int value)> GetLowPoints(char[,] grid)
        {
            var rc = new List<(Point position, int value)>();
            for (var x = 0; x < grid.GetLength(0); x++)
            for (var y = 0; y < grid.GetLength(1); y++)
                if (grid.GetNeighbors(x, y, false).All(c => c > grid[x, y]))
                    rc.Add((new Point(x, y), grid[x, y] - '0'));
            return rc;
        }

        private static IEnumerable<(long size, IEnumerable<Point> area)> GetBasins(char[,] grid) =>
            GetLowPoints(grid).Select(lowPoint => GetBasin(grid, lowPoint.position)).ToList();

        private static (long size, IEnumerable<Point> area) GetBasin(char[,] grid, Point position)
        {
            var rc = (size: 1L, area: new List<Point> {position});
            grid[position.X, position.Y] = '#';
            while(true)
            {
                var extensions = grid.GetNeighborPoints(position.X, position.Y).Where(p => p.c is not '9' and not '#')
                    .ToList();
                if (!extensions.Any()) return rc;
                var neighbor = extensions.First();
                var (size, area) = GetBasin(grid, neighbor.point);
                rc.size += size;
                rc.area.AddRange(area);
            }
        }
    }
}