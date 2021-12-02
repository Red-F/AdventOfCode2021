namespace AdventOfCode.Days;

[Day(2021, 1)]
public class Day01 : BaseDay
{
    public override string PartOne(string input)
    {
        var readings = input.Integers();
        var rc = readings.Window(2).Count(x => x.Last() > x.First());
        return rc.ToString();
    }

    public override string PartTwo(string input)
    {
        var readings = input.Integers();
        var rc = readings.Window(3).Select(x => x.Sum()).Window(2).Count(x => x.Last() > x.First());
        return rc.ToString();
    }
}
