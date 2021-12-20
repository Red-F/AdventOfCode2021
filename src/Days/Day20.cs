namespace AdventOfCode.Days
{
    [Day(2021, 20)]
    public class Day20 : BaseDay
    {
        public override string PartOne(string input)
        {
            (var algorithm, HashSet<(int, int)> image) = Parse(input);
            var infinitePixelColor = '.';
            for (var i = 0; i < 2; i++) (image, infinitePixelColor) = Enhance(image, algorithm, infinitePixelColor);
            return image.Count.ToString();
        }

        public override string PartTwo(string input)
        {
            (var algorithm, HashSet<(int, int)> image) = Parse(input);
            var infinitePixelColor = '.';
            for (var i = 0; i < 50; i++) (image, infinitePixelColor) = Enhance(image, algorithm, infinitePixelColor);
            return image.Count.ToString();
        }

        private static (HashSet<(int x, int y)> image, char infinitePixelColor) Enhance(HashSet<(int x, int y)> image,
            string algorithm, char infinitePixelColor)
        {
            var rc = new HashSet<(int x, int y)>();
            var boundaries = (minx: MinX(image), maxx: MaxX(image), miny: MinY(image), maxy: MaxY(image));
            for (var x = boundaries.minx - 1; x <= boundaries.maxx + 1; x++)
            for (var y = boundaries.miny - 1; y <= boundaries.maxy + 1; y++)
            {
                var index = NinePixels(x, y, image, infinitePixelColor, boundaries);
                if (algorithm[index] == '#')
                    rc.Add((x, y));
            }
            infinitePixelColor = infinitePixelColor == '.' ? algorithm[0] : algorithm[511];
            return (rc, infinitePixelColor);
        }

        private static int NinePixels(int x, int y, HashSet<(int x, int y)> image, char infinitePixelColor, (int minx, int maxx, int miny, int maxy) boundaries)
        {
            var bits = string.Empty;
            bits += PixelColor(x - 1, y - 1, image, infinitePixelColor, boundaries);
            bits += PixelColor(x, y - 1, image, infinitePixelColor, boundaries);
            bits += PixelColor(x + 1, y - 1, image, infinitePixelColor, boundaries);
            bits += PixelColor(x - 1, y, image, infinitePixelColor, boundaries);
            bits += PixelColor(x, y, image, infinitePixelColor, boundaries);
            bits += PixelColor(x + 1, y, image, infinitePixelColor, boundaries);
            bits += PixelColor(x - 1, y + 1, image, infinitePixelColor, boundaries);
            bits += PixelColor(x, y + 1, image, infinitePixelColor, boundaries);
            bits += PixelColor(x + 1, y + 1, image, infinitePixelColor, boundaries);
            return Convert.ToInt32(bits, 2);
        }

        private static char PixelColor(int x, int y, HashSet<(int x, int y)> image, char infinitePixelColor,
            (int minx, int maxx, int miny, int maxy) boundaries)
        {
            if (x < boundaries.minx || x > boundaries.maxx || y < boundaries.miny || y > boundaries.maxy)
                return infinitePixelColor == '#' ? '1' : '0';
            return image.Contains((x, y)) ? '1' : '0';
        }

        private static int MinX(IEnumerable<(int x, int y)> image) => image.Select(p => p.x).Min();
        private static int MaxX(IEnumerable<(int x, int y)> image) => image.Select(p => p.x).Max();
        private static int MinY(IEnumerable<(int x, int y)> image) => image.Select(p => p.y).Min();
        private static int MaxY(IEnumerable<(int x, int y)> image) => image.Select(p => p.y).Max();

        private static (string, HashSet<(int x, int y)>) Parse(string input)
        {
            var data = input.Lines().ToArray();
            var image = new HashSet<(int, int)>();
            var y = 0;
            foreach (var line in data.Skip(1))
            {
                for (var x = 0; x < line.Length; x++)
                    if (line[x] == '#')
                        image.Add((x, y));
                y++;
            }

            return (data.First(), image);
        }
    }
}