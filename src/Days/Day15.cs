using System.Drawing;
using System.Globalization;

namespace AdventOfCode.Days
{
    [Day(2021, 15)]
    public class Day15 : BaseDay
    {
        public override string PartOne(string input)
        {
            var grid = input.CreateCharGrid();
            var nodes = GridToNodes(grid);
            // Dijkstra performs better because cost is not related to distance
            Dijkstra(nodes.First(), nodes.Last());
            // Astar(nodes.First(), nodes.Last());
            var shortestPath = BuildShortestPath(nodes.Last());
            var rc = shortestPath.Sum(n => n.ClosestToStart.Connections.First(x => x.ConnectedTo == n).Cost);
            return rc.ToString(CultureInfo.InvariantCulture);
        }

        public override string PartTwo(string input)
        {
            var grid = input.CreateCharGrid();
            var nodes = GridTo5Nodes(grid);
            // Dijkstra performs better because cost is not related to distance
            Dijkstra(nodes.First(), nodes.Last());
            // Astar(nodes.First(), nodes.Last());
            var shortestPath = BuildShortestPath(nodes.Last());
            var rc = shortestPath.Sum(n => n.ClosestToStart.Connections.First(x => x.ConnectedTo == n).Cost);
            return rc.ToString(CultureInfo.InvariantCulture);
        }

        private static List<Node> GridToNodes(char[,] grid)
        {
            var nodeGrid = new Node[grid.GetLength(0), grid.GetLength(1)];
            var endpoint = new Point(grid.GetLength(0) - 1, grid.GetLength(1) - 1);
            var rc = new List<Node>();
            for (var x = 0; x < grid.GetLength(0); x++)
            for (var y = 0; y < grid.GetLength(1); y++)
            {
                var point = new Point(x, y);
                var value = grid[x, y] - '0';
                var node = new Node(value);
                node.StraightLineDistanceToEnd = CalculateStraightLineDistanceToEnd(point, endpoint);
                SetNode(point, nodeGrid, node);
                rc.Add(node);
            }

            return rc;
        }

        private static List<Node> GridTo5Nodes(char[,] grid)
        {
            var nodeGrid = new Node[grid.GetLength(0) * 5, grid.GetLength(1) * 5];
            var endpoint = new Point(grid.GetLength(0) * 5 - 1, grid.GetLength(1) * 5 - 1);
            var rc = new List<Node>();
            for (var x = 0; x < grid.GetLength(0); x++)
            for (var y = 0; y < grid.GetLength(1); y++)
            {
                if (x == 9 && y == 9) Console.WriteLine("blup");
                var value = grid[x, y] - '0';
                for (var i = 0; i < 5; i++)
                for (var j = 0; j < 5; j++)
                {
                    var point = new Point(x + i * grid.GetLength(0), y + j * grid.GetLength(1));
                    var v = value + i + j;
                    var node = new Node(((v % 10) + (v / 10)) % 10);
                    node.StraightLineDistanceToEnd = CalculateStraightLineDistanceToEnd(point, endpoint);
                    SetNode(point, nodeGrid, node);
                    rc.Add(node);
                }
            }
            return rc;
        }

        private static void SetNode(Point point, Node[,] nodeGrid, Node node)
        {
            var neighbours = nodeGrid.GetNeighborPoints(point);
            foreach (var neighbour in neighbours)
            {
                if (nodeGrid[neighbour.point.X, neighbour.point.Y] is not { } n) continue;
                n.Connections.Add(new(node.Cost, node));
                node.Connections.Add(new (n.Cost, n));
            }

            nodeGrid[point.X, point.Y] = node;
        }

        private static double CalculateStraightLineDistanceToEnd(Point point, Point endpoint) =>
            point.ManhattanDistance(endpoint);

        private static IEnumerable<Node> BuildShortestPath(Node last)
        {
            var rc = new List<Node> {last};
            var current = last.ClosestToStart;
            while (current.ClosestToStart != null)
            {
                rc.Add(current);
                current = current.ClosestToStart;
            }

            rc.Reverse();
            return rc;
        }

        private class Node
        {
            public Node(long cost)
            {
                Cost = cost;
            }

            public long Cost { get; }
            public bool Visited { get; set; }
            public double? MinimumCostToStart { get; set; }
            public double StraightLineDistanceToEnd { get; set; }
            public Node ClosestToStart { get; set; }
            public readonly List<Edge> Connections = new();
        }

        private record Edge(double Cost, Node ConnectedTo);

        private static void Dijkstra(Node startNode, Node endNode)
        {
            startNode.MinimumCostToStart = 0;
            var priorityQ = new List<Node> {startNode};
            do
            {
                priorityQ = priorityQ.OrderBy(x => x.MinimumCostToStart).ToList();
                var node = priorityQ.First();
                priorityQ.Remove(node);
                foreach (var (cost, childNode) in node.Connections.OrderBy(n => n.Cost))
                {
                    if (childNode.Visited) continue;
                    if (childNode.MinimumCostToStart == null ||
                        node.MinimumCostToStart + cost < childNode.MinimumCostToStart)
                    {
                        childNode.MinimumCostToStart = node.MinimumCostToStart + cost;
                        childNode.ClosestToStart = node;
                        if (!priorityQ.Contains(childNode)) priorityQ.Add(childNode);
                    }
                }

                node.Visited = true;
                if (node == endNode) return;
            } while (priorityQ.Any());
        }
        
        private void Astar(Node startNode, Node endNode)
        {
            startNode.MinimumCostToStart = 0;
            var priorityQ = new List<Node> {startNode};
            do {
                priorityQ = priorityQ.OrderBy(x => x.MinimumCostToStart + x.StraightLineDistanceToEnd).ToList();
                var node = priorityQ.First();
                priorityQ.Remove(node);
                foreach (var connection in node.Connections.OrderBy(x => x.Cost))
                {
                    var childNode = connection.ConnectedTo;
                    if (childNode.Visited) continue;
                    if (childNode.MinimumCostToStart == null ||
                        node.MinimumCostToStart + connection.Cost < childNode.MinimumCostToStart)
                    {
                        childNode.MinimumCostToStart = node.MinimumCostToStart + connection.Cost;
                        childNode.ClosestToStart = node;
                        if (!priorityQ.Contains(childNode)) priorityQ.Add(childNode);
                    }
                }
                node.Visited = true;
                if (node == endNode)
                    return;
            } while (priorityQ.Any());
        }
    }
}