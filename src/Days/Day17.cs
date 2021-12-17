namespace AdventOfCode.Days
{
    [Day(2021, 17)]
    public class Day17 : BaseDay
    {
        public override string PartOne(string input)
        {
            // for start velocity V0 we are back at height zero with velocity (-V0 - 1)
            // the distance function is steps/2 * (2V0 - steps + 1)
            // we want to reach Ymin in 1 step with this new velocity, therefore
            // 1/2 * (2 * (-V0 - 1) - 1 + 1 = Ymin => 1/2 * (2 * (-V0 - 1)) = Ymin => V0 = -Ymin - 1
            // the maximum height is when velocity is zero this is after V0 steps
            // so the maximum height is (V0)/2 * (2V0 - V0 - 1) => (V0)/2 * (V0 - 1) => (V0) * (V0 - 1) / 2
            // substiting -Ymin -1 for V0 gives (-Ymin - 1) (-Ymin - 1 + 1) / 2 => (-Ymin - 1) (-Ymin) / 2 =>
            // -1 * (Ymin + 1) * -1 * (Ymin) / 2 => Ymin * (Ymin + 1) / 2
            var target = CreateArea(input);
            return (target.YMin * (target.YMin + 1) / 2).ToString();
        }

        public override string PartTwo(string input)
        {
            // from the previous formulae  we can get Vymax
            // Vy min is the lowes Y boundary, lower will be under the area at the first step
            // Vx min = 1 obviously
            // Vx max is highest X boundary, higher will be to right of the area at the first step
            var target = CreateArea(input);
            var vymin = target.YMin;
            var vymax = -1 * target.YMin - 1;
            var vxmin = 1;
            var vxmax = target.XMax;
            var rc = new List<(int Vx, int Vy)>();
            for (var vx = vxmin; vx <= vxmax; vx++)
            for (var vy = vymin; vy <= vymax; vy++)
                if (Hits(target, vx, vy))
                    rc.Add((vx, vy));

            return rc.Count.ToString();
        }

        private static bool Hits(TargetArea target, int vx, int vy)
        {
            int x = 0, y = 0;
            while (x <= target.XMax && y >= target.YMin)
            {
                if (x >= target.XMin && y <= target.YMax) return true;
                x += vx;
                y += vy;
                vx = Math.Max(0, vx - 1);
                vy--;
            }
            return false;
        }

        private static TargetArea CreateArea(string input)
        {
            //target area: x=20..30, y=-10..-5
            var groups = input.Split(' ');
            var xGroups = groups[2].Split('=')[1].Split('.', ',').Where(s => s != string.Empty)
                .Select(int.Parse).ToArray();
            var yGroups = groups[3].Split('=')[1].Split('.', ',').Where(s => s != string.Empty)
                .Select(int.Parse).ToArray();
            return new TargetArea(xGroups[0], xGroups[1], yGroups[0], yGroups[1]);
        }

        private record TargetArea(int XMin, int XMax, int YMin, int YMax);
    }
}