using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2016
{
    public class EncryptedRoomPart : IComparable<EncryptedRoomPart>
    {
        public EncryptedRoomPart(int count, char character)
        {
            Count = count;
            Character = character;
        }

        public int Count { get; } = 1;
        public char Character { get; }

        public int CompareTo(EncryptedRoomPart other)
        {
            if (Count == other.Count)
            {
                return other.Character.CompareTo(Character);
            }
            else
            {
                return Count.CompareTo(other.Count);
            }
        }
    }
}
