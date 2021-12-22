namespace AdventOfCode.Days
{
    [Day(2021, 22)]
    public class Day22 : BaseDay
    {
        public override string PartOne(string input)
        {
            var rules = Parse(input);
            var reactor = new Dictionary<Cuboid, long>();
            foreach (var rule in rules.Where(rule =>
                         InsideInitialisationAre(rule.cube, new Cuboid((-50, 50), (-50, 50), (-50, 50)))))
                Step(rule, reactor);
            var rc = reactor.Sum(a =>
                (a.Key.X.max - a.Key.X.min + 1L) * (a.Key.Y.max - a.Key.Y.min + 1) *
                (a.Key.Z.max - a.Key.Z.min + 1) * a.Value);
            return rc.ToString();
        }

        public override string PartTwo(string input)
        {
            var rules = Parse(input);
            var reactor = new Dictionary<Cuboid, long>();
            foreach (var rule in rules) Step(rule, reactor);
            var rc = reactor.Sum(a =>
                (a.Key.X.max - a.Key.X.min + 1L) * (a.Key.Y.max - a.Key.Y.min + 1) *
                (a.Key.Z.max - a.Key.Z.min + 1) * a.Value);
            return rc.ToString();
        }

        private static bool InsideInitialisationAre(Cuboid cuboidA, Cuboid cuboidB)
        {
            if (cuboidA.X.min < cuboidB.X.min) return false;
            if (cuboidA.X.max > cuboidB.X.max) return false;
            if (cuboidA.Y.min < cuboidB.Y.min) return false;
            if (cuboidA.Y.max > cuboidB.Y.max) return false;
            if (cuboidA.Z.min < cuboidB.Z.min) return false;
            return cuboidA.Z.max <= cuboidB.Z.max;
        }

        private static void Step((bool on, Cuboid cube) rule,
            Dictionary<Cuboid, long> reactor)
        {
            var newCuboids = new Dictionary<Cuboid, long>();
            foreach (var kvp in reactor)
            {
                var overlap = Overlap(rule.cube, kvp.Key);
                if (overlap.X.min <= overlap.X.max && overlap.Y.min <= overlap.Y.max && overlap.Z.min <= overlap.Z.max)
                    newCuboids[overlap] = newCuboids.SafeGet(overlap) - kvp.Value;
            }

            if (rule.on) newCuboids[rule.cube] = newCuboids.SafeGet(rule.cube) + 1;
            foreach (var kvp in newCuboids)
            {
                reactor[kvp.Key] = reactor.SafeGet(kvp.Key) + kvp.Value;
            }
        }

        private static Cuboid Overlap(Cuboid cuboidA, Cuboid cuboidB)
        {
            var xmin = Math.Max(cuboidA.X.min, cuboidB.X.min);
            var xmax = Math.Min(cuboidA.X.max, cuboidB.X.max);
            var ymin = Math.Max(cuboidA.Y.min, cuboidB.Y.min);
            var ymax = Math.Min(cuboidA.Y.max, cuboidB.Y.max);
            var zmin = Math.Max(cuboidA.Z.min, cuboidB.Z.min);
            var zmax = Math.Min(cuboidA.Z.max, cuboidB.Z.max);
            return new Cuboid((xmin, xmax), (ymin, ymax), (zmin, zmax));
        }

        private static List<(bool on, Cuboid cube)> Parse(string data)
        {
            return (from line in data.Lines()
                select line.Split(new[] {" x=", ",y=", ",z=", ".."}, StringSplitOptions.RemoveEmptyEntries)
                into parts
                let x1 = int.Parse(parts[1])
                let x2 = int.Parse(parts[2])
                let y1 = int.Parse(parts[3])
                let y2 = int.Parse(parts[4])
                let z1 = int.Parse(parts[5])
                let z2 = int.Parse(parts[6])
                select (parts[0] == "on",
                    new Cuboid((x1 < x2 ? x1 : x2, x1 < x2 ? x2 : x1), (y1 < y2 ? y1 : y2, y1 < y2 ? y2 : y1),
                        (z1 < z2 ? z1 : z2, z1 < z2 ? z2 : z1)))).ToList();
        }

        private record Cuboid((int min, int max) X, (int min, int max) Y, (int min, int max) Z);
    }
}