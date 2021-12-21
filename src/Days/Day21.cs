namespace AdventOfCode.Days
{
    [Day(2021, 21)]
    public class Day21 : BaseDay
    {
        public override string PartOne(string input)
        {
            var game = Parse(input);
            var die = new DeterministicDie(100);
            var (score, rolls) = PlayDeterministicGame(game, die, 0);
            return (rolls * score).ToString();
        }

        public override string PartTwo(string input)
        {
            var game = Parse(input);
            var (player1Wins, player2Wins) = PlayQuantumGame(game);
            return player1Wins > player2Wins ? player1Wins.ToString() : player2Wins.ToString();
        }

        private static (int score, int rolls) PlayDeterministicGame(Game game, DeterministicDie deterministicDie, int rolls)
        {
            if (game.PlayersTurn == 1)
            {
                var places = deterministicDie.Roll + deterministicDie.Roll + deterministicDie.Roll;
                places %= 10;
                rolls += 3;
                var position = ((game.Player1Position - 1 + places) % 10) + 1;
                var score = game.Player1Score + position;
                if (score >= 1000) return (game.Player2Score, rolls);
                var nextGame = new Game(2, position, score, game.Player2Position, game.Player2Score);
                return PlayDeterministicGame(nextGame, deterministicDie, rolls);
            }
            else
            {
                var places = deterministicDie.Roll + deterministicDie.Roll + deterministicDie.Roll;
                places %= 10;
                rolls += 3;
                var position = ((game.Player2Position - 1 + places) % 10) + 1;
                var score = game.Player2Score + position;
                if (score >= 1000) return (game.Player1Score, rolls);
                var nextGame = new Game(1, game.Player1Position, game.Player1Score, position, score);
                return PlayDeterministicGame(nextGame, deterministicDie, rolls);
            }
        }

        private readonly Dictionary<Game, (ulong player1Wins, ulong player2Wins)> _gameCache = new();

        private (ulong player1Wins, ulong player2Wins) PlayQuantumGame(Game game)
        {
            if (game.Player1Score >= 21) return (1, 0);
            if (game.Player2Score >= 21) return (0, 1);
            if (_gameCache.ContainsKey(game)) return _gameCache[game];

            var threeRollUniverses = new (ulong nrOfUniverses, int places)[]
                {(1, 3), (3, 4), (6, 5), (7, 6), (6, 7), (3, 8), (1, 9)};
            var wins = (player1: 0UL, player2: 0UL);
            foreach (var (nrOfUniverses, places) in threeRollUniverses)
            {
                if (game.PlayersTurn == 1)
                {
                    var position = (game.Player1Position - 1 + places) % 10 + 1;
                    var score = game.Player1Score + position;
                    var nextGame = new Game(2, position, score, game.Player2Position, game.Player2Score);
                    var (player1Wins, player2Wins) = PlayQuantumGame(nextGame);
                    wins.player1 += player1Wins * nrOfUniverses;
                    wins.player2 += player2Wins * nrOfUniverses;
                }
                else
                {
                    var position = (game.Player2Position - 1 + places) % 10 + 1;
                    var score = game.Player2Score + position;
                    var nextGame = new Game(1, game.Player1Position, game.Player1Score, position, score);
                    var (player1Wins, player2Wins) = PlayQuantumGame(nextGame);
                    wins.player1 += player1Wins * nrOfUniverses;
                    wins.player2 += player2Wins * nrOfUniverses;
                }
            }

            _gameCache.Add(game, wins);
            return wins;
        }

        private record Game(int PlayersTurn, int Player1Position, int Player1Score, int Player2Position,
            int Player2Score);

        private static Game Parse(string data)
        {
            var (item1, item2) = (ParsePlayer(data.Lines().First()), ParsePlayer(data.Lines().Last()));
            return new Game(1, item1, 0, item2, 0);
        }

        private static int ParsePlayer(string line) => int.Parse(line.Split(' ')[4]);

        private record DeterministicDie(int NrOfSides)
        {
            private int _nextValue;
            public int Roll => _nextValue++ % NrOfSides + 1;
        }
    }
}