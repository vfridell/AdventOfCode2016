using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Security.Cryptography;
using AdventOfCode2016;

namespace Advent_Of_Code_2016
{
    class Program
    {
        static void Main(string[] args)
        {
            Day9Part2();
        }

        public static void Day9()
        {
            int totalLength = 0;
            Regex regex = new Regex(@"(\(([0-9]+)x([0-9]+)\))(.*)");
            foreach (string input in Inputs.Day8Input)
            {
                string decompressed = input;
                MatchCollection matches = regex.Matches(decompressed);
                while (matches.Count > 0)
                {
                    Match match = matches[0];
                    int charsAfter = int.Parse(match.Groups[2].Value);
                    int repeatNum = int.Parse(match.Groups[3].Value);
                    string repeatPiece = string.Concat(Enumerable.Repeat(match.Groups[4].Value.Substring(0, charsAfter), repeatNum));
                    repeatPiece = repeatPiece.Replace('(', '{').Replace(')', '}');
                    string pattern = $@"\{match.Groups[1].Value.Replace(")", @"\)")}.{{{match.Groups[2]}}}";
                    string front = match.Index - 1 < 0 ? "" : decompressed.Substring(0, match.Index);
                    string middle = decompressed.Substring(match.Index, match.Groups[1].Value.Length + charsAfter);
                    string end = decompressed.Substring(match.Index + match.Groups[1].Value.Length + charsAfter);
                    decompressed = front + Regex.Replace(middle, pattern, repeatPiece) + end;
                    matches = regex.Matches(decompressed);
                }
                totalLength += decompressed.Length;
            }
            Console.WriteLine($"Total length: {totalLength}");
        }

        public static void Day9Part2()
        {
            long totalLength = 0;
            Regex regex = new Regex(@"(\(([0-9]+)x([0-9]+)\))(.*)");
            foreach (string input in Inputs.Day8Input)
            {
                string decompressed = input;
                MatchCollection matches = regex.Matches(decompressed);
                while (matches.Count > 0)
                {
                    Match match = matches[0];
                    int charsAfter = int.Parse(match.Groups[2].Value);
                    int repeatNum = int.Parse(match.Groups[3].Value);
                    string repeatPiece = string.Concat(Enumerable.Repeat(match.Groups[4].Value.Substring(0, charsAfter), repeatNum));
                    //repeatPiece = repeatPiece.Replace('(', '{').Replace(')', '}');
                    string pattern = $@"\{match.Groups[1].Value.Replace(")", @"\)")}.{{{match.Groups[2]}}}";
                    string front = match.Index - 1 < 0 ? "" : decompressed.Substring(0, match.Index);
                    string middle = decompressed.Substring(match.Index, match.Groups[1].Value.Length + charsAfter);
                    string end = decompressed.Substring(match.Index + match.Groups[1].Value.Length + charsAfter);


                    totalLength += Day9Part2Recursive(middle, repeatNum);

                }
            }
            Console.WriteLine($"Total length: {totalLength}");
        }

        public static long Day9Part2Recursive(string middle, int multiplier)
        {
            
        }

        public static void Day7Part2()
        {
            Regex validABARegex1 = new Regex(@"((\[[a-z]\])+|^[a-z]*|][a-z]*)(([a-z])(?!\4)([a-z])\4).*\[[a-z]*(\5\4\5)[a-z]*\]");
            Regex validABARegex2 = new Regex(@"\[[a-z]*(([a-z])(?!\2)([a-z])\2)[a-z]*\](.*\])*[a-z]*(\3\2\3)");
            int total = 0;
            StreamWriter writer = new StreamWriter(File.OpenWrite("failed_ABA.txt"));
            foreach (string input in Inputs.Day7Input)
            {
                if (validABARegex1.IsMatch(input) || validABARegex2.IsMatch(input))
                {
                    total++;

                    var matches1 = validABARegex1.Matches(input);
                    var matches2 = validABARegex2.Matches(input);
                }
                else
                {
                    //writer.WriteLine(input);
                    Console.WriteLine(input);
                }
            }
            Console.WriteLine($"Total ABA IPs: {total}");
        }

        public static void Day7Part1()
        {
            Regex validABBARegex = new Regex(@"((\[[a-z]\])+|^[a-z]*|][a-z]*)(([a-z])(?!\4)([a-z])\5\4)");
            Regex invalidABBARegex = new Regex(@"\[[a-z]*([a-z])(?!\1)([a-z])\2\1[a-z]*\]");
            int total = 0;
            foreach (string input in Inputs.Day7Input)
            {
                if(validABBARegex.IsMatch(input) && !invalidABBARegex.IsMatch(input))
                    total++;

                var matches = validABBARegex.Matches(input);
            }
            Console.WriteLine($"Total ABBA IPs: {total}");
        }

        public static void Day6()
        {
            Regex repeatRegex = new Regex(@"([a-z])\1*");

            string message = "";
            for (int i = 0; i < Inputs.Day6Input[0].Length; i++)
            {
                string colString = Inputs.Day6Input.Aggregate("", (s1, s) => s[i] + s1);
                string orderedColString = colString.ToCharArray().OrderBy(c => c).Aggregate("", (s1, s) => s + s1);
                var stringList = new List<string>();
                foreach (Match match in repeatRegex.Matches(orderedColString))
                {
                    stringList.Add(match.Groups[0].Value);
                }
                
                message = message + stringList.OrderBy(s => s.Length).First()[0];
            }

            Console.WriteLine(message);
        }

        public static void Day5()
        {
            //string doorID = "abc";
            string doorID = "uqwqemis";
            //string password = "";
            Dictionary<int,char> password = new Dictionary<int, char>();
            int index = 0;
            while (password.Values.Count < 8)
            {
                byte[] bytes = $"{doorID}{index}".ToCharArray().Select(c => (byte) c).ToArray();
                MemoryStream stream = new MemoryStream(bytes);
                byte[] hashedBytes = MD5.Create().ComputeHash(stream);
                string hashString = hashedBytes.Aggregate<byte, string>("", (s, b) => s + b.ToString("x2"));
                if (hashString.Substring(0,5) == "00000")
                {
                    // part 1
                    //password = password + hashString[5];

                    // part 2
                    Regex digitRegex = new Regex("[0-7]");
                    if (digitRegex.IsMatch(hashString.Substring(5, 1)))
                    {
                        int pos = int.Parse(hashString.Substring(5, 1));
                        if(!password.ContainsKey(pos)) password[pos] = hashString[6];
                    }

                }
                index++;
            }
            string passwordString = $"{password[0]}{password[1]}{password[2]}{password[3]}{password[4]}{password[5]}{password[6]}{password[7]}";
            Console.WriteLine($"the password is {passwordString}");
        }

        public static void Day4()
        {
            Regex regex = new Regex(@"([a-z-]+)-([0-9]+)\[([a-z]+)\]");
            int realRoomSectorSum = 0;
            foreach (string roomCode in Inputs.Day4Input)
            {
                MatchCollection matches = regex.Matches(roomCode);
                string encryptedName = matches[0].Groups[1].Value;
                string sectorID = matches[0].Groups[2].Value;
                string checksum = matches[0].Groups[3].Value;

                // get all the characters from the encryped name and sort them
                List<EncryptedRoomPart> parts = new List<EncryptedRoomPart>();
                string stuff = encryptedName.ToCharArray()
                                            .OrderBy(c => c)
                                            .Select(c => c.ToString())
                                            .Aggregate((c, c1) => c + c1)
                                            .Replace("-", "");

                // apply counts to characters and put them in an array of special objects
                char lastChar = stuff[0];
                int currentCount = 1;
                for(int i = 1; i < stuff.Length; i++)
                {
                    if (lastChar == stuff[i])
                    {
                        currentCount++;
                    }
                    else
                    {
                        parts.Add(new EncryptedRoomPart(currentCount, lastChar));
                        currentCount = 1;
                        lastChar = stuff[i];
                    }
                }
                parts.Add(new EncryptedRoomPart(currentCount, lastChar));

                // sort the objects using the custom CompareTo to handle the digit/alpha sorting conflict
                string computedChecksum =
                    new string(
                        parts.OrderByDescending(s => s)
                            .Aggregate<EncryptedRoomPart, string>("", (s, s1) => s + s1.Character )
                            .Where(c => !char.IsDigit(c))
                            .Take(5)
                            .ToArray());

                if (computedChecksum == checksum)
                {
                    realRoomSectorSum += int.Parse(sectorID);
                    string decryptedRoomName =
                        new string(
                            encryptedName.Select(c =>
                            {
                                if (c == '-') return ' ';
                                else return (char) ((((c - 97) + int.Parse(sectorID))%26) + 97);
                            }).ToArray());
                    if (decryptedRoomName.Contains("north"))
                        Console.WriteLine($"'{decryptedRoomName}' sector {sectorID}");
                }
            }

            Console.WriteLine($"sum of the sector IDs of the real rooms: {realRoomSectorSum}");
        }

        public static void Day3()
        {
            // part 1
            //int total = 0;
            //for (int i = 0; i < Inputs.Day3Input.Length / 3; i++)
            //{
            //    if ((Inputs.Day3Input[i, 0] + Inputs.Day3Input[i, 1] <= Inputs.Day3Input[i, 2]) ||
            //        (Inputs.Day3Input[i, 0] + Inputs.Day3Input[i, 2] <= Inputs.Day3Input[i, 1]) ||
            //        (Inputs.Day3Input[i, 1] + Inputs.Day3Input[i, 2] <= Inputs.Day3Input[i, 0]))
            //    {
            //        total++;
            //    }
            //}

            // part 2
            int total = 0;
            for (int i = 0; i < Inputs.Day3Input.Length / 3; i+=3)
            {
                for (int j = 0; j < 3; j++)
                {
                    if ((Inputs.Day3Input[i, j] + Inputs.Day3Input[i + 1, j] <= Inputs.Day3Input[i + 2, j]) ||
                        (Inputs.Day3Input[i, j] + Inputs.Day3Input[i + 2, j] <= Inputs.Day3Input[i + 1, j]) ||
                        (Inputs.Day3Input[i + 1, j] + Inputs.Day3Input[i + 2, j] <= Inputs.Day3Input[i, j]))
                    {
                        total++;
                    }
                }
            }
            Console.WriteLine($"Total bad triangles: {total}");
            Console.WriteLine($"Total possible triangles: {Inputs.Day3Input.Length / 3 - total}");
        }

        public static void Day2()
        {
            int c = 0, r = 2;
            //var input = new List<string>() {"ULL",
            //                                "RRDDD",
            //                                "LURDL",
            //                                "UUUUD",};

            var input = new List<string>() {"UDRLRRRUULUUDULRULUDRDRURLLDUUDURLUUUDRRRLUUDRUUDDDRRRLRURLLLDDDRDDRUDDULUULDDUDRUUUDLRLLRLDUDUUUUDLDULLLDRLRLRULDDDDDLULURUDURDDLLRDLUDRRULDURDDLUDLLRRUDRUDDDLLURULRDDDRDRRLLUUDDLLLLRLRUULRDRURRRLLLLDULDDLRRRRUDRDULLLDDRRRDLRLRRRLDRULDUDDLDLUULRDDULRDRURRURLDULRUUDUUURDRLDDDURLDURLDUDURRLLLLRDDLDRUURURRRRDRRDLUULLURRDLLLDLDUUUDRDRULULRULUUDDULDUURRLRLRRDULDULDRUUDLLUDLLLLUDDULDLLDLLURLLLRUDRDLRUDLULDLLLUDRLRLUDLDRDURDDULDURLLRRRDUUDLRDDRUUDLUURLDRRRRRLDDUUDRURUDLLLRRULLRLDRUURRRRRLRLLUDDRLUDRRDUDUUUDRUDULRRULRDRRRDDRLUUUDRLLURURRLLDUDRUURDLRURLLRDUDUUDLLLUULLRULRLDLRDDDU",
            "DRRRDRUDRLDUUDLLLRLULLLUURLLRLDRLURDRDRDRLDUUULDRDDLDDDURURUDRUUURDRDURLRLUDRRRDURDRRRDULLRDRRLUUUURLRUULRRDUDDDDUURLDULUDLLLRULUDUURRDUULRRDDURLURRUDRDRLDLRLLULULURLRDLRRRUUURDDUUURDRDRUURUDLULDRDDULLLLLRLRLLUDDLULLUDDLRLRDLDULURDUDULRDDRLUDUUDUDRLLDRRLLDULLRLDURUDRLRRRDULUUUULRRLUDDDLDUUDULLUUURDRLLULRLDLLUUDLLUULUULUDLRRDDRLUUULDDRULDRLURUURDLURDDRULLLLDUDULUDURRDRLDDRRLRURLLRLLLLDURDLUULDLDDLULLLRDRRRDLLLUUDDDLDRRLUUUUUULDRULLLDUDLDLURLDUDULRRRULDLRRDRUUUUUURRDRUURLDDURDUURURULULLURLLLLUURDUDRRLRRLRLRRRRRULLDLLLRURRDULLDLLULLRDUULDUDUDULDURLRDLDRUUURLLDLLUUDURURUD",
            "UDUUUUURUDLLLRRRDRDRUDDRLLDRRLDRLLUURRULUULULRLLRUDDRLDRLUURDUDLURUULLLULLRRRULRLURRDDULLULULRUDDDUURDRLUDUURRRRUUULLRULLLDLURUDLDDLLRRRULDLLUURDRRRDRDURURLRUDLDLURDDRLLLUUDRUULLDLLLLUUDRRURLDDUDULUDLDURDLURUURDUUUURDLLLRUUURDUUUDLDUDDLUDDUDUDUDLDUDUUULDULUURDDLRRRULLUDRRDLUDULDURUURULLLLUDDDLURURLRLRDLRULRLULURRLLRDUDUDRULLRULRUDLURUDLLDUDLRDRLRDURURRULLDDLRLDDRLRDRRDLRDDLLLLDUURRULLRLLDDLDLURLRLLDULRURRRRDULRLRURURRULULDUURRDLURRDDLDLLLRULRLLURLRLLDDLRUDDDULDLDLRLURRULRRLULUDLDUDUDDLLUURDDDLULURRULDRRDDDUUURLLDRDURUDRUDLLDRUD",
            "ULRDULURRDDLULLDDLDDDRLDUURDLLDRRRDLLURDRUDDLDURUDRULRULRULULUULLLLDRLRLDRLLLLLRLRRLRLRRRDDULRRLUDLURLLRLLURDDRRDRUUUDLDLDRRRUDLRUDDRURRDUUUDUUULRLDDRDRDRULRLLDLDDLLRLUDLLLLUURLDLRUDRLRDRDRLRULRDDURRLRUDLRLRLDRUDURLRDLDULLUUULDRLRDDRDUDLLRUDDUDURRRRDLDURRUURDUULLDLRDUDDLUDDDRRRULRLULDRLDDRUURURLRRRURDURDRULLUUDURUDRDRLDLURDDDUDDURUDLRULULURRUULDRLDULRRRRDUULLRRRRLUDLRDDRLRUDLURRRDRDRLLLULLUULRDULRDLDUURRDULLRULRLRRURDDLDLLRUUDLRLDLRUUDLDDLLULDLUURRRLRDULRLRLDRLDUDURRRLLRUUDLUURRDLDDULDLULUUUUDRRULLLLLLUULDRULDLRUDDDRDRDDURUURLURRDLDDRUURULLULUUUDDLRDULDDLULDUDRU",
            "LRLRLRLLLRRLUULDDUUUURDULLLRURLDLDRURRRUUDDDULURDRRDURLRLUDLLULDRULLRRRDUUDDRDRULLDDULLLUURDLRLRUURRRLRDLDUDLLRLLURLRLLLDDDULUDUDRDLRRLUDDLRDDURRDRDUUULLUURURLRRDUURLRDLLUDURLRDRLURUURDRLULLUUUURRDDULDDDRULURUULLUDDDDLRURDLLDRURDUDRRLRLDLRRDDRRDDRUDRDLUDDDLUDLUDLRUDDUDRUDLLRURDLRUULRUURULUURLRDULDLDLLRDRDUDDDULRLDDDRDUDDRRRLRRLLRRRUUURRLDLLDRRDLULUUURUDLULDULLLDLULRLRDLDDDDDDDLRDRDUDLDLRLUDRRDRRDRUURDUDLDDLUDDDDDDRUURURUURLURLDULUDDLDDLRUUUULRDRLUDLDDLLLRLLDRRULULRLRDURRRLDDRDDRLU"
            };

            //int [,] keypad = { {1,2,3}, {4,5,6}, {7,8,9} };
            char[,] keypad = {  { 'X', 'X', '1', 'X', 'X' }, 
                                { 'X', '2', '3', '4', 'X' }, 
                                { '5', '6', '7', '8', '9' }, 
                                { 'X', 'A', 'B', 'C', 'X' },
                                { 'X', 'X', 'D', 'X', 'X' }
                             };

            Action U = () => { if (r > 0 && keypad[r - 1, c] != 'X') r--; };
            Action L = () => { if (c > 0 && keypad[r, c - 1] != 'X') c--; };
            Action R = () => { if (c < 4 && keypad[r, c + 1] != 'X') c++; };
            Action D = () => { if (r < 4 && keypad[r + 1, c] != 'X') r++; };

            var moveActions = new Dictionary<char, Action>() { {'U', U}, {'D', D}, {'L', L}, {'R', R} };

            string code = "";
            foreach (string directions in input)
            {
                foreach (char dir in directions)
                {
                    moveActions[dir]();
                }
                code = code + keypad[r,c];
            }

            Console.WriteLine($"Code is {code}");
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
