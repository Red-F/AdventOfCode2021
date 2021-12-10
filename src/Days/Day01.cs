namespace AdventOfCode.Days;

[Day(2021, 1)]
public class Day01 : BaseDay
{
    public override string PartOne(string input)
    {
        var readings = input.Integers();
        var rc = readings.Window(2).Select(x => x.ToArray()).Count(x => x[1] > x[0]);
        return rc.ToString();
    }

    public override string PartTwo(string input)
    {
        var readings = input.Integers();
        var windows = readings.Window(3).Select(x => x.Sum()).ToArray();
        var rc = 0L;
        for (var i = 0; i < windows.Length - 1; i++)
            if (windows[i + 1] > windows[i])
                rc++;
        return rc.ToString();
    }
}
