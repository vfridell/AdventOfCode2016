using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AdventOfCode2016;

namespace Advent_Of_Code_2016
{
    class Program
    {
        static void Main(string[] args)
        {
            Day13Part2();
        }

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

        public static void Day13Part2()
        {
            //int favoriteNum = 10;
            //var destination = new Day13Location() {location = new Tuple<int, int>(7, 4)};
            int favoriteNum = 1358;
            Func<Day13Location, Day13Location, double> distanceFunc = (Day13Location l1, Day13Location l2) => Math.Sqrt((l2.x - l1.x) * (l2.x - l1.x) + (l2.y - l1.y) * (l2.y - l1.y));
            var currentLocation = new Day13Location() { location = new Tuple<int, int>(1, 1), gScore = 0 };
            Func<int, int, bool> IsSpace = (x, y) =>
            {
                int z = (x * x + 3 * x + 2 * x * y + y + y * y) + favoriteNum;
                int onBits = 0;
                for (int i = 0; i < 15; i++) { if ((z & (int)Math.Pow(2, i)) > 0) onBits++; }
                return onBits % 2 == 0;
            };

            var openSet = new List<Day13Location>();
            openSet.Add(currentLocation);
            var closedSet = new List<Day13Location>();
            bool success = false;
            while (openSet.Count > 0)
            {
                currentLocation = openSet.First();
                openSet.Remove(currentLocation);
                closedSet.Add(currentLocation);
                double nextGScore = currentLocation.gScore + 1;
                if(nextGScore > 50) continue;
                for (int x = -1; x < 2; x++)
                {
                    for (int y = -1; y < 2; y++)
                    {
                        if ((x != 0 && y != 0) || x == 0 && y == 0) continue;  // cardinal directions only, no diagonal and avoid the current location
                        var neighborTuple = new Tuple<int, int>(currentLocation.x + x, currentLocation.y + y);
                        if (neighborTuple.Item1 < 0 || neighborTuple.Item2 < 0) continue; // negative values are invalid
                        if (IsSpace(currentLocation.x + x, currentLocation.y + y))
                        {
                            var neighbor = new Day13Location() { location = neighborTuple, gScore = nextGScore};
                            //if (neighbor != null && neighbor.gScore > nextGScore) openSet.Remove(neighbor);

                            //neighbor = closedSet.FirstOrDefault(l => l.Equals(new Day13Location() { location = neighborTuple }));
                            //if (neighbor != null && neighbor.gScore > nextGScore) closedSet.Remove(neighbor);

                            if (!closedSet.Contains(neighbor) && !openSet.Contains(neighbor))
                            {
                                openSet.Add(neighbor);
                                neighbor.gScore = nextGScore;
                            }
                        }
                    }
                }
            }

            // print out the path
            var gScore50Locs = closedSet.Where(l => l.gScore <= 50);
            Console.WriteLine($"There are {gScore50Locs.Count()} locations within 50 steps from the start");
        }

