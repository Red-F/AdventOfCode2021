namespace AdventOfCode.Days
{
    [Day(2021, 11)]
    public class Day11 : BaseDay
    {
        public override string PartOne(string input)
        {
            var grid = input.CreateCharGrid().Map(c => (value: c - '0', flashed: false));
            var rc = 0L;
            for (var i = 0; i < 100; i++) rc += Step(grid);
            return rc.ToString();
        }

        public override string PartTwo(string input)
        {
            var grid = input.CreateCharGrid().Map(c => (value: c - '0', flashed: false));
            var rc = 1L;
            var maxFlashCount = grid.GetLength(0) * grid.GetLength(1);
            while (Step(grid) != maxFlashCount) rc++;
            return rc.ToString();
        }

        private static long Step((int value, bool flashed)[,] grid)
        {
            var rc = 0L;
            for (var x = 0; x < grid.GetLength(0); x++)
            for (var y = 0; y < grid.GetLength(1); y++)
                if (++grid[x, y].value >= 10 && !grid[x, y].flashed)
                    rc += Flash(grid, x, y);

            for (var x = 0; x < grid.GetLength(0); x++)
            for (var y = 0; y < grid.GetLength(1); y++)
            {
                if (!grid[x,y].flashed) continue;
                grid[x,y].value = 0;
                grid[x,y].flashed = false;
            };
            return rc;
        }

        private static long Flash((int value, bool flashed)[,] grid, int x, int y)
        {
            var rc = 1L;
            grid[x, y].flashed = true;
            var neighbors = grid.GetNeighborPoints(x, y, true).Where(p => !p.c.flashed).ToArray();
            foreach (var (point, _) in neighbors)
                if (++grid[point.X, point.Y].value >= 10 && !grid[point.X, point.Y].flashed)
                    rc += Flash(grid, point.X, point.Y);

            return rc;
        }
    }
}