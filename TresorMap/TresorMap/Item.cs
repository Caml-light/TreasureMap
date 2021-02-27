using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TreasureMap
{
    #region BoardItems
    public class Mountain : Item
    {
        public Mountain(string line) : base(line)
        {
            var info = line.Split(FileHelper.Separator);
            if (info.Length != 3)
            {
                FileHelper.Log("Parsing Error : number of parameter for Mountain not correct");
            }

            X = Int32.Parse(info[1]);
            Y = Int32.Parse(info[2]);
        }

        public override bool IsAvailable()
        {
            return false;
        }

        public override void SetAvailability(bool available)
        {
            return; // availability is always false;
        }

        public override string ToLine()
        {
            return $"M {FileHelper.Separator} {X} {FileHelper.Separator} {Y}";
        }
    }


    public class Treasure : Item
    {
        public Treasure(string line) : base(line)
        {
            var info = line.Split(FileHelper.Separator);
            if (info.Length !=4)
            {
                FileHelper.Log("Parsing Error : number of parameter for Treasure not correct");
            }

            X = Int32.Parse(info[1]);
            Y = Int32.Parse(info[2]);
            Quantity = Int32.Parse(info[3]);
            Available = true;
        }
        public int Quantity { get; set; }
        public bool Available { get; set; }

        public override bool IsAvailable()
        {
            return Available;
        }

        public override void SetAvailability(bool available)
        {
            Available = available;
        }

        public override string ToLine()
        {
            return $"T {FileHelper.Separator} {X} {FileHelper.Separator} {Y} {FileHelper.Separator} {Quantity}";
        }
    }

    public class Grassland : Item
    {
        public Grassland(string line) : base(line)
        {
            Available = true;
        }

        public Grassland(int x, int y) : base ("")
        {
            X = x;
            Y = y;
            Available = true;
        }

        public bool Available { get; set; }

        public override bool IsAvailable()
        {
            return Available;
        }

        public override void SetAvailability(bool available)
        {
            Available = available;
        }

        public override string ToLine()
        {
            return string.Empty;
        }
    }

    public class Explorer : Item
    {
        public string Name { get; private set; }

        public Queue<char> Movements { get; private set; }

        public int TreasorQuantity { get; set; }

        public Orientation Orientation { get; set; }

        public Explorer(string line) : base(line)
        {
            var info = line.Split(FileHelper.Separator);
            if (info.Length != 6)
            {
                FileHelper.Log("Parsing Error : number of parameter for Treasure not correct");
            }

            Name = info[1].Trim();
            X = Int32.Parse(info[2]);
            Y = Int32.Parse(info[3]);
            Orientation = Enum.TryParse<Orientation>(info[4], out var orien) ? orien : Orientation.N;

            Movements = new Queue<char>(info[5].Trim());

            TreasorQuantity = 0;
        }

        public void NextMove(Map map)
        {
            char next = Movements.Dequeue();

            switch(next)
            {
                case 'A':
                    Forward(map);
                    break;
                case 'D':
                case 'G':
                    Orientation = Orientation.Turn(next);
                    break;
                default:
                    throw new NotSupportedException($"the movement {next} is not supported");
            }
        }

        public void Forward(Map map)
        {
            Item currentItem = map.Board[X, Y];
            var (xPos, yPos) = Orientation.NextPos();

            Item nextItem = map.Board[X + xPos, Y + yPos];

            if(nextItem.IsAvailable())
            {
                currentItem.SetAvailability(true);
                nextItem.SetAvailability(false);

                var treasure = nextItem as Treasure;
                if (treasure is not null)
                {
                    if(treasure.Quantity > 0)
                    {
                        treasure.Quantity--;
                        TreasorQuantity++;
                    }
                }

                X += xPos;
                Y += yPos;

            }
        }

        public override string ToLine()
        {
            return $"A {FileHelper.Separator} {Name} {FileHelper.Separator} {X} {FileHelper.Separator} {Y} {FileHelper.Separator} {Orientation} {FileHelper.Separator} {TreasorQuantity}";
        }

        public override bool IsAvailable()
        {
            throw new NotSupportedException();
        }

        public override void SetAvailability(bool available)
        {
            throw new NotSupportedException();
        }
    }

    public abstract class Item : IParsable
    {
        public int X {get; set;}
        public int Y { get; set; }

        public Item(string line)
        {
        }

        public abstract string ToLine();

        public abstract bool IsAvailable();

        public abstract void SetAvailability(bool available);
    }

    #endregion BoardItems

    public class Map : IParsable
    {
        public Item[,] Board { get; set; }

        public IList<Explorer> Players { get; set; }

        public void PlaceItem(Item item)
        {
            var explorer = item as Explorer;
            if(explorer is not null)
            {
                if(Board[item.X, item.Y].IsAvailable())
                {
                    Board[item.X, item.Y].SetAvailability(false);
                    Players.Add(explorer);
                }
                else
                {
                    throw new ArgumentException($"Explorer start position is not correct, pos {item.X}:{item.Y} is not available");
                }

            }
            else
            {
                Board[item.X, item.Y] = item;
            }
        }

        public Item GetItem(int x, int y)
        {
            return Board[x, y];
        }

        public void ParseFromLine(string line)
        {
            var info = line.Split(FileHelper.Separator);
            if (info.Length != 3)
            {
                FileHelper.Log("Parsing Error : number of parameter for map not correct");
            }

            int x = Int32.Parse(info[1]);
            int y = Int32.Parse(info[2]);

            Board = new Item[x, y];

            for (int xi = 0; xi < x; xi++)
            {
                for (int yi = 0; yi < y; yi++)
                {
                    Board[xi, yi] = new Grassland(xi, yi);
                }
            }

            Players = new List<Explorer>();
        }

        public string ToLine()
        {
            return $"C {FileHelper.Separator} {Board.GetLength(0)} {FileHelper.Separator} {Board.GetLength(1)}";
        }

        public bool PlayTurn()
        {
            var playerTurn = Players.Where(p => p.Movements.Count != 0);

            if(playerTurn is null || !playerTurn.Any())
            {
                return false;
            }

            foreach(var pt in Players)
            {
                pt.NextMove(this);
            }

            return true;
        }

        public void Init(IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                switch (line.First())
                {
                    case 'C':
                        ParseFromLine(line);
                        break;
                    case 'M':
                        Mountain mountain = new Mountain(line);
                        PlaceItem(mountain);
                        break;
                    case 'T':
                        Treasure treasure = new Treasure(line);
                        PlaceItem(treasure);
                        break;
                    case 'A':
                        Explorer explorer = new Explorer(line);
                        PlaceItem(explorer);
                        break;
                    default:
                        throw new ArgumentException("Wrong File format");
                }
            }
        }

        public List<string> SaveState()
        {
            List<string> result = new();
            result.Add(ToLine());

            foreach(Item item in Board)
            {
                string itemState = item.ToLine();
                if(itemState != string.Empty)// exclude grassLand item that return empty string
                {
                    result.Add(itemState);
                }
            }

            result = result.OrderBy(i => i).ToList();
            result.AddRange(Players.Select(p => p.ToLine()));
            return result;
        }
    }



}
