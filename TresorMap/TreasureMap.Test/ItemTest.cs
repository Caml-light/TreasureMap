using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TreasureMap.Test
{
    [TestFixture]
    public class ItemTest
    {
        [TestCase("C-3-2")]
        [TestCase("C - 3 - 2 ")]
        public void MapCreationTest(string line)
        {
            Map map = new();
            map.ParseFromLine(line);
            Assert.AreEqual(3, map.Board.GetLength(0));
            Assert.AreEqual(2, map.Board.GetLength(1));
        }

        [Test]
        public void MapInitTest()
        {
            string mapLine = "C - 3 - 4 ";
            string mountainLine = "M - 1 - 0  ";
            string treasureLine = "T - 0 - 1 - 2 ";
            string explorerLine = "A - Lara - 1 - 1 - S - AADADAGGA";

            List<string> lines = new () { mapLine, mountainLine, treasureLine, explorerLine };

            Map map = new();
            map.Init(lines);

            Assert.IsTrue(map.GetItem(1, 0) is Mountain);
            Assert.IsTrue(map.GetItem(0, 1) is Treasure);
            Assert.IsFalse(map.GetItem(1, 1).IsAvailable());
        }

        [Test]
        public void MapSaveStateTest()
        {
            string mapLine = "C - 3 - 4";
            string mountainLine = "M - 1 - 0";
            string treasureLine = "T - 0 - 1 - 2";
            string explorerLine = "A - Lara - 1 - 1 - S - AADADAGGA";

            List<string> lines = new() { mapLine, mountainLine, treasureLine, explorerLine };

            Map map = new();
            map.Init(lines);

            var resultLines = map.SaveState();

            Assert.AreEqual(mapLine, resultLines[0]);
            Assert.AreEqual(mountainLine, resultLines[1]);
            Assert.AreEqual(treasureLine, resultLines[2]);
            Assert.AreEqual("A - Lara - 1 - 1 - S - 0", resultLines[3]);
        }


        [Test]
        public void PlaceItemTest()
        {
            Item item = new Mountain("M - 0 - 1");
            Map map = MapTestHelper.GetMap();
            map.PlaceItem(item);

            Assert.IsTrue(map.Board[0, 1] is Mountain);

            Item explorer = new Explorer("A - test - 0 - 1 - E - AGAGAGAG");

            Assert.Throws<ArgumentException>(() => map.PlaceItem(explorer));


            Assert.IsTrue(map.Board[1, 1] is Grassland);
            Assert.IsTrue(map.Board[1, 1].IsAvailable());
            Assert.AreEqual(0, map.Players.Count);

            explorer.X = 1;
            map.PlaceItem(explorer);

            Assert.IsFalse(map.Board[1, 1].IsAvailable());
            Assert.AreEqual(1, map.Players.Count);

            Assert.Throws<ArgumentException>(() => map.PlaceItem(explorer));
        }

        [Test]
        public void PlayTurnTest()
        {
            string mapLine = "C - 3 - 4 ";
            string treasureLine = "T - 0 - 0 - 1 ";
            string explorerLine = "A - Lara - 0 - 1 - N - AG";

            List<string> lines = new() { mapLine, treasureLine, explorerLine };
            Map map = new();
            map.Init(lines);

            Assert.AreEqual(1, ((Treasure)map.GetItem(0, 0)).Quantity);

            Assert.AreEqual(0, map.Players[0].TreasorQuantity);
            Assert.AreEqual(0, map.Players[0].X);
            Assert.AreEqual(1, map.Players[0].Y);
            Assert.AreEqual(Orientation.N, map.Players[0].Orientation);


            Assert.IsTrue(map.PlayTurn());
            Assert.AreEqual(0, ((Treasure)map.GetItem(0, 0)).Quantity);


            Assert.AreEqual(1, map.Players[0].TreasorQuantity);
            Assert.AreEqual(0, map.Players[0].X);
            Assert.AreEqual(0, map.Players[0].Y);
            Assert.AreEqual(Orientation.N, map.Players[0].Orientation);

            Assert.IsTrue(map.PlayTurn());

            Assert.AreEqual(0, map.Players[0].X);
            Assert.AreEqual(0, map.Players[0].Y);
            Assert.AreEqual(Orientation.O, map.Players[0].Orientation);

            Assert.IsFalse(map.PlayTurn());
        }

        [Test]
        public void playerPrioTest()
        {
            string mapLine = "C - 3 - 4 ";
            string treasureLine = "T - 1 - 1 - 1 ";
            string readyLine = "A - ready - 1 - 2 - N - A";
            string playerOneLine = "A - playerOne - 1 - 0 - S - A";

            List<string> lines = new() { mapLine, treasureLine, readyLine, playerOneLine};
            Map map = new();
            map.Init(lines);

            Assert.AreEqual(1, map.Players[0].Movements.Count);
            Assert.AreEqual(1, map.Players[1].Movements.Count);

            Assert.AreEqual(0, map.Players[0].TreasorQuantity);
            Assert.AreEqual(0, map.Players[1].TreasorQuantity);

            Assert.AreEqual(1, ((Treasure)map.GetItem(1, 1)).Quantity);
            Assert.IsTrue(((Treasure)map.GetItem(1, 1)).IsAvailable());

            Assert.AreEqual(1, map.Players[0].X);
            Assert.AreEqual(2, map.Players[0].Y);
            Assert.AreEqual(1, map.Players[1].X);
            Assert.AreEqual(0, map.Players[1].Y);


            Assert.IsTrue(map.PlayTurn());


            Assert.AreEqual(0, ((Treasure)map.GetItem(1, 1)).Quantity);
            Assert.IsFalse(((Treasure)map.GetItem(1, 1)).IsAvailable());

            Assert.AreEqual(1, map.Players[0].TreasorQuantity);
            Assert.AreEqual(0, map.Players[1].TreasorQuantity);

            Assert.AreEqual(1, map.Players[0].X);
            Assert.AreEqual(1, map.Players[0].Y);
            Assert.AreEqual(1, map.Players[1].X);
            Assert.AreEqual(0, map.Players[1].Y);
        }
    }
}