        public static void Day13()
        {
            //int favoriteNum = 10;
            //var destination = new Day13Location() {location = new Tuple<int, int>(7, 4)};
            int favoriteNum = 1358;
            var destination = new Day13Location() {location = new Tuple<int, int>(31, 39)};
            Func<Day13Location, Day13Location, double> distanceFunc = (Day13Location l1, Day13Location l2) => Math.Sqrt((l2.x - l1.x)*(l2.x - l1.x) + (l2.y - l1.y)*(l2.y - l1.y));
            int [,] officeSpace = new int[255,255];
            var cameFrom = new Dictionary<Day13Location, Day13Location>();
            var currentLocation = new Day13Location() {location = new Tuple<int, int>(1, 1), gScore = 0};
            currentLocation.fScore = distanceFunc(currentLocation, destination);
            Func<int, int, bool> IsSpace = (x, y) =>
            {
                int z = (x*x + 3*x + 2*x*y + y + y*y) + favoriteNum;
                int onBits = 0;
                for(int i = 0; i < 15; i++) { if((z & (int)Math.Pow(2, i)) > 0) onBits++; }
                return onBits%2 == 0;
            };

            var openSet = new List<Day13Location>();
            openSet.Add(currentLocation);
            var closedSet = new List<Day13Location>();
            bool success = false;
            while (openSet.Count > 0)
            {
                openSet.Sort();
                currentLocation = openSet.First();
                if(currentLocation.Equals(destination))
                {
                    Console.WriteLine("Success!");
                    success = true;
                    break;
                }
                openSet.Remove(currentLocation);
                closedSet.Add(currentLocation);
                double nextGScore = currentLocation.gScore + 1;
                for (int x = -1; x < 2; x++)
                {
                    for (int y = -1; y < 2; y++)
                    {
                        if((x != 0 && y != 0) || x==0 && y==0) continue;  // cardinal directions only, no diagonal and avoid the current location
                        var neighborTuple = new Tuple<int, int>(currentLocation.x + x, currentLocation.y + y);
                        if(neighborTuple.Item1 < 0 || neighborTuple.Item2 < 0) continue; // negative values are invalid
                        if (IsSpace(currentLocation.x + x, currentLocation.y + y))
                        {
                            double possibleFScore = distanceFunc(currentLocation, destination) + currentLocation.gScore;

                            var neighbor = openSet.FirstOrDefault( l => l.Equals(new Day13Location() { location = neighborTuple }));
                            if (neighbor != null && neighbor.gScore > nextGScore) openSet.Remove(neighbor);

                            neighbor = closedSet.FirstOrDefault( l => l.Equals(new Day13Location() { location = neighborTuple }));
                            if(neighbor != null && neighbor.gScore > nextGScore) closedSet.Remove(neighbor);

                            if (!closedSet.Contains(neighbor) && !openSet.Contains(neighbor))
                            {
                                if (null == neighbor) neighbor = new Day13Location() {location = neighborTuple};
                                openSet.Add(neighbor);
                                neighbor.gScore = nextGScore;
                                neighbor.fScore = nextGScore + possibleFScore;
                                cameFrom[neighbor] = currentLocation;
                            }
                        }
                    }
                }
            }

            // print out the path
            Console.WriteLine(currentLocation);
            int steps = 0;
            while (cameFrom.TryGetValue(currentLocation, out currentLocation))
            {
                Console.WriteLine(currentLocation);
                steps++;
            }
            Console.WriteLine($"Took {steps} steps");
        }

        public static void Day12()
        {
            int currentLineNumber = 0;
            Regex cmdRegex = new Regex(@"^(\w{3}) (.*)");

            var registers = new Dictionary<string, int>();
            registers["c"] = 1;

            Action<string> copyCommand = (s1) => { int x; if(int.TryParse(s1.Split(' ')[0], out x)) { registers[s1.Split(' ')[1]] = int.Parse(s1.Split(' ')[0]); } else { if (!registers.ContainsKey(s1.Split(' ')[0])) registers[s1.Split(' ')[0]] = 0; registers[s1.Split(' ')[1]] = registers[s1.Split(' ')[0]]; } };
            Action<string> incrementCommand = (s1) => { registers[s1]++; };
            Action<string> decrementCommand = (s1) => { registers[s1]--; };
            Action<string> jumpCommand = (s1) =>
            {
                int x;
                if (int.TryParse(s1.Split(' ')[0], out x))
                {
                    if(x != 0) currentLineNumber = currentLineNumber + int.Parse(s1.Split(' ')[1]);
                }
                else
                {
                    if (!registers.ContainsKey(s1.Split(' ')[0])) registers[s1.Split(' ')[0]] = 0;
                    if (registers[s1.Split(' ')[0]] != 0)
                        currentLineNumber = currentLineNumber + int.Parse(s1.Split(' ')[1]);
                }
            };
            var commandDict = new Dictionary<string, Action<string>>()
            {
                { "cpy", copyCommand},
                { "inc", incrementCommand},
                { "dec", decrementCommand},
                { "jnz", jumpCommand},
            };

            while (currentLineNumber < Inputs.Day12Input.Count)
            {
                string currentLine = Inputs.Day12Input[currentLineNumber];
                MatchCollection matches = cmdRegex.Matches(currentLine);
                int oldLineNumber = currentLineNumber;
                commandDict[matches[0].Groups[1].Value](matches[0].Groups[2].Value);
                if (currentLineNumber == oldLineNumber) currentLineNumber++;
            }

            Console.WriteLine($"Register a = {registers["a"]}");
        }

