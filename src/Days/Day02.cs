namespace AdventOfCode.Days;

[Day(2021, 2)]
public class Day02 : BaseDay
{
    public override string PartOne(string input)
    {
        var steps = input.ParseStrings(ParseCommand).ToArray();
        var depth = 0L;
        var horizontalPosition = 0L;
        foreach (var (command, amount) in steps)
        {
            switch (command)
            {
                case "forward":
                    horizontalPosition += amount;
                    break;
                case "up":
                    depth = Math.Max(0, depth - amount);
                    break;
                case "down":
                    depth += amount;
                    break;
            }
        }
        return (horizontalPosition * depth).ToString();
    }

    public override string PartTwo(string input)
    {
        var steps = input.ParseStrings(ParseCommand).ToArray();
        var depth = 0L;
        var horizontalPosition = 0L;
        var aim = 0L;
        foreach (var (command, amount) in steps)
        {
            switch (command)
            {
                case "forward":
                    horizontalPosition += amount;
                    depth = Math.Max(0, depth + aim * amount);
                    break;
                case "up":
                    aim -= amount;
                    break;
                case "down":
                    aim += amount;
                    break;
            }
        }
        return (horizontalPosition * depth).ToString();
    }

    private static (string command, int amount) ParseCommand(string arg)
    {
        var words = arg.Words().ToArray();
        return (words[0], int.Parse(words[1]));
    }
}