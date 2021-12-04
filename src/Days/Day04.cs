namespace AdventOfCode.Days
{
    [Day(2021, 4)]
    public class Day04 : BaseDay
    {
        public override string PartOne(string input)
        {
            var numbersCalled = input.Lines().First().Integers().ToArray();
            var boards = ParseAllBoards(input.Lines().Skip(1).ToArray()).ToArray();
            foreach (var numberCalled in numbersCalled)
            {
                foreach (var board in boards)
                {
                    if (Mark(board, numberCalled))
                        return (SumUnmarked(board) * numberCalled).ToString();
                }
            }

            return string.Empty;
        }

        public override string PartTwo(string input)
        {
            var numbersCalled = input.Lines().First().Integers().ToArray();
            var boards = ParseAllBoards(input.Lines().Skip(1).ToArray()).ToArray();
            foreach (var numberCalled in numbersCalled)
            {
                foreach (var board in boards)
                {
                    if (!Mark(board, numberCalled)) continue;
                    if (boards.Count() == 1) return (SumUnmarked(boards.First()) * numberCalled).ToString();
                    boards = boards.Where(x => x != board).ToArray();
                }
            }

            return string.Empty;
        }

        private static long SumUnmarked(Field[,] board)
        {
            var rc = 0L;
            for (var x = 0; x < 5; x++)
            {
                for (var y = 0; y < 5; y++)
                {
                    if (!board[x, y].Marked)
                        rc += board[x, y].Value;
                }
            }

            return rc;
        }

        private static bool Mark(Field[,] board, int numberCalled)
        {
            for (var x = 0; x < 5; x++)
            {
                for (var y = 0; y < 5; y++)
                {
                    if (board[x, y].Value != numberCalled) continue;
                    board[x, y] = board[x, y] with {Marked = true};
                    return IsWinning(board);
                }
            }

            return false;
        }

        private static bool IsWinning(Field[,] board)
        {
            for (var i = 0; i < 5; i++)
            {
                // current row and column
                if (board[i, 0].Marked && board[i, 1].Marked && board[i, 2].Marked && board[i, 3].Marked &&
                    board[i, 4].Marked) return true;
                if (board[0, i].Marked && board[1, i].Marked && board[2, i].Marked && board[3, i].Marked &&
                    board[4, i].Marked) return true;
            }

            return false;
        }

        private static IEnumerable<Field[,]> ParseAllBoards(string[] data)
        {
            var boards = new List<Field[,]>();
            while (data.Length > 0)
            {
                boards.Add(data.Take(5).Select(x => x.Integers().Select(y => new Field(y, false)).ToArray()).ToList()
                    .CreateGrid());
                data = data.Skip(5).ToArray();
            }

            return boards.ToArray();
        }
    }

    internal record Field(int Value, bool Marked);
}