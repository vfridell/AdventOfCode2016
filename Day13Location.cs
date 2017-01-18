using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2016
{
    class Day13Location : IComparable<Day13Location>
    {
        public Tuple<int, int> location;
        public int x => location.Item1;
        public int y => location.Item2;
        public double fScore;
        public double gScore;

        public int CompareTo(Day13Location other) => fScore.CompareTo(other.fScore);

        public override int GetHashCode() => fScore.GetHashCode() + gScore.GetHashCode() + location.Item1.GetHashCode() + location.Item2.GetHashCode();

        public override bool Equals(object obj)
        {
            if (!(obj is Day13Location)) return false;
            return Equals((Day13Location)obj);
        }

        public bool Equals(Day13Location other)
        {
            return location.Equals(other.location);
        }

        public override string ToString()
        {
            return $"({x},{y})";
        }
    }
}