        public static void Day11Simple()
        {
            int moveCount = 0;
            int[] floorTest = {0, 5, 1, 4, 0};
            int totalPieces = floorTest.Sum();
            int elevatorPieces = Math.Min(floorTest[1], 2);
            floorTest[1] -= elevatorPieces;
            int currentFloor = 1;
            while (floorTest[4] + 1 != totalPieces)
            {
                // go down
                while (elevatorPieces < 2 && currentFloor > 1)
                {
                    currentFloor--;
                    int piecesTaken = Math.Min(floorTest[currentFloor], 2 - elevatorPieces);
                    if (piecesTaken > 0)
                    {
                        elevatorPieces += piecesTaken;
                        floorTest[currentFloor] -= piecesTaken;
                    }
                    moveCount++;
                }
                // go up
                while (currentFloor < 4)
                {
                    currentFloor++;
                    int piecesTaken = Math.Min(floorTest[currentFloor], 2 - elevatorPieces);
                    if (piecesTaken > 0)
                    {
                        elevatorPieces += piecesTaken;
                        floorTest[currentFloor] -= piecesTaken;
                    }
                    moveCount++;
                }

                floorTest[4] += 1;
                elevatorPieces--;
            }

            Console.WriteLine($"Minimum number of moves is {moveCount}");
        }

        public static void Day11()
        {
            Day11Board currentBoard = new Day11Board();
            currentBoard.fScore = currentBoard.distance;
            currentBoard.gScore = 0;

            Dictionary<Day11Board, Day11Board> cameFrom = new Dictionary<Day11Board, Day11Board>();
            List<Day11Board> openSet = new List<Day11Board>() { currentBoard };
            List<Day11Board> closedSet = new List<Day11Board>();

            bool success = false;
            int nodeNum = 0;
            while (openSet.Count > 0)
            {
                openSet.Sort();
                currentBoard = openSet[0];
                Console.WriteLine(currentBoard);
                if (currentBoard.Success)
                {
                    Console.WriteLine($"Success! fScore: {currentBoard.fScore} gScore: {currentBoard.gScore}");
                    success = true;
                    break;
                }

                openSet.Remove(currentBoard);
                closedSet.Add(currentBoard);
                foreach (Day11Board childBoard in currentBoard.GetAllBoards())
                {
                    int nextGScore = currentBoard.gScore + 1;

                    Day11Board existingBoard = openSet.FirstOrDefault(b => b.Equals(childBoard));
                    if (existingBoard != null && nextGScore < existingBoard.gScore)
                    {
                        openSet.Remove(existingBoard);
                    }

                    existingBoard = closedSet.FirstOrDefault(b => b.Equals(childBoard));
                    if (existingBoard != null && nextGScore < existingBoard.gScore)
                    {
                        closedSet.Remove(existingBoard);
                    }

                    if (!openSet.Contains(childBoard) && !closedSet.Contains(childBoard))
                    {
                        childBoard.gScore = nextGScore;
                        childBoard.fScore = childBoard.gScore + childBoard.distance;
                        openSet.Add(childBoard);
                        cameFrom[childBoard] = currentBoard;
                    }
               }

                nodeNum++;
            }

            PrintFullPath(cameFrom, currentBoard);
            if(!success) Console.WriteLine("Could not find a solution");
        }


        public static void PrintFullPath(Dictionary<Day11Board, Day11Board> cameFrom, Day11Board currentBoard)
        {
            Day11Board board = currentBoard;
            Console.WriteLine(board.ToString());
            int cnt = 0;
            while (cameFrom.ContainsKey(board))
            {
                board = cameFrom[board];
                Console.WriteLine(board.ToString());
                cnt++;
            }
            Console.WriteLine($"Solution found in {cnt} moves");
        }

        public static void Day10()
        {
            Dictionary<int, Day10Bot> bots = new Dictionary<int, Day10Bot>();
            var instructions = new List<Day10Instruction>();
            Regex regex = new Regex(@"value (\d+).* bot (\d+)|bot (\d+).* (bot|output) (\d+).* (bot|output) (\d+)");
            foreach (string input in Inputs.Day10Input)
            {
                MatchCollection matches = regex.Matches(input);
                if (matches[0].Value.Substring(0, 3) == "bot")
                {
                    int fromBot = int.Parse(matches[0].Groups[3].Value);
                    bool targetLowBot = matches[0].Groups[4].Value == "bot";
                    int lowBot = int.Parse(matches[0].Groups[5].Value);
                    bool targetHighBot = matches[0].Groups[6].Value == "bot";
                    int highBot = int.Parse(matches[0].Groups[7].Value);
                    instructions.Add(new GiveInstruction(fromBot, lowBot, targetLowBot, highBot, targetHighBot));
                }
                else
                {
                    int chipValue = int.Parse(matches[0].Groups[1].Value);
                    int botNum = int.Parse(matches[0].Groups[2].Value);
                    instructions.Add(new ValueAddInstruction(botNum, chipValue));
                }
            }

            while (instructions.Count > 0)
            {
                var newInstructions = new List<Day10Instruction>();
                foreach (var instruction in instructions)
                {
                    if (!instruction.Execute(bots))
                    {
                        newInstructions.Add(instruction);
                    }
                }
                instructions = newInstructions;
            }
            Console.WriteLine($"Output 0, 1, and 2 values multiplied: {bots[-9999].Low * bots[-1].Low * bots[-2].Low}");
        }

