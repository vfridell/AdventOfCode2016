using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace AdventOfCode2016
{
    public class Day11Board : IComparable
    {
        public int ElevatorFloor = 1;
        public Dictionary<int, List<Day11Piece>> FloorPieces = new Dictionary<int, List<Day11Piece>>();

        public bool Success => (FloorPieces[1].Count + FloorPieces[2].Count + FloorPieces[3].Count) == 0;

        private int _totalPieces = 0;
        public int TotalPieces
        {
            get
            {
                if(_totalPieces == 0)
                {
                    _totalPieces = FloorPieces.Values.Aggregate(0, (i, list) => i + list.Count);
                }
                return _totalPieces;
            }
        }

        public int fScore { get; set; }
        public int gScore { get; set; }

        int _distance = -99;
        public int distance
        {
            get
            {
                if (_distance == -99)
                {
                    int moveCount = 0;

                    int[] floorTest = { 0, FloorPieces[1].Count, FloorPieces[2].Count, FloorPieces[3].Count, FloorPieces[4].Count };
                    int elevatorPieces = ElevatorFloor == 4 ? 1 : Math.Min(FloorPieces[ElevatorFloor].Count, 2);
                    floorTest[ElevatorFloor] -= elevatorPieces;
                    int currentFloor = ElevatorFloor;
                    while (floorTest[4] + 1 != TotalPieces)
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


                    //int minMoves = (4 - ElevatorFloor);
                    //moveCount = Math.Max(minMoves, (int)Math.Ceiling(((double)FloorPieces[1].Count + (double)FloorPieces[2].Count +
                    //              (double)FloorPieces[3].Count) / (double)2));

                    _distance = moveCount;

                }
                return _distance;
            }
        }

        public Day11Board()
        {
            gScore = 0;
            fScore = 0;

            // The first floor contains a polonium generator, a thulium generator, a thulium-compatible microchip, a promethium generator, a ruthenium generator, a ruthenium-compatible microchip, a cobalt generator, and a cobalt - compatible microchip.
            // The second floor contains a polonium-compatible microchip and a promethium-compatible microchip.
            // The third floor contains nothing relevant.
            // The fourth floor contains nothing relevant.
            FloorPieces.Add(1, new List<Day11Piece>()
            {
                new Day11Piece("polonium", true),
                new Day11Piece("promethium", true),
                new Day11Piece("thulium", true),
                new Day11Piece("ruthenium", true),
                new Day11Piece("cobalt", true),
                new Day11Piece("thulium", false),
                new Day11Piece("ruthenium", false),
                new Day11Piece("cobalt", false),

                // part 2
                new Day11Piece("elerium", false),
                new Day11Piece("elerium", true),
                new Day11Piece("dilithium", false),
                new Day11Piece("dilithium", true),
            });
            FloorPieces.Add(2, new List<Day11Piece>() {
                new Day11Piece("polonium", false),
                new Day11Piece("promethium", false),
            });
            FloorPieces.Add(3, new List<Day11Piece>());
            FloorPieces.Add(4, new List<Day11Piece>());


            //// Example Input
            //FloorPieces.Add(1, new List<Day11Piece>()
            //{
            //    new Day11Piece("hydrogen", false),
            //    new Day11Piece("lithium", false),
            //});
            //FloorPieces.Add(2, new List<Day11Piece>() {
            //    new Day11Piece("hydrogen", true),
            //});
            //FloorPieces.Add(3, new List<Day11Piece>()
            //{
            //    new Day11Piece("lithium", true),
            //});

            //FloorPieces.Add(4, new List<Day11Piece>()
            //{
            //});
        }

        public void ApplyMove(Day11Move move)
        {
            if(0 == FloorPieces[move.FromFloor].RemoveAll(p => p.Equals(move.Piece))) throw new Exception("Could not remove piece 1");
            FloorPieces[move.ToFloor].Add(move.Piece);
            if (move.PieceCount == 2)
            {
                if(0 == FloorPieces[move.FromFloor].RemoveAll(p => p.Equals(move.Piece2))) throw new Exception("Could not remove piece 2");
                FloorPieces[move.ToFloor].Add(move.Piece2);
            }
            ElevatorFloor = move.ToFloor;
        }

        public List<Day11Board> GetAllBoards()
        {
            List<Day11Move> moves = GetAllMoves();
            var boardList = new List<Day11Board>();
            foreach (Day11Move move in moves)
            {
                //// reject moves that are crap
                //if (move.ToFloor > move.FromFloor)
                //{
                //    //if (move.PieceCount != 2) continue;  // reject moving up without two pieces
                //}
                //if (move.FromFloor > move.ToFloor)
                //{
                ////    //if (FloorPieces[move.ToFloor].Count == 0) continue;  // reject move of pieces to empty lower floor 
                //    if (move.PieceCount == 2) continue;  // reject move of two pieces down
                //}

                Day11Board newBoard = Clone();
                newBoard.ApplyMove(move);
                boardList.Add(newBoard);
            }
            return boardList;
        }

        public List<Day11Move> GetAllMoves()
        {
            //int totalPieces = FloorPieces.Values.Aggregate(0, (i, list) => i + list.Count);
            //if(totalPieces != 10) throw new Exception("Something went wrong");

            List<Day11Move> moves = new List<Day11Move>();
            int up = ElevatorFloor + 1 < 5 ? ElevatorFloor + 1 : -99;
            int down = ElevatorFloor - 1 > 0 ? ElevatorFloor - 1 : -99;
            
            // get available pairs on current floor
            var piecePair = from p in FloorPieces[ElevatorFloor]
                join p2 in FloorPieces[ElevatorFloor] on p.TypeName equals p2.TypeName
                where p.Key.Length < p2.Key.Length
                select new {p, p2};
            foreach (var pair in piecePair)
            {
                if(up > 0) moves.Add(new Day11Move(pair.p, pair.p2, ElevatorFloor, up));
                if(down > 0) moves.Add(new Day11Move(pair.p, pair.p2, ElevatorFloor, down));
            }

            // get singles that can move to an adjacent floor
            foreach (Day11Piece piece in FloorPieces[ElevatorFloor])
            {
                AddSingleMovesToAdjacentFloor(moves, up, piece);
                AddSingleMovesToAdjacentFloor(moves, down, piece);
            }

            // get pairs from the single moves
            IEnumerable<Day11Move> singlesPairMoves = from m in moves
                join m2 in moves on m.ToFloor equals m2.ToFloor
                where m.Piece.TypeName != m2.Piece.TypeName
                where m.Piece.GetHashCode() < m2.Piece.GetHashCode()
                where m.PieceCount == 1 && m2.PieceCount == 1
                where !m.Piece.ConflictingPair(m2.Piece)
                select new Day11Move(m.Piece, m2.Piece, ElevatorFloor, m.ToFloor);
            moves.AddRange(singlesPairMoves.ToList());

            return moves;
        }


        private void AddSingleMovesToAdjacentFloor(List<Day11Move> moves, int floor, Day11Piece pieceToMove)
        {
            if (floor > 0 && Math.Abs(floor - ElevatorFloor) == 1 )
            {
                int conflicts = 0;
                bool addedMove = false;
                foreach (var pieceDown in FloorPieces[floor])
                {
                    if (pieceDown.MatchedPair(pieceToMove))
                    {
                        addedMove = true;
                        moves.Add(new Day11Move(pieceToMove, ElevatorFloor, floor));
                    }
                    else if (pieceDown.Generator ^ pieceToMove.Generator) conflicts++;
                }
                if (conflicts == 0 && addedMove == false) moves.Add(new Day11Move(pieceToMove, ElevatorFloor, floor));
            }
        }

        public Day11Board Clone()
        {
            Day11Board newBoard = new Day11Board();
            newBoard.ElevatorFloor = ElevatorFloor;
            newBoard.FloorPieces = new Dictionary<int, List<Day11Piece>>();
            foreach (var kvp in FloorPieces)
            {
                newBoard.FloorPieces.Add(kvp.Key, new List<Day11Piece>(kvp.Value));
            }

            //int totalPieces = newBoard.FloorPieces.Values.Aggregate(0, (i, list) => i + list.Count);
            //if (totalPieces != TotalPieces) throw new Exception("Something went wrong");

            return newBoard;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Day11Board)) return false;
            return Equals((Day11Board)obj);
        }

        public bool Equals(Day11Board other)
        {
            if (other.ElevatorFloor != ElevatorFloor) return false;
            foreach (var kvp in FloorPieces)
            {
                foreach (Day11Piece piece in kvp.Value)
                {
                    if (!other.FloorPieces[kvp.Key].Contains(piece)) return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            int hashCode = ElevatorFloor*7;
            foreach (var kvp in FloorPieces)
            {
                foreach (Day11Piece piece in kvp.Value)
                {
                    hashCode += piece.GetHashCode() * 17;
                }
            }
            return hashCode;
        }



        public override string ToString()
        {
            StringBuilder output = new StringBuilder($"{distance}\n");
            for (int i = 4; i>0; i--)
            {
                output.Append(ElevatorFloor == i ? $"-->{i} " : $"   {i} ");
                foreach (Day11Piece piece in FloorPieces[i])
                {
                    output.Append(piece.TypeName.Substring(0,2)).Append(piece.Generator ? "G " : "K ");
                }
                output.AppendLine();
            }
            return output.ToString();
        }

        public int CompareTo(object obj)
        {
            if (!(obj is Day11Board)) throw new Exception("Not comparable");
            int fScoreComparison = fScore.CompareTo(((Day11Board) obj).fScore);
            if (fScoreComparison != 0) return fScoreComparison;
            return distance.CompareTo(((Day11Board) obj).distance);
        }
    }

    public struct Day11Move
    {
        public int PieceCount => pieces.Count;
        public Day11Piece Piece => pieces[0];
        public Day11Piece Piece2 => pieces[1];
        public int ToFloor { get; }
        public int FromFloor { get; }

        private List<Day11Piece> pieces;

        public Day11Move(Day11Piece piece, int fromFloor, int toFloor)
        {
            pieces = new List<Day11Piece>();
            pieces.Add(piece);
            FromFloor = fromFloor;
            ToFloor = toFloor;
        }

        public Day11Move(Day11Piece piece, Day11Piece piece2, int fromFloor, int toFloor)
        {
            pieces = new List<Day11Piece>();
            pieces.Add(piece);
            pieces.Add(piece2);
            FromFloor = fromFloor;
            ToFloor = toFloor;
        }

        public override string ToString()
        {
            string piece2Desc = PieceCount == 2 ? Piece2.ToString() : "nothing";
            return $"Move {Piece} {piece2Desc} from {FromFloor} to {ToFloor}";
        }
    }

    public struct Day11Piece
    {
        public string TypeName { get; }
        public bool Generator { get; }

        public Day11Piece(string typeName, bool generator)
        {
            TypeName = typeName;
            Generator = generator;
            Key = $"{TypeName}{Generator}";
        }

        public bool MatchedPair(Day11Piece other) => (other.Generator != Generator) && other.TypeName == TypeName;
        public bool ConflictingPair(Day11Piece other) => (other.Generator != Generator) && other.TypeName != TypeName;

        public string Key { get; }

        public override string ToString()
        {
            return Key;
        }

        public override int GetHashCode() => TypeName.GetHashCode() + Generator.GetHashCode();

        public override bool Equals(object obj)
        {
            if (!(obj is Day11Piece)) return false;
            return Equals((Day11Piece)obj);
        }

        public bool Equals(Day11Piece other)
        {
            return TypeName.Equals(other.TypeName) && Generator == other.Generator;
        }
    }
}
