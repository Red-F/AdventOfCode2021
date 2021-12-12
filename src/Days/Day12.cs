namespace AdventOfCode.Days
{
    [Day(2021, 12)]
    public class Day12 : BaseDay
    {
        public override string PartOne(string input) =>
            CountPathsFromStartToEnd(Network(input), EntranceGuardOne).Count().ToString();

        public override string PartTwo(string input) =>
            CountPathsFromStartToEnd(Network(input), EntranceGuardTwo).Count().ToString();

        private static Node[] Network(string data)
        {
            var paths = data.ParseStrings(ParsePath).ToArray();
            var nodeNames = paths.SelectMany(s => s).Distinct().ToArray();
            var nodes = new Node[nodeNames.Length];
            for (var i = 0; i < nodeNames.Length; i++) nodes[i] = new Node(nodeNames[i], new List<int>());
            foreach (var path in paths)
            {
                var start = nodes.IndexOf(nodes.First(n => n.Name == path[0]));
                var end = nodes.IndexOf(nodes.First(n => n.Name == path[1]));
                nodes[start].Connections.Add(end);
                nodes[end].Connections.Add(start);
            }

            return nodes;
        }

        private static string[] ParsePath(string arg) => arg.Split('-');

        private static IEnumerable<string> CountPathsFromStartToEnd(IList<Node> network,
            Func<string, HashSet<string>, bool, (bool canEnter, bool smallCaveEnteredTwice)> guard)
        {
            var routes = new List<string>();
            var startNode = network.First(n => n.Name == "start");
            foreach (var connection in startNode.Connections)
                routes.AddRange(CountToEndFromNode(network[connection], network, "start", new HashSet<string>(), false,
                    guard));
            return routes;
        }

        private static IEnumerable<string> CountToEndFromNode(Node node, IList<Node> network, string path,
            HashSet<string> visited, bool smallCaveEnteredTwice,
            Func<string, HashSet<string>, bool, (bool canEnter, bool smallCaveEnteredTwice)> guard)
        {
            var routes = new List<string>();
            var connections = new List<int>(node.Connections);
            path = path + $",{node.Name}";
            visited.Add(node.Name);
            foreach (var connection in connections)
            {
                if (network[connection].Name == "start") continue;
                if (network[connection].Name == "end") routes.Add(path + ",end");
                else
                {
                    var (canEnter, twice) = guard(network[connection].Name, visited, smallCaveEnteredTwice);
                    if (canEnter)
                        routes.AddRange(CountToEndFromNode(network[connection], network, path,
                            new HashSet<string>(visited), twice, guard));
                }
            }

            return routes;
        }

        private static (bool canEnter, bool smallCaveEnteredTwice) EntranceGuardOne(string nextCaveName,
            HashSet<string> visited, bool smallCaveEnteredTwice) =>
            (nextCaveName.All(char.IsUpper) || !visited.Contains(nextCaveName), smallCaveEnteredTwice);

        private static (bool canEnter, bool smallCaveEnteredTwice) EntranceGuardTwo(string nextCaveName,
            HashSet<string> visited, bool smallCaveEnteredTwice)
        {
            if (nextCaveName.All(char.IsUpper)) return (true, smallCaveEnteredTwice);
            if (!visited.Contains(nextCaveName)) return (true, smallCaveEnteredTwice);
            return smallCaveEnteredTwice ? (false, true) : (true, visited.Contains(nextCaveName));
        }

        private record Node(string Name, List<int> Connections);
    }
}