        public static void Day8()
        {
            Regex regex = new Regex(@"(rotate (row y=|column x=)([0-9]+) by ([0-9]+))|(rect ([0-9]+)x([0-9]+))");
            int width = 50, height = 6;
            bool[,] screen = new bool[width, height];
            Action<int, int> rect = (x, y) => { for (int i = 0; i < x; i++) for (int j = 0; j < y; j++) screen[i, j] = true; };
            Action<int, int> rotaCol = (x, n) => {
                for (int j = 0; j < n; j++)
                {
                    bool temp = false;
                    for (int i = 0; i < height - 1; i++)
                    {
                        temp = screen[x, i + 1];
                        screen[x, i + 1] = screen[x, 0];
                        screen[x, 0] = temp;
                    }
                }
            };

            Action<int, int> rotaRow = (y, n) => {
                for (int j = 0; j < n; j++)
                {
                    bool temp = false;
                    for (int i = 0; i < width - 1; i++)
                    {
                        temp = screen[i + 1, y];
                        screen[i + 1, y] = screen[0, y];
                        screen[0, y] = temp;
                    }
                    //Display(screen, height, width);
                }
            };

            
            foreach(string input in Inputs.Day8Input)
            {
                Match match = regex.Matches(input)[0];
                switch(match.Captures[0].Value.Substring(0,4))
                {
                    case "rota":
                        if (match.Groups[2].Value.Substring(0, 3) == "col") rotaCol(int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value));
                        else if (match.Groups[2].Value.Substring(0, 3) == "row") rotaRow(int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value));
                        else throw new Exception("bad rotate subCommand");
                        break;
                    case "rect":
                        rect(int.Parse(match.Groups[6].Value), int.Parse(match.Groups[7].Value));
                        break;
                    default:
                        throw new Exception("bad command");
                }
                Display(screen, height, width);
            }

            int count = 0;
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    if (screen[x, y]) count++;

            Console.WriteLine($"{count} pixels lit");

        }

        public static void Display(bool[,] screen, int height, int width)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                    Console.Write(screen[x, y] ? "X" : ".");
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public static void Day9()
        {
            int totalLength = 0;
            Regex regex = new Regex(@"(\(([0-9]+)x([0-9]+)\))(.*)");
            foreach (string input in Inputs.Day9Input)
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
            Console.WriteLine(DateTime.Now.ToLongTimeString());
            DateTime start = DateTime.Now;
            long totalLength = 0;
            Regex regex = new Regex(@"(\(([0-9]+)x([0-9]+)\))(.*)", RegexOptions.Compiled);
            foreach (string input in Inputs.Day9Input)
            {
                string decompressed = input;
                MatchCollection matches = regex.Matches(decompressed);
                while (matches.Count > 0)
                {
                    Match match = matches[0];
                    int charsAfter = int.Parse(match.Groups[2].Value);
                    int repeatNum = int.Parse(match.Groups[3].Value);
                    string repeatPiece = string.Concat(Enumerable.Repeat(match.Groups[4].Value.Substring(0, charsAfter), repeatNum));
                    string pattern = $@"\{match.Groups[1].Value.Replace(")", @"\)")}.{{{match.Groups[2]}}}";
                    string front = match.Index - 1 < 0 ? "" : decompressed.Substring(0, match.Index);
                    totalLength += front.Length;
                    string middle = decompressed.Substring(match.Index, match.Groups[1].Value.Length + charsAfter);
                    string end = decompressed.Substring(match.Index + match.Groups[1].Value.Length + charsAfter);
                    decompressed = Regex.Replace(middle, pattern, repeatPiece) + end;
                    matches = regex.Matches(decompressed);
                }
                totalLength += decompressed.Length;
            }
            Console.WriteLine($"Total length: {totalLength}");
            Console.WriteLine($"Duration in minutes: {(DateTime.Now - start).TotalMinutes}");
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
