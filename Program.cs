using System;
using System.Collections.Generic;

namespace Advent_Of_Code_2016
{
    class Program
    {
        static void Main(string[] args)
        {
            Day1();
        }


        public static void Day1()
        {
            Console.WriteLine("Day 1");
            string input = "R1,R1,R3,R1,R1,L2,R5,L2,R5,R1,R4,L2,R3,L3,R4,L5,R4,R4,R1,L5,L4,R5,R3,L1,R4,R3,L2,L1,R3,L4,R3,L2,R5,R190,R3,R5,L5,L1,R54,L3,L4,L1,R4,R1,R3,L1,L1,R2,L2,R2,R5,L3,R4,R76,L3,R4,R191,R5,R5,L5,L4,L5,L3,R1,R3,R2,L2,L2,L4,L5,L4,R5,R4,R4,R2,R3,R4,L3,L2,R5,R3,L2,L1,R2,L3,R2,L1,L1,R1,L3,R5,L5,L1,L2,R5,R3,L3,R3,R5,R2,R5,R5,L5,L5,R2,L3,L5,L2,L1,R2,R2,L2,R2,L3,L2,R3,L5,R4,L4,L5,R3,L4,R1,R3,R2,R4,L2,L3,R2,L5,R5,R4,L2,R4,L1,L3,L1,L3,R1,R2,R1,L5,R5,R3,L3,L3,L2,R4,R2,L5,L1,L1,L5,L4,L1,L1,R1";
            //string input2 = "R8,R4,R4,R8";
            int[] facing = { 1, 1, -1, -1};
            List<Tuple<int,int>> history = new List<Tuple<int, int>>();
            Tuple<int, int> repeatedCoord = null;
            var currentCoord = new Tuple<int, int>(0, 0);

            int[] xy = {0,0};
            int index = 0;
            int facingIndex = 0;

            foreach (var s in input.Split(','))
            {
                if (s[0] == 'R') facingIndex++;
                else facingIndex--;

                xy[index%2] += (facing[facingIndex%4]*int.Parse(s.Substring(1)));
                index++;
                if (repeatedCoord == null)
                {

                    for(int x = currentCoord.Item1; x != xy[0];  x += (xy[0] - currentCoord.Item1) < 0 ? -1 : 1)
                    {
                        currentCoord = new Tuple<int, int>(x, xy[1]);
                        if (history.Contains(currentCoord))
                        {
                            repeatedCoord = currentCoord;
                        }
                        else
                        {
                            history.Add(currentCoord);
                        }
                    }
                    for(int y = currentCoord.Item2; y != xy[1];  y += (xy[1] - currentCoord.Item2) < 0 ? -1 : 1)
                    {
                        currentCoord = new Tuple<int, int>(xy[0], y);
                        if (history.Contains(currentCoord))
                        {
                            repeatedCoord = currentCoord;
                        }
                        else
                        {
                            history.Add(currentCoord);
                        }
                    }

                    currentCoord = new Tuple<int, int>(xy[0], xy[1]);
                }
            }

            Console.WriteLine($"Distance to final position: {Math.Abs(xy[0]) + Math.Abs(xy[1])}");
            Console.WriteLine($"Distance to target (repeated location): {Math.Abs(repeatedCoord.Item1) + Math.Abs(repeatedCoord.Item2)}");
        }
    }
}
