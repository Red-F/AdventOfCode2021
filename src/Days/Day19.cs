namespace AdventOfCode.Days
{
    [Day(2021, 19)]
    public class Day19 : BaseDay
    {
        public override string PartOne(string input)
        {
            var scanners = OrientateAllScanners(ParseScanners(input)).ToArray();
            var orientedBeaconPositions = BeaconPositions(scanners);
            return orientedBeaconPositions.Length.ToString();
        }

        public override string PartTwo(string input)
        {
            var scanners = OrientateAllScanners(ParseScanners(input)).ToArray();
            var maxDistance = scanners.Aggregate(0L,
                (current, t) => scanners.Select(t1 => t.Position.GetManhattanDistance(t1.Position)).Prepend(current)
                    .Max());

            return maxDistance.ToString();
        }

        private static Point3D[] BeaconPositions(IEnumerable<Scanner> scanners)
        {
            var orientedBeaconPositions = new List<Point3D>();
            foreach (var scanner in scanners) orientedBeaconPositions.AddRange(scanner.Beacons.Select(x => x.Point));
            return orientedBeaconPositions.Distinct().ToArray();
        }

        private static List<Scanner> OrientateAllScanners(List<Scanner> scanners)
        {
            var orientedScanners = new List<Scanner> {scanners.First()};
            scanners.Remove(scanners.First());
            var scannerIndex = 0;
            while (scanners.Count > 0)
            {
                var currentScanner = orientedScanners[scannerIndex++];
                var processScanners = new List<Scanner>();
                foreach (var nextScanner in scanners)
                {
                    var overlapping = Overlapping(currentScanner, nextScanner);
                    if (overlapping.Length < 12) continue;
                    processScanners.Add(nextScanner);
                    orientedScanners.Add(Reorient(nextScanner with { }, overlapping));
                }

                foreach (var s in processScanners) scanners.Remove(s);
            }

            return orientedScanners;
        }

        private static Scanner Reorient(Scanner s, (Point3D b1, Point3D b2)[] matchingPoints)
        {
            for (var s1X = 0; s1X < 4; s1X++)
            {
                for (var s1Y = 0; s1Y < 4; s1Y++)
                {
                    for (var s1Z = 0; s1Z < 4; s1Z++)
                    {
                        var vector = matchingPoints[0].b1 - matchingPoints[0].b2;
                        if (matchingPoints.All(x => x.b2 + vector == x.b1))
                            return TranslateScanner(s, vector);

                        RotateZScanner(s);
                        RotateZMatchingPoints(matchingPoints);
                    }

                    RotateYScanner(s);
                    RotateYMatchingPoints(matchingPoints);
                }

                RotateXScanner(s);
                RotateXMatchingPoints(matchingPoints);
            }

            throw new Exception("should never get here");
        }

        private static (Point3D b1, Point3D b2)[] Overlapping(Scanner s1, Scanner s2)
        {
            var rc = new List<(Point3D b1, Point3D b2)>();
            foreach (var s1Beacon in s1.Beacons)
            {
                foreach (var s2Beacon in s2.Beacons)
                {
                    var count = s1Beacon.Distances.Count(d => s2Beacon.Distances.Contains(d));
                    if (count >= 11) rc.Add((s1Beacon.Point, s2Beacon.Point));
                }
            }

            return rc.ToArray();
        }

        private static Scanner TranslateScanner(Scanner scanner, Point3D vector)
        {
            for (var i = 0; i < scanner.Beacons.Length; i++)
                scanner.Beacons[i] = scanner.Beacons[i] with {Point = scanner.Beacons[i].Point + vector};
            return scanner with {Position = vector};
        }

        private static void RotateXScanner(Scanner scanner)
        {
            for (var i = 0; i < scanner.Beacons.Length; i++)
                scanner.Beacons[i] = scanner.Beacons[i] with {Point = RotateX(scanner.Beacons[i].Point)};
        }

        private static void RotateYScanner(Scanner scanner)
        {
            for (var i = 0; i < scanner.Beacons.Length; i++)
                scanner.Beacons[i] = scanner.Beacons[i] with {Point = RotateY(scanner.Beacons[i].Point)};
        }

        private static void RotateZScanner(Scanner scanner)
        {
            for (var i = 0; i < scanner.Beacons.Length; i++)
                scanner.Beacons[i] = scanner.Beacons[i] with {Point = RotateZ(scanner.Beacons[i].Point)};
        }

        private static void RotateXMatchingPoints((Point3D reference, Point3D matched)[] points)
        {
            for (var i = 0; i < points.Length; i++)
                points[i] = (points[i].reference, RotateX(points[i].matched));
        }

        private static void RotateYMatchingPoints((Point3D reference, Point3D matched)[] points)
        {
            for (var i = 0; i < points.Length; i++)
                points[i] = (points[i].reference, RotateY(points[i].matched));
        }

        private static void RotateZMatchingPoints((Point3D reference, Point3D matched)[] points)
        {
            for (var i = 0; i < points.Length; i++)
                points[i] = (points[i].reference, RotateZ(points[i].matched));
        }

        private static Point3D RotateX(Point3D point) => new Point3D(point.X, -point.Z, point.Y);
        private static Point3D RotateY(Point3D point) => new Point3D(point.Z, point.Y, -point.X);
        private static Point3D RotateZ(Point3D point) => new Point3D(-point.Y, point.X, point.Z);

        private static List<Scanner> ParseScanners(string input)
        {
            var data = input.Lines(true).ToArray();
            var rc = new List<Scanner>();
            var points = new List<Point3D>();
            foreach (var t in data)
            {
                if (t == string.Empty)
                {
                    var beacons = new List<Beacon>();
                    var pointArray = points.ToArray();

                    foreach (var point in pointArray)
                    {
                        var distances = new List<long>();
                        foreach (var t1 in pointArray)
                        {
                            if (point == t1) continue;
                            var dx = point.X - t1.X;
                            var dy = point.Y - t1.Y;
                            var dz = point.Z - t1.Z;
                            distances.Add(dx * dx + dy * dy + dz * dz);
                        }

                        distances.Sort();
                        beacons.Add(new Beacon(point, distances.ToArray()));
                        points = new List<Point3D>();
                    }

                    rc.Add(new Scanner(new Point3D(0, 0, 0), beacons.ToArray()));
                }
                else if (!t.Contains("---"))
                {
                    var parts = t.Split(',').Select(int.Parse).ToArray();
                    var (x, y, z) = parts;
                    points.Add((new Point3D(x, y, z)));
                }
            }

            return rc;
        }

        private record Scanner(Point3D Position, Beacon[] Beacons);

        private record Beacon(Point3D Point, long[] Distances);
    }
}