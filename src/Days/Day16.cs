namespace AdventOfCode.Days
{
    [Day(2021, 16)]
    public class Day16 : BaseDay
    {
        public override string PartOne(string input)
        {
            var bitStream = new BitStream(input.Trim());
            var packet = ParsePacket(bitStream);
            return packet.VersionSum().ToString();
        }

        public override string PartTwo(string input)
        {
            var bitStream = new BitStream(input.Trim());
            var packet = ParsePacket(bitStream);
            return packet.CalculatedValue().ToString();
        }

        private record Packet
        {
            public record Operator(uint Version, uint Type, IEnumerable<Packet> SubPackets) : Packet();
            public record Literal(uint Version, uint Type, ulong Value) : Packet();

            public long VersionSum() => this switch
            {
                Literal l => l.Version,
                Operator o => o.Version + o.SubPackets.Sum(p => p.VersionSum()),
                _ => throw new InvalidOperationException()
            };
            
            public ulong CalculatedValue() => this switch
            {
                Literal l => l.Value,
                Operator o => o.Type switch
                {
                    0 => o.SubPackets.Aggregate(0UL, (acc, packet) => acc + packet.CalculatedValue()),
                    1 => o.SubPackets.Aggregate(1UL, (acc, packet) => acc * packet.CalculatedValue()),
                    2 => o.SubPackets.Select(p => p.CalculatedValue()).Min(),
                    3 => o.SubPackets.Select(p => p.CalculatedValue()).Max(),
                    5 => o.SubPackets.First().CalculatedValue() > o.SubPackets.Last().CalculatedValue() ? 1UL : 0UL,
                    6 => o.SubPackets.First().CalculatedValue() < o.SubPackets.Last().CalculatedValue() ? 1UL : 0UL,
                    7 => o.SubPackets.First().CalculatedValue() == o.SubPackets.Last().CalculatedValue() ? 1UL : 0UL,
                    _ => throw new InvalidOperationException()
                },
                _ => throw new InvalidOperationException()
            };
        }

        private static Packet ParsePacket(BitStream bs)
        {
            var version = Convert.ToUInt32(bs.Take(3), 2);
            var type = Convert.ToUInt32(bs.Take(3), 2);
            return type == 4 ? new Packet.Literal(version, type, ParseLiteral(bs)) : new Packet.Operator(version, type, ParseSubPackets(bs));
        }

        private static IEnumerable<Packet> ParseSubPackets(BitStream bs)
        {
            var rc = new List<Packet>();
            var lengthTypeId = bs.Take(1);
            if (lengthTypeId == "0")
            {
                var numberOfBitsInSubPackets = Convert.ToInt32(bs.Take(15) , 2);
                var subBitStream = new BitStream(bs.Take(numberOfBitsInSubPackets));
                while (!subBitStream.EndOfStream()) rc.Add(ParsePacket(subBitStream));
            }
            else
            {
                var numberOfSubPackets = Convert.ToInt32(bs.Take(11), 2);
                for (var i = 0; i < numberOfSubPackets; i++) rc.Add(ParsePacket(bs));
            }

            return rc;
        }

        private static ulong ParseLiteral(BitStream bs)
        {
            var groups = new List<string>();
            var s = bs.Take(5);
            while (s[0] == '1')
            {
                groups.Add(s.Substring(1, 4));
                s = bs.Take(5);
            }

            groups.Add(s.Substring(1, 4));
            return Convert.ToUInt64(string.Join(string.Empty, groups), 2);
        }
        
        private class BitStream
        {
            private readonly string _binaryBuffer;
            private int _index;

            public BitStream(string digits)
            {
                _binaryBuffer = digits.All(c => c is '1' or '0') ? digits : digits.HexToBinary();
                _index = 0;
            }

            public string Take(int count)
            {
                if (count > _binaryBuffer.Length - _index)
                    throw new ArgumentException(
                        $"can not read {count} bits from stream, only {(_binaryBuffer.Length - _index)} bits left");
                var rc = _binaryBuffer.Substring(_index, count);
                _index += count;
                return rc;
            }

            public bool EndOfStream() =>
                _index >= _binaryBuffer.Length || _binaryBuffer.Substring(_index).All(c => c == '0');
        }
    }
}
